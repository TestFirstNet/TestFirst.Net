using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using TestFirst.Net.Lang;
using Microsoft.CSharp;
using System.CodeDom;
using System.Text.RegularExpressions;

namespace TestFirst.Net.Template 
{
    /// <summary>
    /// I generate matcher classes
    /// 
    /// Usage:
    /// 
    /// var t = new TestFirst.Net.Template.MatchersTemplate();
    ///
    /// t.Defaults().Namespace("MyNamespace");
    ///
    /// t.ForPropertyType<MyPropertyType1>
    ///    .AddMatchMethodTaking<String>("MyPropertyType1.Parse($argName)")  ==> will generate  public MyMatcher MyPropertyName(String expect){ MyPropertyName(MyPropertyType1.Parse(expect)); return this;}
    ///    .AddMatchMethodTaking<MyPropertyType1>("AnInstance.Equal($argName)");
    ///
    /// t.ForPropertyType<MyPropertyType3>
    ///    .AddMatchMethodTaking<String>("new Foo($argName)")
    ///    .AddMatchMethodTaking<Object>("AnInstance.Equals($argName)");
    ///
    /// 
    /// t.GenerateFor<MyPoco1>(); ==> will geenrate a matcher for this class
    /// t.GenerateFor<MyPoco2>(); ==> ditto
    /// 
    /// t.Render() => performs the generation, outputs a string
    /// </summary>
    public class MatchersTemplate : AbstractTemplate {

        private readonly IDictionary<String, String> m_equalMatchersByTypeName = new Dictionary<String, String>();

        private readonly List<TemplateOptions> m_toGenerate = new List<TemplateOptions>();

        private readonly TemplateOptions BuiltinDefaults = new TemplateOptions()
                .IncludeParentProps(true)
                .ExcludeProperties(typeof(IEnumerable<>), "Item")
                .ExcludeProperties(typeof(IList<>), "Item")
                .ExcludeProperties(typeof(IDictionary<,>), "Item");

        private TemplateOptions m_defaultOptions = new TemplateOptions();

        public MatchersTemplate()
        {
            m_defaultOptions = BuiltinDefaults; 
            
            EqualMatcherFor<string>("AString.EqualTo($argName)");
            EqualMatcherFor<bool>("ABool.EqualTo($argName)");
            EqualMatcherFor<bool?>("ABool.EqualTo($argName)");
            EqualMatcherFor<short>("AShort.EqualTo($argName)");
            EqualMatcherFor<short?>("AShort.EqualTo($argName)");
            EqualMatcherFor<int>("AnInt.EqualTo($argName)");
            EqualMatcherFor<int?>("AnInt.EqualTo($argName)");
            EqualMatcherFor<float>("AFloat.EqualTo($argName)");
            EqualMatcherFor<float?>("AFloat.EqualTo($argName)");
            EqualMatcherFor<long>("ALong.EqualTo($argName)");
            EqualMatcherFor<long?>("ALong.EqualTo($argName)");
            EqualMatcherFor<double>("ADouble.EqualTo($argName)");
            EqualMatcherFor<double?>("ADouble.EqualTo($argName)");
            EqualMatcherFor<decimal>("ADecimal.EqualTo($argName)");
            EqualMatcherFor<decimal?>("ADecimal.EqualTo($argName)");
            EqualMatcherFor<DateTime>("ADateTime.EqualTo($argName)");
            EqualMatcherFor<DateTime?>("ADateTime.EqualTo($argName)");
            EqualMatcherFor<Guid>("AGuid.EqualTo($argName)");
            EqualMatcherFor<Guid?>("AGuid.EqualTo($argName)");
            EqualMatcherFor<FileInfo>("AFileInfo.EqualTo($argName)");
            EqualMatcherFor<Uri>("AnUri.EqualTo($argName)");
        }

        public PropertyMatchers ForPropertyType<TFrom>()
        {
            return Defaults().ForPropertyType<TFrom>();
        }
        
        public TemplateOptions Defaults()
        {
            return m_defaultOptions;
        }

        public void EqualMatcherFor<T>(String equalSnippet)
        {
            EqualMatcherFor(typeof(T).FullName, equalSnippet);
        }

        public void EqualMatcherFor(String fullType, String equalSnippet)
        {
            m_equalMatchersByTypeName[fullType] = equalSnippet;
        }

        public void GenerateForClassesMarkedWithAttributeIn<TAttribute>(Assembly assembly)
            where TAttribute : System.Attribute
        {
            var types = FindTypesWithAttributeIn<TAttribute>(assembly);
            GenerateFor(types);
        }

        private IEnumerable<Type> FindTypesWithAttributeIn<TAttribute>(Assembly assembly)
            where TAttribute : System.Attribute
        {
            return from type in assembly.GetTypes()
                   where type.IsDefined(typeof(TAttribute), false)
                   select type;
        }

        public void GenerateFor(IEnumerable<Type> types)
        {
            foreach (var t in types){
                GenerateFor(t);
            }   
        }

        public void GenerateForAssembly(Assembly assembly, params string[] globPaths)
        {
            var types = assembly.GetTypes();
            var matched = new HashSet<Type>();
            foreach(var glob in globPaths){
                var re = FromAntToRegex(glob);
                foreach (var type in types)
                {
                    //Console.WriteLine(type.FullName);
                    if (!type.IsAbstract && re.IsMatch(type.FullName))
                    {
                        //Console.WriteLine("MATCHED:" + type.FullName);
                        matched.Add(type);
                    }
                }
            }

            foreach (var type in matched)
            {
                GenerateFor(type);
            }

        }

        private static Regex FromAntToRegex(string antExpression)
        {
            var sb = new StringBuilder();
            foreach (var c in antExpression)
            {
                if (c == '.')
                {
                    sb.Append("\\.");
                }
                else if (c == '*')
                {
                    sb.Append(".*");
                }
                else if (c == '?')
                {
                    sb.Append(".{1}");
                }
                else
                {
                    sb.Append(c);
                }
            }
            return new Regex(sb.ToString(), RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        }
        public TemplateOptions GenerateFor<T>()
        {
            return GenerateFor(typeof(T));
        }

        public TemplateOptions GenerateFor(Type objectType)
        {
            var opts = new TemplateOptions();
            opts._MatcherForType = objectType;
            m_toGenerate.Add(opts);
            return opts;
        }
        
        /// <summary>
        /// Render to the given file path, returning true if the generation caused a change to the file. If the file path is relative, then
        /// a search is performed from the current directory up for a *.csproj and this is used as the root directory to calculate the relative path from
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool RenderToFile(string path){
            FileInfo file;
            if (Path.IsPathRooted(path))
            {
                file = new FileInfo(path);
            }
            else
            {
                file = new FileInfo(Path.Combine(FindProjectRootDir().FullName, path));
            }
            return RenderToFile(file);
        }

        private static DirectoryInfo FindProjectRootDir(){
            var current = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (current != null)
            {
                var files = current.GetFiles("*.csproj");
                if (files.Length > 0)
                {
                    return current;
                }
                current = current.Parent;
            }
            throw new FileNotFoundException("Could not find project root directory (walking up looking for a *.csproj file). You may need to specify an absolute file path");
        }

        /// <summary>
        /// Render generated code to the given file, returning true if the file was modified. If the generation causes no change t the file, returns false
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool RenderToFile(FileInfo file)
        {
            return WriteToFileIfChanged(Render(), file);
        }

        protected override void Generate()
        {
            WriteLine("using System;");
            WriteLine("using System.Collections.Generic;");
            WriteLine("using System.Linq;");
            WriteLine("using System.Text;");
            WriteLine("using TestFirst.Net;");
            WriteLine("using TestFirst.Net.Matcher;");

            var defaults = BuiltinDefaults.Merge(m_defaultOptions);

            foreach (var opt in m_toGenerate)
            {
                var merged = defaults.Merge(opt);
                var t = new SingleMatcherTemplate(merged,m_equalMatchersByTypeName);
                Write(t.Render());
            }
        }

        private bool WriteToFileIfChanged(string content, FileInfo file)
        {
            if (file.Exists)
            {
                using (var stream = file.OpenText())
                {
                    var existingContent = stream.ReadToEnd();
                    if (existingContent.Equals(content))
                    {
                        Console.WriteLine("File content the same for path " + file.FullName + ", not writing");
                        return false;
                    }
                }
            }
            WriteToFile(content, file);
            return true;
        }
        private void WriteToFile(String content, FileInfo file) {
            file.Directory.Create();

            Console.WriteLine("Writing generated file to:" + file.FullName);

            using (var stream = file.Open(FileMode.Create, FileAccess.Write)) {
                var bytes = System.Text.Encoding.UTF8.GetBytes(content);
                stream.Write(bytes, 0, bytes.Length);
            }
        }
        
        internal class SingleMatcherTemplate : AbstractTemplate
        {
            private IList<String> m_imports = new List<String>();

            internal IDictionary<String, String> EqualMatcherSnippetsByTypeName;
            internal IList<PropertyMatchers> AdditionalPropertyMatchers { get; set; }

            internal IDictionary<Type, IList<String>> ExcludeProperties { get;set;}

            internal Type ObjectType { get; set; }
            internal String MatcherName { get; set; }
            internal string Namespace { get; set; }
            internal bool IncludeParentProps { get; set; }
            internal IList<String> CustomMethodsSourceCode { get; set; }

            internal SingleMatcherTemplate(TemplateOptions options, IDictionary<String, String> equalMatcherSnippetsByTypeName)
            {
                ObjectType = options._MatcherForType;
                MatcherName = options._MatcherName ?? "A" + options._MatcherForType.Name;
                Namespace = options._Namespace ?? options._MatcherForType.Namespace;
                IncludeParentProps = options._IncludeParentProps ?? true;
                ExcludeProperties = options._ExcludeProperties;
                EqualMatcherSnippetsByTypeName = equalMatcherSnippetsByTypeName;
                AdditionalPropertyMatchers = options._PropertyMatchers;
            }

            protected override void Generate() 
            {
                //namespace
                WriteLine();
                WriteLine("namespace " + Namespace + " {");

                var cleanObjectFullName = ObjectType.ToPrettyTypeName();
                //class
                WriteLine();
                IncrementIndent();
                WriteLine("public partial class " + MatcherName + " : PropertyMatcher<" + cleanObjectFullName  + ">{");

                IncrementIndent();
                //static property access
                WriteLine();
                WriteLine("// provide IDE rename and find reference support");
                WriteLine("private static readonly " + cleanObjectFullName + " PropertyNames = null;");
                WriteLine();
                
                //With() method
                WriteLine();
                WriteLine("public static " + MatcherName + " With(){");
                WriteLine(CurrentIndent + "return new " + MatcherName + "();");
                WriteLine("}");

                //Null() method
                WriteLine();
                WriteLine("public static IMatcher<" + cleanObjectFullName + "> Null(){");
                WriteLine(CurrentIndent + "return AnInstance.Null<" + cleanObjectFullName + ">();");
                WriteLine("}");

                //NotNull() method
                WriteLine();
                WriteLine("public static IMatcher<" + cleanObjectFullName + "> NotNull(){");
                WriteLine(CurrentIndent + "return AnInstance.NotNull<" + cleanObjectFullName + ">();");
                WriteLine("}");

                //Instance() method
                WriteLine();
                WriteLine("public static IMatcher<" + cleanObjectFullName + "> Instance(" + cleanObjectFullName  + " expect){");
                WriteLine(CurrentIndent + "return AnInstance.SameAs(expect);");
                WriteLine("}");

                if (CustomMethodsSourceCode != null)
                {
                    foreach (var methodSource in CustomMethodsSourceCode)
                    {
                        var code = methodSource
                            .Replace("$namespace", Namespace)
                            .Replace("$objectType", ObjectType.ToPrettyTypeName())
                            .Replace("$matcherName", MatcherName);
                        WriteLine();
                        WriteLine(code);
                    }
                    WriteLine();
                }
                    
                GenerateMethods();

                DecrementIndent();
                WriteLine("}");//class
                DecrementIndent(); 
                WriteLine("}");//namespace
            }

            private void GenerateMethods() 
            {
                var bindings = BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance;
                if( !IncludeParentProps ){
                    bindings = bindings | BindingFlags.DeclaredOnly;
                }
                var props = ObjectType.GetProperties(bindings).OrderBy(p => p.Name);
                var excludes = ExcludeProperties ==null?null:ExcludeProperties
                        .Where(kv=>kv.Key.IsSuperclassOrInterfaceOf(ObjectType))
                        .ToList();

                foreach (var p in props) {
                    var pType = p.PropertyType;
                    var pNameLower = p.Name.ToLowerInvariant();
                    var pIsIndexed = p.GetIndexParameters().Length > 0;
                    var skip = pIsIndexed || (excludes != null && excludes.Any(typeToNamesPair => typeToNamesPair.Value.Contains(pNameLower)));
                    if (!skip)
                    {
                        var pCleanTypeName = p.PropertyType.ToPrettyTypeName();
                        //custom user converters. E.g String=>Foo, or Foo=>FooMatcher
                        if (AdditionalPropertyMatchers != null)
                        {
                            foreach (var template in AdditionalPropertyMatchers)
                            {
                                if (template.Match(p))
                                {
                                    template.Generate(this, p, MatcherName);
                                }
                            }
                        }
                        var createEquals = EqualMatcherSnippetsByTypeName.ContainsKey(p.PropertyType.FullName);
                        if (createEquals)
                        {
                            var equalMatcherSnippet = EqualMatcherSnippetsByTypeName[p.PropertyType.FullName];
                            var code = equalMatcherSnippet.Replace("$argType", pCleanTypeName).Replace("$argName", "expect").Replace("$propertyName", p.Name);

                            WriteLine();
                            WriteLine("public " + MatcherName + " " + p.Name + "(" + pCleanTypeName + " expect) {");
                            IncrementIndent();
                            if (code.EndsWith(";"))
                            {
                                WriteLine(code);
                            }
                            else
                            {
                                WriteLine(p.Name + "(" + code + ");");
                            }
                            WriteLine("return this;");
                            DecrementIndent();
                            WriteLine("}");
                        }
                        else if (pType.IsEnum)
                        {
                            WriteLine();
                            WriteLine("public " + MatcherName + " " + p.Name + "(" + pCleanTypeName + "? expect) {");
                            IncrementIndent();
                            WriteLine(p.Name + "(AnInstance.EqualTo(expect));");
                            WriteLine("return this;");
                            DecrementIndent();
                            WriteLine("}");
                        }
                        var isNullable = !pType.IsValueType || Nullable.GetUnderlyingType(pType) != null;
                        var addNullableMark = pType.IsValueType && Nullable.GetUnderlyingType(pType) == null;

                        if (isNullable)
                        {
                            WriteLine();
                            WriteLine("public " + MatcherName + " " + p.Name + "Null() {");
                            IncrementIndent();
                            switch (pCleanTypeName)
                            {
                                case "bool?":
                                    WriteLine(p.Name + "(ABool.Null());");
                                    break;
                                //case "byte?":
                                //    WriteLine(p.Name + "(AByte.Null());");
                                //    break;
                                case "decimal?":
                                    WriteLine(p.Name + "(ADecimal.Null());");
                                    break;
                                case "double?":
                                    WriteLine(p.Name + "(ADouble.Null());");
                                    break;
                                case "float?":
                                    WriteLine(p.Name + "(AFloat.Null());");
                                    break;
                                case "System.Guid?":
                                    WriteLine(p.Name + "(AGuid.Null());");
                                    break;
                                case "int?":
                                    WriteLine(p.Name + "(AnInt.Null());");
                                    break;
                                case "long?":
                                    WriteLine(p.Name + "(ALong.Null());");
                                    break;
                                case "short?":
                                    WriteLine(p.Name + "(AShort.Null());");
                                    break;
                                case "string":
                                    WriteLine(p.Name + "(AString.Null());");
                                    break;
                                default :
                                    WriteLine(p.Name + "(AnInstance.EqualTo<" + pCleanTypeName + (addNullableMark ? "?" : "") + ">(null));");
                                    break;
                            }
                            WriteLine("return this;");
                            DecrementIndent();
                            WriteLine("}");
                        }
                        
                        WriteLine(); 
                        WriteLine("public " + MatcherName + " " + p.Name + "(IMatcher<" + pCleanTypeName + (addNullableMark ? "?" : "") + "> matcher) {");
                        IncrementIndent();
                        WriteLine("WithProperty(()=>PropertyNames." + p.Name + ",matcher);");
                        WriteLine("return this;");
                        DecrementIndent();
                        WriteLine("}");

                    }
                }   
            }
        }

    }

    public class TemplateOptions
    {
        private static readonly TemplateOptions Empty = new TemplateOptions();

        internal Type _MatcherForType;
        internal String _MatcherName;
        internal String _Namespace;
        internal bool? _IncludeParentProps;
        internal IDictionary<Type, IList<String>> _ExcludeProperties;
        internal IList<PropertyMatchers> _PropertyMatchers;

        internal TemplateOptions()
        {
            _MatcherForType = typeof(Object);
        }

        internal TemplateOptions Merge(TemplateOptions overrides)
        {
            overrides = overrides ?? Empty;

            var merged = new TemplateOptions()
            {
                _MatcherForType = overrides._MatcherForType?? _MatcherForType,
                _MatcherName = overrides._MatcherName ?? _MatcherName,
                _Namespace = overrides._Namespace ?? _Namespace,
                _IncludeParentProps = overrides._IncludeParentProps ?? _IncludeParentProps,
                _ExcludeProperties = Merge(_ExcludeProperties, overrides._ExcludeProperties),
                _PropertyMatchers = SafeConcat(_PropertyMatchers, overrides._PropertyMatchers)
            };
            return merged;
        }

        public TemplateOptions MatcherName(String val)
        {
            _MatcherName = val;
            return this;
        }

        public TemplateOptions Namespace(String val)
        {
            _Namespace = val;
            return this;
        }

        public TemplateOptions IncludeParentProps(bool val)
        {
            _IncludeParentProps = val;
            return this;
        }

        public TemplateOptions ExcludeProperties(params string[] propertyNames)
        {
            ExcludeProperties(_MatcherForType, propertyNames);
            return this;
        }

        public TemplateOptions ExcludeProperties<T>(params string[] propertyNames)
        {
            ExcludeProperties(typeof(T), propertyNames);
            return this;
        }
        public TemplateOptions ExcludeProperties(Type declaredOnType, params string[] propertyNames)
        {
            if (_ExcludeProperties == null)
            {
                _ExcludeProperties = new Dictionary<Type, IList<String>>();
            }
            IList<string> excludeNames;
            if (!_ExcludeProperties.TryGetValue(declaredOnType, out excludeNames))
            {
                excludeNames = new List<String>();
                _ExcludeProperties[declaredOnType] = excludeNames;
            }
            foreach (var name in propertyNames)
            {
                var nameLower = name.ToLowerInvariant();
                if (!excludeNames.Contains(nameLower))
                {
                    excludeNames.Add(nameLower);
                }
            }
            return this;
        }

        public PropertyMatchers ForPropertyType<TFrom>()
        {
            var c = new PropertyMatchers().ForPropertyType<TFrom>();
            if (_PropertyMatchers == null)
            {
                _PropertyMatchers = new List<PropertyMatchers>();
            }
            _PropertyMatchers.Add(c);
            return c;
        }

        private static IDictionary<Type, IList<String>> Merge(IDictionary<Type, IList<String>> left, IDictionary<Type, IList<String>> right)
        {
            if (left != null && right != null)
            {
                //merge!
                var merged = new Dictionary<Type, IList<String>>();

                var allTypes = left.Keys.Concat(right.Keys).Distinct();
                foreach (var type in allTypes)
                {
                    IList<String> leftNames;
                    IList<String> rightNames;
                    left.TryGetValue(type, out leftNames);
                    right.TryGetValue(type, out rightNames);

                    merged[type] = Merge(leftNames, rightNames);
                }
                return merged;
            }

            if (left != null)
            {
                return left;
            }
            if (right != null)
            {
                return right;
            }
            return null;
        }

        private static IList<String> Merge(IList<String> left, IList<String> right)
        {
            if (left != null && right != null)
            {
                //merge!
                return left.Concat(right).Distinct().ToList();
            }

            if (left != null)
            {
                return left;
            }
            if (right != null)
            {
                return right;
            }
            return null;
        }

        private static IList<T> SafeConcat<T>(IList<T> left, IList<T> right)
        {
            if (left != null && right != null)
            {
                //merge!
                return left.Concat(right).Distinct().ToList();
            }

            if (left != null)
            {
                return left;
            }
            if (right != null)
            {
                return right;
            }
            return null;
        }
    }

    public class PropertyMatchers
    {
        private Type m_propertyType;
        private List<CustomMatchMethodTemplate> m_methods = new List<CustomMatchMethodTemplate>();

        internal PropertyMatchers()
        { }

        internal PropertyMatchers ForPropertyType<TPropertyType>()
        {
            m_propertyType = typeof(TPropertyType);
            return this;
        }

        internal bool Match(PropertyInfo p)
        {
            return m_propertyType.IsSuperclassOrInterfaceOf(p.PropertyType);
        }

        public PropertyMatchers AddMatchMethodTaking<TMethodArgType>(String code)
        {
            m_methods.Add(new CustomMatchMethodTemplate { ArgType = typeof(TMethodArgType), Code = code });
            return this;
        }

        internal void Generate(ITemplate t, PropertyInfo p, String matcherName)
        {
            foreach (var m in m_methods)
            {
                m.Generate(t, p, matcherName);
            }
        }

        private class CustomMatchMethodTemplate
        {
            internal Type ArgType { get; set; }
            internal String Code { get; set; }

            internal void Generate(ITemplate t, PropertyInfo p, String matcherName)
            {
                var argCleanTypeName = ArgType.ToPrettyTypeName(); 
                t.WriteLine();
                t.WriteLine("public " + matcherName + " " + p.Name + "(" + argCleanTypeName + " expect) {");
                t.IncrementIndent();

                var code = Code.Replace("$argType", argCleanTypeName).Replace("$argName", "expect").Replace("$propertyName", p.Name);
                if (Code.EndsWith(";"))
                {
                    t.WriteLine(code);
                }
                else
                {
                    t.WriteLine(p.Name + "(" + code + ");");
                }
                t.WriteLine("return this;");
                t.DecrementIndent();
                t.WriteLine("}");
            }
        }
    }


    public interface ITemplate
    {
        /// <summary>
        /// Invoke the template and render all the content. The methods below can only be called in the context of this call
        /// </summary>
        /// <returns></returns>
        String Render();

        void WriteLine();
        void Write(String line, params Object[] args);
        void WriteLine(String line, params Object[] args);
        void IncrementIndent();
        void DecrementIndent();
    }

    public abstract class AbstractTemplate : ITemplate
    {
        private StringWriter m_w;
        private const String Indent = "    ";
        public String CurrentIndent { get; private set; }
        private int m_indentDepth = 0;

        public String Render()
        {
            CurrentIndent = "";
            m_w = new StringWriter();
            Generate();
            var s = m_w.ToString();
            m_w = null;
            return s;
        }

        protected abstract void Generate();

        public void WriteLine()
        {
            m_w.WriteLine();
        }

        public void Write(String line, params Object[] args)
        {
            if (args == null || args.Length == 0)
            {
                m_w.Write(line);
            }
            else
            {
                m_w.Write(line, args);
            }
        }

        public void WriteLine(String line, params Object[] args)
        {
            m_w.Write(CurrentIndent);
            if (args == null || args.Length == 0)
            {
                m_w.WriteLine(line);
            }
            else
            {
                m_w.WriteLine(line, args);
            }
        }

        public void IncrementIndent()
        {
            SetIndent(++m_indentDepth);
        }

        public void DecrementIndent()
        {
            SetIndent(--m_indentDepth);
        }

        private void SetIndent(int depth)
        {
            CurrentIndent = "";
            for (var i = 0; i < depth; i++)
            {
                CurrentIndent += Indent;
            }
        }

    }
    internal static class TypeExtensions 
    {
        private static readonly CSharpCodeProvider Compiler = new CSharpCodeProvider();

        public static String ToPrettyTypeName(this Type t)
        {
            var name = _ToPrettyTypeName(t);

            return name;
        }
        public static String _ToPrettyTypeName(this Type t)
        {
            var underlyingType = Nullable.GetUnderlyingType(t);
            if (underlyingType != null)
            {
                return ToPrettyTypeName(underlyingType) + "?";
            }
            if (IsEnumerableType(t))
            {
                if (t.GetGenericArguments().Length == 1)
                {
                    return "System.Collections.Generic.IEnumerable<" + ToPrettyTypeName(t.GetGenericArguments()[0]) + ">";
                }
                return GetRawTypeName(t) + "<" + String.Join(",", t.GetGenericArguments().Select(ToPrettyTypeName)) + ">";
            }
            if (t.IsArray)
            {
                return ToPrettyTypeName(t.GetElementType()) + "[]";
            }

            return Compiler.GetTypeOutput(new CodeTypeReference(t));
        }

        static bool IsEnumerableType(Type type)
        {
            if (!type.IsGenericType)
            {
                return false;
            }
            if (typeof(string) == type)//implements IEnumerable<char>
            {
                return false;
            }
            foreach (Type intType in type.GetInterfaces())
            {
                if (intType.IsGenericType && intType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    return true;
                }
            }
            return false;
        }

        private static String GetRawTypeName(Type t)
        {
            var name = t.FullName;
            var idx = name.IndexOf('`');
            if (idx != -1)
            {
                return name.Substring(0, idx);
            }
            return name;
        }
    }
}
