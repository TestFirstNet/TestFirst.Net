using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TestFirst.Net.Lang;

namespace TestFirst.Net.Random
{
    public class RandomFiller
    {
        private const BindingFlags ReflectionFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty;
        private const int MinColSize = 2;
        private const int MaxColSize = 5;

        private readonly Random m_random = new Random();
            
        private readonly Lookup<IRandomGenerator> m_generators = new Lookup<IRandomGenerator>();
        private readonly Lookup<Func<Type, object>> m_interfacesToClass = new Lookup<Func<Type,Object>>();
        private readonly Lookup<int> m_recursionLimit = new Lookup<int>();
        private readonly Func<Type, PropertyInfo, bool> m_propertyFilter;
         
        /// <summary>
        /// The path of the current property all the way from the parent. E.g. when joined could look 
        /// like order.person.billing.address.city.name
        /// </summary>
        private readonly Stack<String> m_parentPropertyPath = new Stack<string>();

        /// <summary>
        /// All the parent types which have been filled. This is to prevent inifinite recursion trying to fill a hierachy
        /// which conatins types pointing back to itself
        /// </summary>
        private readonly Stack<Type> m_parentTypes = new Stack<Type>();

        private readonly bool m_loggingEnabled;
        private readonly bool m_failOnCircularDependency;

        public static Builder With()
        {
            return new Builder();
        }
        /// <summary>
        /// Use the builder to create me
        /// </summary>        
        private RandomFiller( //keep private so builder is used
            bool loggingEnabled,
            bool failOnCircularDependency,
            Lookup<IRandomGenerator> generators,
            Lookup<Func<Type,Object>> interfacesToClass,
            Func<Type, PropertyInfo, bool> propertyFilter,
            Lookup<int> recursionLimits
            )
        {
            m_loggingEnabled = loggingEnabled;
            m_failOnCircularDependency = failOnCircularDependency;

            RegisterDefaultGenerators();

            m_generators.SetAll(generators);
            m_interfacesToClass.SetAll(interfacesToClass);
            m_recursionLimit.SetAll(recursionLimits);

            m_propertyFilter = propertyFilter;
        }

        private void RegisterDefaultGenerators()
        {
            // ReSharper disable RedundantTypeArgumentsOfMethod

            SetGenerator<bool>(type=>m_random.Bool());
            SetGenerator<bool?>(type=>m_random.Bool());
            SetGenerator<char>(type=>m_random.Utf8Char());
            SetGenerator<char?>(type=>m_random.Utf8Char());
            SetGenerator<byte>(type=>m_random.Byte());
            SetGenerator<byte?>(type=>m_random.Byte());
            SetGenerator<short>(type=>m_random.Short());
            SetGenerator<short?>(type=>m_random.Short());
            SetGenerator<int>(type=>m_random.Int());
            SetGenerator<int?>(type=>m_random.Int());
            SetGenerator<long>(type=>m_random.Long());
            SetGenerator<long?>(type=>m_random.Long());
            SetGenerator<double>(type=>m_random.Double());
            SetGenerator<double?>(type=>m_random.Double());
            SetGenerator<float>(type=>m_random.Float());
            SetGenerator<float?>(type=>m_random.Float());

            SetGenerator<String>(type=>m_random.Utf8String());

            SetGenerator<UInt16>(type=>m_random.UInt16());
            SetGenerator<UInt16?>(type=>m_random.UInt16());
            SetGenerator<Int16>(type=>m_random.Int16());
            SetGenerator<Int16?>(type=>m_random.Int16());

            SetGenerator<UInt32>(type=>m_random.UInt32());
            SetGenerator<UInt32?>(type=>m_random.UInt32());
            SetGenerator<Int32>(type=>m_random.Int32());
            SetGenerator<Int32?>(type=>m_random.Int32());

            SetGenerator<UInt64>(type=>m_random.UInt64());
            SetGenerator<UInt64?>(type=>m_random.UInt64());
            SetGenerator<Int64>(type=>m_random.Int64());
            SetGenerator<Int64?>(type=>m_random.Int64());

            SetGenerator<SByte>(type=>m_random.SByte());
            SetGenerator<SByte?>(type=>m_random.SByte());

            SetGenerator<Decimal>(type=>m_random.Decimal());
            SetGenerator<Decimal?>(type=>m_random.Decimal());
            SetGenerator<Guid>(type=>m_random.Guid());
            SetGenerator<Guid?>(type=>m_random.Guid());
            SetGenerator<DateTime>(type=>m_random.DateTime());
            SetGenerator<DateTime?>(type=>m_random.DateTime());
            SetGenerator<TimeSpan>(type=>m_random.TimeSpan());
            SetGenerator<TimeSpan?>(type=>m_random.TimeSpan());
            SetGenerator<Object>(type=>m_random.Object());

            // ReSharper restore RedundantTypeArgumentsOfMethod
        }

        private void SetGenerator<TPropertyType>(Func<Type,TPropertyType> func)
        {
            var type = typeof(TPropertyType);
            var generator = new FuncGenerator<TPropertyType>(func);
            SetGenerator(type, generator);  
        }

        private void SetGenerator(Type type, IRandomGenerator gen)
        {
            Debug("Registering generator for type {0}", type.PrettyName());                
            m_generators.SetForType(type,gen);
        }

        public Object FillWithRandom(Type t)
        {
            return InternalCreateRandomFor(null, null, t);
        }

        public T FillWithRandom<T>()
        {
            return (T)InternalCreateRandomFor(null, null, typeof(T));
        }

        public T FillWithRandom<T>(T instance)
        {
            return (T)InternalFillWithRandom((Object)instance);
        }

        public Object FillWithRandom(Object instance)
        {
            return InternalFillWithRandom(instance);
        }

        private Object InternalFillWithRandom(Object instance)
        {
            Debug("Fill with random type {0}", instance.GetType().PrettyName());
            var type = instance.GetType();
            var properties = type.GetProperties(ReflectionFlags);
            foreach (var prop in properties)
            {
                try
                {
                    if (prop.CanRead && prop.CanWrite && prop.GetIndexParameters().Length == 0 && IncludeProp(type, prop))
                    {
                        SetRandomValue(instance, prop);
                    }
                }
                catch (Exception e)
                {
                    throw new RandomFillException(
                        String.Format("Error randomly filling property named '{0}' on type {1}",
                            prop.Name,
                            instance == null ? null : instance.GetType().PrettyName()), e);
                }
            }
            return instance;
        }

        private bool IncludeProp(Type onInstance, PropertyInfo prop)
        {
            return m_propertyFilter.Invoke(onInstance,prop);
        }

        private void SetRandomValue(Object instance, PropertyInfo prop)
        {
            if (PushProp(instance.GetType(), prop))
            {
                try
                {
                    var val = InternalCreateRandomFor(instance.GetType(), prop);
                    Debug("Setting value for prop '{0}',  value '{1}'", prop.Name, val);
                    //no indexed prop at the mo!
                    prop.SetValue(instance, val, null);
                }
                finally
                {
                    PopProp();
                }
            }
        }

        private Object InternalCreateRandomFor(Type onInstanceType, PropertyInfo prop)
        {
            return InternalCreateRandomFor(onInstanceType, prop.Name, prop.PropertyType);
        }

        private Object InternalCreateRandomFor(Type onInstanceType, String propertyName, Type propertyType)
        {
            var generator = GetGeneratorFor(onInstanceType, propertyName, propertyType);
            if (generator != null)
            {
                var val = generator.CreateValue(onInstanceType, propertyName, propertyType);
                Debug("For prop '{0}', type '{1}', generated val '{2}'", propertyName, propertyType.PrettyName(), val);
                return val;
            }
            throw new RandomFillException(String.Format(
                        "Couldn't generate random for  {0}.{1} of type {2}. Consider registering " +
                        "a custom generator for the given type. Remember to register the nullable " +
                        "version also", 
                        onInstanceType.PrettyName(), propertyName, propertyType.PrettyName()));                          
        }

        private bool PushProp(Type onType, PropertyInfo prop)
        {
            bool processProperty = true;
            if (m_parentTypes.Contains(prop.PropertyType))
            {
                processProperty = CheckRecursionLimit(onType, prop);
            }
            if (processProperty)
            {
                m_parentPropertyPath.Push(prop.Name);
                m_parentTypes.Push(prop.PropertyType);
            }
            return processProperty;
        }

        private bool CheckRecursionLimit(Type onType, PropertyInfo prop)
        {
            int limit;
            if (m_recursionLimit.TryGetValue(onType, prop.Name, prop.PropertyType, out limit))
            {
                //count parents
                var depth = m_parentTypes.Count((type) => type == prop.PropertyType);
                if (depth < limit)
                {
                    return true;
                }

                //caller is aware of this circular dependency, set a limit, so probably just wants to ignore now
                return false;
            }

            if (!m_failOnCircularDependency)
            {
                return false;
            }
            throw new RandomFillException(
                String.Format(
                    "Circular dependency detected on type '{0}', for property with name '{1}', of type '{2}', full propertyPath is '{3}'",
                    onType.PrettyName(),
                    prop.Name,
                    prop.PropertyType.PrettyName(),
                    String.Join(".", m_parentPropertyPath)
                    ));
        }

        private void PopProp()
        {
            m_parentPropertyPath.Pop();
            m_parentTypes.Pop();
        }

        private IRandomGenerator GetGeneratorFor(Type onInstanceType, String propertyName, Type propertyType)
        {
            Type type = ExtractUnderlyingTypeIfNullable(propertyType);
            Debug("Try get generator for propertyName '{0}', type '{1}'", propertyName, type.PrettyName());

            IRandomGenerator generator;
            if (m_generators.TryGetValue(onInstanceType, propertyName, propertyType, out generator))
            {
                Debug("Found existing generator");
                return generator;
            }

            generator = CreateGeneratorFor(onInstanceType, propertyName, propertyType);
            if (generator != null)
            {
                Debug("Created new generator");
                SetGenerator(type,generator);
            }
            return generator;
        }

        private IRandomGenerator CreateGeneratorFor(Type onInstanceType, String propertyName, Type propertyType)
        {
            Debug("Creating new generator");

            Type type = ExtractUnderlyingTypeIfNullable(propertyType);
            if (type.IsEnum)
            {
                Debug("IsEnum");
                return CreateEnumGeneratorFor(type);
            }
            if (type.IsArray)
            {
                Debug("IsArray");
                return CreateArrayGenerator();
            }

            if (type.IsGenericType)
            {
                Debug("IsGenericType");

                if (ImplementsInterface(type,typeof(IDictionary<,>)))
                {
                    Debug("IsDictionary");
                    return CreateDictionaryGeneratorFor(type);
                }
                if (ImplementsInterface(type,typeof(ICollection<>)) || ImplementsInterface(type,typeof(IList<>)))
                {
                    Debug("IsList/IsCol");
                    return CreateListGeneratorFor(type);
                }
            }
            if(type.IsInterface)
            {
                Debug("IsInterface");
                return CreateInterfaceGenerator(onInstanceType, propertyName, type);
            }
            if (type.IsClass)
            {
                Debug("IsClass");
                return CreateClassInstanceGenerator();
            }
            Debug("Could not create generator");
            return null;
        }

        private FuncGenerator<object> CreateClassInstanceGenerator()
        {
            var gen = new FuncGenerator<Object>((genType) =>
                {
                    Object propertyInstance;
                    try
                    {
                        propertyInstance = Activator.CreateInstance(genType);
                    }
                    catch (Exception e)
                    {
                        throw new RandomFillException("Error creating new instance for type:" + genType.PrettyName(), e);
                    }
                    InternalFillWithRandom(propertyInstance);
                    return propertyInstance;
                });
            return gen;
        }

        private FuncGenerator<object> CreateInterfaceGenerator(Type onInstanceType, string propertyName, Type type)
        {
            var gen = new FuncGenerator<Object>((genType) =>
                {
                    var propertyInstance = CreateInstanceFromInterface(onInstanceType, propertyName, type);
                    InternalFillWithRandom(propertyInstance);
                    return propertyInstance;
                });
            return gen;
        }

        private FuncGenerator<object> CreateEnumGeneratorFor(Type type)
        {
            return new FuncGenerator<Object>(genType => m_random.EnumOf(type));
        }

        private FuncGenerator<object> CreateArrayGenerator()
        {
            var gen = new FuncGenerator<Object>((parentType, listPropertyName, arrayType) =>
                {
                    Type itemType = arrayType.GetElementType();
                    var numItems = m_random.IntBetween(MinColSize, MaxColSize + 1);
                    dynamic array = Array.CreateInstance(itemType, numItems);
                    for (int i = 0; i < numItems; i++)
                    {
                        dynamic item = InternalCreateRandomFor(parentType, listPropertyName + ".Value", itemType);
                        array[i] = item;
                    }
                    return array;
                });
            return gen;
        }

        private FuncGenerator<object> CreateDictionaryGeneratorFor(Type type)
        {
            //perform a bunch of precalc
            Type keyType = type.GetGenericArguments()[0];
            Type valueType = type.GetGenericArguments()[1];
            Type concreteDictType;
            if(type.IsInterface)
            {
                concreteDictType = typeof (Dictionary<,>).MakeGenericType(type.GetGenericArguments());
            }
            else
            {
                concreteDictType = type;
            }
            var concreteDictCtor = concreteDictType.GetConstructor(new Type[] {});

            var gen = new FuncGenerator<Object>((parentType, dictPropertyName, dictType) =>
                {
                    dynamic dict = concreteDictCtor.Invoke(new object[] {});
                    var numItems = m_random.IntBetween(MinColSize, MaxColSize + 1);
                    //don't use the dicationary count as we may be using a static key dependening on the user generator
                    for (int i = 0; i < numItems; i++)
                    {
                        //todo:allow customisation or keys?
                        dynamic key = InternalCreateRandomFor(parentType, dictPropertyName + ".Key", keyType);
                        dynamic val = InternalCreateRandomFor(parentType, dictPropertyName + ".Value", valueType);
                        if (!dict.ContainsKey(key))
                        {
                            dict.Add(key, val);
                        }
                    }
                    return dict;
                });
            return gen;
        }

        private FuncGenerator<object> CreateListGeneratorFor(Type type)
        {
            //perform a bunch of precalc
            Type itemType = type.GetGenericArguments()[0];
            Type concreteListType;
            if(type.IsInterface)
            {
                concreteListType = typeof (List<>).MakeGenericType(type.GetGenericArguments());
            }
            else
            {
                concreteListType = type;
            }
            var concreteListCtor = concreteListType.GetConstructor(new Type[] {});

            var gen = new FuncGenerator<Object>((parentType, listPropertyName, listType) =>
                {
                    dynamic list = concreteListCtor.Invoke(new object[] {});
                    var numItems = m_random.IntBetween(MinColSize, MaxColSize + 1);
                    //don't use the list count as we may be using a static key dependening on the user generator
                    //and the lis may only allow uniques
                    for (int i = 0; i < numItems; i++)
                    {
                        dynamic item = InternalCreateRandomFor(parentType, listPropertyName + ".Value", itemType);
                        list.Add(item);
                    }
                    return list;
                });
            return gen;
        }


        private Type ExtractUnderlyingTypeIfNullable(Type type)
        {
            bool isNullable = !type.IsValueType || (Nullable.GetUnderlyingType(type) != null);
            if (isNullable && Nullable.GetUnderlyingType(type) != null)
            {
                type = Nullable.GetUnderlyingType(type);
            }
            return type;
        }

        private bool ImplementsInterface(Type t,Type interfaceType)
        {
            if (t.Name == interfaceType.Name && t.Namespace == interfaceType.Namespace) 
            {
                return true;
            }                    
            foreach (Type interfce in t.GetInterfaces())
            {
                if (interfce.Name == interfaceType.Name && interfce.Namespace == interfaceType.Namespace) 
                {
                    return true;
                }
            }
            return false;
        }

        private Object CreateInstanceFromInterface(Type onInstanceType, String propertyName, Type type)
        {
            Debug("Looking for interface instance factory for type {0}", type.PrettyName());

            Func<Type,Object> instanceFactory;

            if (m_interfacesToClass.TryGetValue(onInstanceType,propertyName,type, out instanceFactory))
            {
                return instanceFactory.Invoke(type);
            }

            throw new RandomFillException(
                String.Format("Couldn't find a concrete implementation for interface {0} for {1}.{2}",
                              type.PrettyName(), 
                              onInstanceType.PrettyName(),
                              propertyName));
        }

        private void Debug(String msg,params Object[] args)
        {
            if (m_loggingEnabled)
            {
                if (args != null && args.Length > 0)
                {
                    System.Diagnostics.Debug.WriteLine("[" + GetType().Name + "] " + String.Format(msg, args));
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[" + GetType().Name + "] " + msg);
                }
            }
        }

        private class FuncGenerator<TPropertyType> : IRandomGenerator
        {
            private readonly Func<Type,String,Type,TPropertyType> m_createRandomFunc;

            public FuncGenerator(Func<TPropertyType> createRandomFunc)
            {
                m_createRandomFunc = (onType,propertyName,propertyType) => { return createRandomFunc.Invoke(); };
            }

            public FuncGenerator(Func<Type, TPropertyType> createRandomFunc)
            {
                m_createRandomFunc = (onType,propertyName,propertyType) => { return createRandomFunc.Invoke(propertyType); };
            }

            public FuncGenerator(Func<Type, String, Type, TPropertyType> createRandomFunc)
            {
                m_createRandomFunc = createRandomFunc;
            }

            public object CreateValue(Type onType,String propertyName,Type propertyType)
            {
                return m_createRandomFunc.Invoke(onType,propertyName,propertyType);
            }
        }
        
        /// <summary>
        /// Generates a random value
        /// </summary>
        public interface IRandomGenerator
        {
            /// <summary>
            /// Creates a value for the given property
            /// </summary>
            /// <param name="onType">the owning type of this property. Could be null</param>
            /// <param name="propertyName">the name of the property. Could be null</param>
            /// <param name="propertyType">the type to generate</param>
            /// <returns>a new instance for the given property type</returns>
            Object CreateValue(Type onType, String propertyName, Type propertyType);
        }

        public class Builder : IBuilder<RandomFiller>
        {
            private static readonly Func<Type,PropertyInfo, bool> IncludeAllPropertiesFilter = (type,prop)=>true; 

            private bool m_enableLogging = false;
            private bool m_failOnCircularDependency = true;

            private readonly Lookup<IRandomGenerator> m_generators = new Lookup<IRandomGenerator>();
            private readonly Lookup<Func<Type,Object>> m_interfacesToClass = new Lookup<Func<Type,Object>>();
            private readonly Lookup<int> m_recursionLimit = new Lookup<int>();

            private Func<Type, PropertyInfo, bool> m_propertyFilter = IncludeAllPropertiesFilter;

            public RandomFiller Build()
            {
                return new RandomFiller(
                    m_enableLogging,
                    m_failOnCircularDependency,
                    m_generators,
                    m_interfacesToClass,
                    m_propertyFilter??IncludeAllPropertiesFilter, 
                    m_recursionLimit);
            }

            /// <summary>
            /// Log what the filler is doing to the System Debug
            /// </summary>
            public Builder EnableLogging(bool val)
            {
                m_enableLogging = val;
                return this;
            }

            /// <summary>
            /// If set (default is true), when a dto has a child dto of the same type, fail. Else silently 
            /// swallow error (setting child to null).
            /// </summary>
            public Builder FailOnCircularDependencies(bool val)
            {
                m_failOnCircularDependency = val;
                return this;
            }

            public Builder InterfaceToImplementation<TInterface,TConcrete>()
                where TConcrete:TInterface, new()
                where TInterface:class
            {                
                InterfaceToImplementation<TInterface>(type => Activator.CreateInstance<TConcrete>());
                return this;
            }

            public Builder InterfaceToImplementation<TInterface>(Func<Type,TInterface> objectFactory)
                where TInterface:class
            {           
                m_interfacesToClass.SetForType(typeof(TInterface), objectFactory);
                return this;
            }

            public Builder InterfaceToImplementation(Type interfaceType, Func<Type, Object> objectFactory)
            {           
                m_interfacesToClass.SetForType(interfaceType, objectFactory);
                return this;
            }

            public Builder GeneratorForType<TPropertyType>(Func<TPropertyType> func)
            {
                var type = typeof (TPropertyType);
                var generator = new FuncGenerator<TPropertyType>(func);
                GeneratorForType(type, generator); 
                return this; 
            }

            public Builder GeneratorForNamedPropertyOnType<TInstanceType,TPropertyType>(String propertyName,Func<TPropertyType> func)
            {
                m_generators.SetOnTypeForPropertyName(
                    typeof(TInstanceType),
                    propertyName,
                    new FuncGenerator<TPropertyType>(func));
                return this; 
            }

            public Builder GeneratorForTypeOnType<TPropertyType,TInstanceType>(Func<TPropertyType> func)
            {
                m_generators.SetOnTypeForPropertyType(
                    typeof(TInstanceType),
                    typeof(TPropertyType),
                    new FuncGenerator<TPropertyType>(func));
                return this; 
            }

            public Builder GeneratorForType(Type type,Func<Object> func)
            {          
                m_generators.SetForType(type,new FuncGenerator<Object>(func));
                return this;
            }

            public Builder GeneratorForType(Type type, IRandomGenerator gen)
            {               
                m_generators.SetForType(type,gen);

                return this;
            }

            public Builder SelectPropertiesToFill(Func<Type,PropertyInfo,bool> returnTrueForPropertiesToSet)
            {
                m_propertyFilter = returnTrueForPropertiesToSet;
                return this;
            }

            public Builder RecursionLimitOnTypeWithName(Type onType,String name, int limit)
            {           
                PreConditions.AssertTrue(limit > 0, "Limit must be greater than 0");
                m_recursionLimit.SetOnTypeForPropertyName(onType, name, limit);
                return this;
            }

            public Builder RecursionLimitOnTypeForType(Type onType,Type forType, int limit)
            {          
                PreConditions.AssertTrue(limit > 0, "Limit must be greater than 0");
                m_recursionLimit.SetOnTypeForPropertyType(onType, forType, limit);
                return this;
            }
        }
    }
}