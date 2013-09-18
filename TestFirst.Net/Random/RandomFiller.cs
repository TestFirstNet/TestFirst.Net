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
            
        private readonly Lookup<IRandomValueFactory> m_randomValueFactories = new Lookup<IRandomValueFactory>();
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
            Lookup<IRandomValueFactory> generators,
            Lookup<Func<Type,Object>> interfacesToClass,
            Func<Type, PropertyInfo, bool> propertyFilter,
            Lookup<int> recursionLimits
            )
        {
            m_loggingEnabled = loggingEnabled;
            m_failOnCircularDependency = failOnCircularDependency;

            RegisterDefaultGenerators();

            m_randomValueFactories.SetAll(generators);
            m_interfacesToClass.SetAll(interfacesToClass);
            m_recursionLimit.SetAll(recursionLimits);

            m_propertyFilter = propertyFilter;
        }

        private void RegisterDefaultGenerators()
        {
            // ReSharper disable RedundantTypeArgumentsOfMethod

            SetValueFactoryFor<bool>(type=>m_random.Bool());
            SetValueFactoryFor<bool?>(type=>m_random.Bool());
            SetValueFactoryFor<char>(type=>m_random.Utf8Char());
            SetValueFactoryFor<char?>(type=>m_random.Utf8Char());
            SetValueFactoryFor<byte>(type=>m_random.Byte());
            SetValueFactoryFor<byte?>(type=>m_random.Byte());
            SetValueFactoryFor<short>(type=>m_random.Short());
            SetValueFactoryFor<short?>(type=>m_random.Short());
            SetValueFactoryFor<int>(type=>m_random.Int());
            SetValueFactoryFor<int?>(type=>m_random.Int());
            SetValueFactoryFor<long>(type=>m_random.Long());
            SetValueFactoryFor<long?>(type=>m_random.Long());
            SetValueFactoryFor<double>(type=>m_random.Double());
            SetValueFactoryFor<double?>(type=>m_random.Double());
            SetValueFactoryFor<float>(type=>m_random.Float());
            SetValueFactoryFor<float?>(type=>m_random.Float());

            SetValueFactoryFor<String>(type=>m_random.Utf8String());

            SetValueFactoryFor<UInt16>(type=>m_random.UInt16());
            SetValueFactoryFor<UInt16?>(type=>m_random.UInt16());
            SetValueFactoryFor<Int16>(type=>m_random.Int16());
            SetValueFactoryFor<Int16?>(type=>m_random.Int16());

            SetValueFactoryFor<UInt32>(type=>m_random.UInt32());
            SetValueFactoryFor<UInt32?>(type=>m_random.UInt32());
            SetValueFactoryFor<Int32>(type=>m_random.Int32());
            SetValueFactoryFor<Int32?>(type=>m_random.Int32());

            SetValueFactoryFor<UInt64>(type=>m_random.UInt64());
            SetValueFactoryFor<UInt64?>(type=>m_random.UInt64());
            SetValueFactoryFor<Int64>(type=>m_random.Int64());
            SetValueFactoryFor<Int64?>(type=>m_random.Int64());

            SetValueFactoryFor<SByte>(type=>m_random.SByte());
            SetValueFactoryFor<SByte?>(type=>m_random.SByte());

            SetValueFactoryFor<Decimal>(type=>m_random.Decimal());
            SetValueFactoryFor<Decimal?>(type=>m_random.Decimal());
            SetValueFactoryFor<Guid>(type=>m_random.Guid());
            SetValueFactoryFor<Guid?>(type=>m_random.Guid());
            SetValueFactoryFor<DateTime>(type=>m_random.DateTime());
            SetValueFactoryFor<DateTime?>(type=>m_random.DateTime());
            SetValueFactoryFor<TimeSpan>(type=>m_random.TimeSpan());
            SetValueFactoryFor<TimeSpan?>(type=>m_random.TimeSpan());
            SetValueFactoryFor<Object>(type=>m_random.Object());

            // ReSharper restore RedundantTypeArgumentsOfMethod
        }

        private void SetValueFactoryFor<TPropertyType>(Func<Type,TPropertyType> func)
        {
            var type = typeof(TPropertyType);
            var generator = new RandomValueViaFunction<TPropertyType>(func);
            SetValueFactoryFor(type, generator);  
        }

        private void SetValueFactoryFor(Type type, IRandomValueFactory gen)
        {
            Debug("Registering generator for type {0}", type.PrettyName());                
            m_randomValueFactories.SetForType(type,gen);
        }

        public Object FillWithRandom(Type t)
        {
            return InternalNewRandomValueFor(null, null, t);
        }

        public T FillWithRandom<T>()
        {
            return (T)InternalNewRandomValueFor(null, null, typeof(T));
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
                    var val = InternalNewRandomValueFor(instance.GetType(), prop);
                    Debug("Setting value for property '{0}',  value '{1}'", prop.Name, val);
                    //no indexed prop at the mo!
                    prop.SetValue(instance, val, null);
                }
                finally
                {
                    PopProp();
                }
            }
        }

        private Object InternalNewRandomValueFor(Type onInstanceType, PropertyInfo prop)
        {
            return InternalNewRandomValueFor(onInstanceType, prop.Name, prop.PropertyType);
        }

        private Object InternalNewRandomValueFor(Type onInstanceType, String propertyName, Type propertyType)
        {
            var generator = GetValueFactoryFor(onInstanceType, propertyName, propertyType);
            if (generator != null)
            {
                var val = generator.CreateValue(onInstanceType, propertyName, propertyType);
                Debug("For property '{0}', type '{1}', generated value '{2}'", propertyName, propertyType.PrettyName(), val);
                return val;
            }
            throw new RandomFillException(String.Format(
                        "Couldn't generate random for  {0}.{1} of type {2}. Consider registering " +
                        "a custom value factory for the given type. Remember to register the nullable " +
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
                    "Circular dependency detected on type '{0}', for property with name '{1}', of type '{2}', full property path is '{3}'",
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

        private IRandomValueFactory GetValueFactoryFor(Type onInstanceType, String propertyName, Type propertyType)
        {
            Type type = ExtractUnderlyingTypeIfNullable(propertyType);
            Debug("Try get value factory for propertyName '{0}', type '{1}'", propertyName, type.PrettyName());

            IRandomValueFactory factory;
            if (m_randomValueFactories.TryGetValue(onInstanceType, propertyName, propertyType, out factory))
            {
                Debug("Found existing value factory");
                return factory;
            }

            factory = NewValueFactoryFor(onInstanceType, propertyName, propertyType);
            if (factory != null)
            {
                Debug("Created new value factory");
                SetValueFactoryFor(type,factory);
            }
            return factory;
        }

        private IRandomValueFactory NewValueFactoryFor(Type onInstanceType, String propertyName, Type propertyType)
        {
            Debug("Creating new value factory");

            Type type = ExtractUnderlyingTypeIfNullable(propertyType);
            if (type.IsEnum)
            {
                Debug("IsEnum");
                return NewEnumFactoryFor(type);
            }
            if (type.IsArray)
            {
                Debug("IsArray");
                return NewArrayFactory();
            }

            if (type.IsGenericType)
            {
                Debug("IsGenericType");

                if (ImplementsInterface(type,typeof(IDictionary<,>)))
                {
                    Debug("IsDictionary");
                    return NewDictionaryFactoryFor(type);
                }
                if (ImplementsInterface(type,typeof(ICollection<>)) || ImplementsInterface(type,typeof(IList<>)))
                {
                    Debug("IsList/IsCol");
                    return NewListFactoryFor(type);
                }
            }
            if(type.IsInterface)
            {
                Debug("IsInterface");
                return NewInterfaceFactoryFor(onInstanceType, propertyName, type);
            }
            if (type.IsClass)
            {
                Debug("IsClass");
                return NewClassInstanceFactory();
            }
            Debug("Could not create value factory");
            return null;
        }

        private RandomValueViaFunction<object> NewClassInstanceFactory()
        {
            var factory = new RandomValueViaFunction<Object>((genType) =>
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
            return factory;
        }

        private RandomValueViaFunction<object> NewInterfaceFactoryFor(Type onInstanceType, string propertyName, Type type)
        {
            var factory = new RandomValueViaFunction<Object>((genType) =>
                {
                    var propertyInstance = NewInstanceFromInterface(onInstanceType, propertyName, type);
                    InternalFillWithRandom(propertyInstance);
                    return propertyInstance;
                });
            return factory;
        }

        private RandomValueViaFunction<object> NewEnumFactoryFor(Type type)
        {
            return new RandomValueViaFunction<Object>(genType => m_random.EnumOf(type));
        }

        private RandomValueViaFunction<object> NewArrayFactory()
        {
            var factory = new RandomValueViaFunction<Object>((parentType, listPropertyName, arrayType) =>
                {
                    Type itemType = arrayType.GetElementType();
                    var numItems = m_random.IntBetween(MinColSize, MaxColSize + 1);
                    dynamic array = Array.CreateInstance(itemType, numItems);
                    for (int i = 0; i < numItems; i++)
                    {
                        dynamic item = InternalNewRandomValueFor(parentType, listPropertyName + ".Value", itemType);
                        array[i] = item;
                    }
                    return array;
                });
            return factory;
        }

        private RandomValueViaFunction<object> NewDictionaryFactoryFor(Type type)
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

            var factory = new RandomValueViaFunction<Object>((parentType, dictPropertyName, dictType) =>
                {
                    dynamic dict = concreteDictCtor.Invoke(new object[] {});
                    var numItems = m_random.IntBetween(MinColSize, MaxColSize + 1);
                    //don't use the dicationary count as we may be using a static key dependening on the user generator
                    for (int i = 0; i < numItems; i++)
                    {
                        //todo:allow customisation or keys?
                        dynamic key = InternalNewRandomValueFor(parentType, dictPropertyName + ".Key", keyType);
                        dynamic val = InternalNewRandomValueFor(parentType, dictPropertyName + ".Value", valueType);
                        if (!dict.ContainsKey(key))
                        {
                            dict.Add(key, val);
                        }
                    }
                    return dict;
                });
            return factory;
        }

        private RandomValueViaFunction<object> NewListFactoryFor(Type type)
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

            var factory = new RandomValueViaFunction<Object>((parentType, listPropertyName, listType) =>
                {
                    dynamic list = concreteListCtor.Invoke(new object[] {});
                    var numItems = m_random.IntBetween(MinColSize, MaxColSize + 1);
                    //don't use the list count as we may be using a static key dependening on the user generator
                    //and the lis may only allow uniques
                    for (int i = 0; i < numItems; i++)
                    {
                        dynamic item = InternalNewRandomValueFor(parentType, listPropertyName + ".Value", itemType);
                        list.Add(item);
                    }
                    return list;
                });
            return factory;
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

        private Object NewInstanceFromInterface(Type onInstanceType, String propertyName, Type type)
        {
            Debug("Looking for interface-->instance factory for type {0}", type.PrettyName());

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

        private class RandomValueViaFunction<TPropertyType> : IRandomValueFactory
        {
            private readonly Func<Type,String,Type,TPropertyType> m_createRandomFunc;

            public RandomValueViaFunction(Func<TPropertyType> createRandomFunc)
            {
                m_createRandomFunc = (onType,propertyName,propertyType) => { return createRandomFunc.Invoke(); };
            }

            public RandomValueViaFunction(Func<Type, TPropertyType> createRandomFunc)
            {
                m_createRandomFunc = (onType,propertyName,propertyType) => { return createRandomFunc.Invoke(propertyType); };
            }

            public RandomValueViaFunction(Func<Type, String, Type, TPropertyType> createRandomFunc)
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
        public interface IRandomValueFactory
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

            private readonly Lookup<IRandomValueFactory> m_factories = new Lookup<IRandomValueFactory>();
            private readonly Lookup<Func<Type,Object>> m_interfacesToClassInstances = new Lookup<Func<Type,Object>>();
            private readonly Lookup<int> m_recursionLimit = new Lookup<int>();

            private Func<Type, PropertyInfo, bool> m_propertyFilter = IncludeAllPropertiesFilter;

            public RandomFiller Build()
            {
                return new RandomFiller(
                    m_enableLogging,
                    m_failOnCircularDependency,
                    m_factories,
                    m_interfacesToClassInstances,
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

            public Builder InterfaceToImplementation<TInterface>(Func<Type,TInterface> classInstanceFactory)
                where TInterface:class
            {           
                m_interfacesToClassInstances.SetForType(typeof(TInterface), classInstanceFactory);
                return this;
            }

            public Builder InterfaceToImplementation(Type interfaceType, Func<Type, Object> classInstanceFactory)
            {           
                m_interfacesToClassInstances.SetForType(interfaceType, classInstanceFactory);
                return this;
            }           

            public Builder ValueFactoryForType<TPropertyType>(Func<TPropertyType> factoryFunction)
            {
                var type = typeof (TPropertyType);
                var factory = new RandomValueViaFunction<TPropertyType>(factoryFunction);
                ValueFactoryForType(type, factory); 
                return this; 
            }
            
            public Builder ValueFactoryForNamedPropertyOnType<TInstanceType,TPropertyType>(String propertyName,Func<TPropertyType> func)
            {
                m_factories.SetOnTypeForPropertyName(
                    typeof(TInstanceType),
                    propertyName,
                    new RandomValueViaFunction<TPropertyType>(func));
                return this; 
            }

            public Builder ValueFactoryForTypeOnType<TPropertyType,TInstanceType>(Func<TPropertyType> func)
            {
                m_factories.SetOnTypeForPropertyType(
                    typeof(TInstanceType),
                    typeof(TPropertyType),
                    new RandomValueViaFunction<TPropertyType>(func));
                return this; 
            }

            public Builder ValueFactoryForType(Type type,Func<Object> factoryFunction)
            {          
                m_factories.SetForType(type,new RandomValueViaFunction<Object>(factoryFunction));
                return this;
            }

            public Builder ValueFactoryForType(Type type, IRandomValueFactory factoryFunction)
            {               
                m_factories.SetForType(type,factoryFunction);

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

            [Obsolete("Use ValueFactoryForType instead")]
            public Builder GeneratorForType<TPropertyType>(Func<TPropertyType> func)
            {
                ValueFactoryForType<TPropertyType>(func); 
                return this; 
            }

            [Obsolete("Use ValueFactoryForNamedPropertyOnType instead")]
            public Builder GeneratorForNamedPropertyOnType<TInstanceType,TPropertyType>(String propertyName,Func<TPropertyType> func)
            {
                ValueFactoryForNamedPropertyOnType<TInstanceType,TPropertyType>(propertyName,func);
                return this; 
            }

            [Obsolete("Use ValueFactoryForTypeOnType instead")]
            public Builder GeneratorForTypeOnType<TPropertyType,TInstanceType>(Func<TPropertyType> func)
            {
                ValueFactoryForTypeOnType<TPropertyType,TInstanceType>(func);
                return this; 
            }

            [Obsolete("Use ValueFactoryForType instead")]
            public Builder GeneratorForType(Type type,Func<Object> func)
            {          
                ValueFactoryForType(type,func);
                return this;
            }

            [Obsolete("Use ValueFactoryForType instead")]
            public Builder GeneratorForType(Type type, IRandomValueFactory gen)
            {               
                ValueFactoryForType(type, gen);
                return this;
            }
        }
    }
}