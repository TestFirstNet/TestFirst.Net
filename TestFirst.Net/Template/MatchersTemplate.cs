using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CSharp;

namespace TestFirst.Net.Template 
{
    public interface ITemplate
    {
        /// <summary>
        /// Invoke the template and render all the content. The methods below can only be called in the context of this call
        /// </summary>
        /// <returns>The rendered template</returns>
        string Render();

        void WriteLine();
        void Write(string line, params object[] args);
        void WriteLine(string line, params object[] args);
        void IncrementIndent();
        void DecrementIndent();
    }

    /// <summary>
    /// I generate matcher classes
    /// <para>
    /// Usage:
    /// </para>
    /// <para>
    /// var t = new TestFirst.Net.Template.MatchersTemplate();
    /// </para>
    /// <para>
    /// t.Defaults().WithNamespace("MyNamespace");
    /// </para>
    /// <para>
    /// t.ForPropertyType&lt;MyPropertyType1&gt;
    ///    .AddMatchMethodTaking&lt;String&gt;("MyPropertyType1.Parse($argName)")  ==> will generate  public MyMatcher MyPropertyName(string expect){ MyPropertyName(MyPropertyType1.Parse(expect)); return this;}
    ///    .AddMatchMethodTaking&lt;MyPropertyType1&gt;("AnInstance.Equal($argName)");
    /// </para>
    /// <para>
    /// t.ForPropertyType&lt;MyPropertyType3&gt;
    ///    .AddMatchMethodTaking&lt;string&gt;("new Foo($argName)")
    ///    .AddMatchMethodTaking&lt;object&gt;("AnInstance.Equals($argName)");
    /// </para>
    /// <para>
    /// t.GenerateFor&lt;MyPoco1&gt;(); ==> will generate a matcher for this class
    /// t.GenerateFor&lt;MyPoco2&gt;(); ==> ditto
    /// </para>
    /// <para>
    /// t.Render() => performs the generation, outputs a string
    /// </para>
    /// </summary>
    public class MatchersTemplate : AbstractTemplate 
    {
        private readonly IDictionary<string, string> m_equalMatchersByTypeName = new Dictionary<string, string>();

        private readonly List<TemplateOptions> m_toGenerate = new List<TemplateOptions>();

        private readonly TemplateOptions m_builtinDefaults = new TemplateOptions()
                .WithIncludeParentProps(true)
                .WithExcludeProperties(typeof(IEnumerable<>), "Item")
                .WithExcludeProperties(typeof(IList<>), "Item")
                .WithExcludeProperties(typeof(IDictionary<,>), "Item");

        private readonly TemplateOptions m_defaultOptions = new TemplateOptions();

        public MatchersTemplate()
        {
            m_defaultOptions = m_builtinDefaults; 
            
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

        public void EqualMatcherFor<T>(string equalSnippet)
        {
            EqualMatcherFor(typeof(T).FullName, equalSnippet);
        }

        public void EqualMatcherFor(string fullType, string equalSnippet)
        {
            m_equalMatchersByTypeName[fullType] = equalSnippet;
        }

        public void GenerateForClassesMarkedWithAttributeIn<TAttribute>(Assembly assembly)
            where TAttribute : Attribute
        {
            var types = FindTypesWithAttributeIn<TAttribute>(assembly);
            GenerateFor(types);
        }

        public void GenerateFor(IEnumerable<Type> types)
        {
            foreach (var t in types)
            {
                GenerateFor(t);
            }   
        }

        public void GenerateForAssembly(Assembly assembly, params string[] globPaths)
        {
            var types = assembly.GetTypes();
            var matched = new HashSet<Type>();
            foreach (var glob in globPaths)
            {
                var re = FromAntToRegex(glob);
                foreach (var type in types)
                {
                    if (!type.IsAbstract && re.IsMatch(type.FullName))
                    {
                        matched.Add(type);
                    }
                }
            }

            foreach (var type in matched)
            {
                GenerateFor(type);
            }
        }

        public TemplateOptions GenerateFor<T>()
        {
            return GenerateFor(typeof(T));
        }

        public TemplateOptions GenerateFor(Type objectType)
        {
            var opts = new TemplateOptions();
            opts.MatcherForType = objectType;
            m_toGenerate.Add(opts);
            return opts;
        }
        
        /// <summary>
        /// Render to the given file path, returning true if the generation caused a change to the file. If the file path is relative, then
        /// a search is performed from the current directory up for a *.csproj and this is used as the root directory to calculate the relative path from
        /// </summary>
        /// <param name="path">The path to render the matcher to</param>
        /// <returns>true if the file contents have changed</returns>
        public bool RenderToFile(string path)
        {
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

        /// <summary>
        /// Render generated code to the given file, returning true if the file was modified. If the generation causes no change to the file, returns false
        /// </summary>
        /// <param name="file">The file to render to</param>
        /// <returns>true if the file was modified</returns>
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

            var defaults = m_builtinDefaults.Merge(m_defaultOptions);

            foreach (var opt in m_toGenerate)
            {
                var merged = defaults.Merge(opt);
                var t = new SingleMatcherTemplate(merged, m_equalMatchersByTypeName);
                Write(t.Render());
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

        private static DirectoryInfo FindProjectRootDir()
        {
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

        private IEnumerable<Type> FindTypesWithAttributeIn<TAttribute>(Assembly assembly)
            where TAttribute : Attribute
        {
            return from type in assembly.GetTypes()
                   where type.IsDefined(typeof(TAttribute), false)
                   select type;
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

        private void WriteToFile(string content, FileInfo file)
        {
            file.Directory.Create();

            Console.WriteLine("Writing generated file to:" + file.FullName);

            using (var stream = file.Open(FileMode.Create, FileAccess.Write)) 
            {
                var bytes = Encoding.UTF8.GetBytes(content);
                stream.Write(bytes, 0, bytes.Length);
            }
        }

        private class SingleMatcherTemplate : AbstractTemplate
        {
            private readonly IDictionary<string, string> m_equalMatcherSnippetsByTypeName;
            private readonly IList<PropertyMatchers> m_additionalPropertyMatchers;
            private readonly IDictionary<Type, IList<string>> m_excludeProperties;
            private readonly Type m_objectType;
            private readonly string m_matcherName;
            private readonly string m_namespace;
            private readonly bool m_includeParentProps;

            internal SingleMatcherTemplate(TemplateOptions options, IDictionary<string, string> equalMatcherSnippetsByTypeName)
            {
                m_objectType = options.MatcherForType;
                m_matcherName = options.MatcherName ?? "A" + options.MatcherForType.Name;
                m_namespace = options.Namespace ?? options.MatcherForType.Namespace;
                m_includeParentProps = options.IncludeParentProps ?? true;
                m_excludeProperties = options.ExcludeProperties;
                m_equalMatcherSnippetsByTypeName = equalMatcherSnippetsByTypeName;
                m_additionalPropertyMatchers = options.PropertyMatchers;
            }

            protected override void Generate() 
            {
                // namespace
                WriteLine();
                WriteLine("namespace " + m_namespace + "[[nl]]{");

                // class
                var cleanObjectFullName = m_objectType.ToPrettyTypeName();
                IncrementIndent();
                WriteLine("/// <summary>");
                WriteLine("/// Matcher for a <see cref=\"" + cleanObjectFullName + "\"/>");
                WriteLine("/// </summary>");
                WriteLine("public partial class " + m_matcherName + " : PropertyMatcher<" + cleanObjectFullName + ">[[nl]]{");

                // static property access
                IncrementIndent();
                WriteLine("// provide IDE rename and find reference support");
                WriteLine("private static readonly " + cleanObjectFullName + " PropertyNames = null;");
                
                // With() method
                WriteLine();
                WriteLine("public static " + m_matcherName + " With()[[nl]]{");
                WriteLine(Indent + "return new " + m_matcherName + "();");
                WriteLine("}");

                // Null() method
                WriteLine();
                WriteLine("public static IMatcher<" + cleanObjectFullName + "> Null()[[nl]]{");
                WriteLine(Indent + "return AnInstance.Null<" + cleanObjectFullName + ">();");
                WriteLine("}");

                // NotNull() method
                WriteLine();
                WriteLine("public static IMatcher<" + cleanObjectFullName + "> NotNull()[[nl]]{");
                WriteLine(Indent + "return AnInstance.NotNull<" + cleanObjectFullName + ">();");
                WriteLine("}");

                // Instance() method
                WriteLine();
                WriteLine("public static IMatcher<" + cleanObjectFullName + "> Instance(" + cleanObjectFullName + " expect)[[nl]]{");
                WriteLine(Indent + "return AnInstance.SameAs(expect);");
                WriteLine("}");

                GenerateMethods();

                DecrementIndent();
                WriteLine("}"); // class
                DecrementIndent(); 
                WriteLine("}"); // namespace
            }

            private void GenerateMethods() 
            {
                var bindings = BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance;
                if (!m_includeParentProps)
                {
                    bindings = bindings | BindingFlags.DeclaredOnly;
                }
                var props = m_objectType.GetProperties(bindings).OrderBy(p => p.Name);
                var excludes = m_excludeProperties == null ? null : m_excludeProperties
                        .Where(kv => kv.Key.IsSuperclassOrInterfaceOf(m_objectType))
                        .ToList();

                foreach (var p in props)
                {
                    var pType = p.PropertyType;
                    var pNameLower = p.Name.ToLowerInvariant();
                    var pIsIndexed = p.GetIndexParameters().Length > 0;
                    var skip = pIsIndexed || (excludes != null && excludes.Any(typeToNamesPair => typeToNamesPair.Value.Contains(pNameLower)));
                    if (!skip)
                    {
                        var pCleanTypeName = p.PropertyType.ToPrettyTypeName();

                        // custom user converters. E.g String => Foo, or Foo => FooMatcher
                        if (m_additionalPropertyMatchers != null)
                        {
                            foreach (var template in m_additionalPropertyMatchers)
                            {
                                if (template.Match(p))
                                {
                                    template.Generate(this, p, m_matcherName);
                                }
                            }
                        }
                        var createEquals = m_equalMatcherSnippetsByTypeName.ContainsKey(p.PropertyType.FullName);
                        if (createEquals)
                        {
                            var equalMatcherSnippet = m_equalMatcherSnippetsByTypeName[p.PropertyType.FullName];
                            var code = equalMatcherSnippet.Replace("$argType", pCleanTypeName).Replace("$argName", "expect").Replace("$propertyName", p.Name);

                            WriteLine();
                            WriteLine("public " + m_matcherName + " " + p.Name + "(" + pCleanTypeName + " expect)[[nl]]{");
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
                            WriteLine("public " + m_matcherName + " " + p.Name + "(" + pCleanTypeName + "? expect)[[nl]]{");
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
                            WriteLine("public " + m_matcherName + " " + p.Name + "Null()[[nl]]{");
                            IncrementIndent();
                            switch (pCleanTypeName)
                            {
                                case "bool?":
                                    WriteLine(p.Name + "(ABool.Null());");
                                    break;
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
                                default:
                                    WriteLine(p.Name + "(AnInstance.EqualTo<" + pCleanTypeName + (addNullableMark ? "?" : string.Empty) + ">(null));");
                                    break;
                            }
                            WriteLine("return this;");
                            DecrementIndent();
                            WriteLine("}");
                        }
                        
                        WriteLine();
                        WriteLine("public " + m_matcherName + " " + p.Name + "(IMatcher<" + pCleanTypeName + (addNullableMark ? "?" : string.Empty) + "> matcher)[[nl]]{");
                        IncrementIndent();
                        WriteLine("WithProperty(() => PropertyNames." + p.Name + ", matcher);");
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

        internal TemplateOptions()
        {
            MatcherForType = typeof(object);
        }

        internal Type MatcherForType { get; set; }
        internal string MatcherName { get; private set; }
        internal string Namespace { get; private set; }
        internal bool? IncludeParentProps { get; private set; }
        internal IDictionary<Type, IList<string>> ExcludeProperties { get; private set; }
        internal IList<PropertyMatchers> PropertyMatchers { get; private set; }

        public TemplateOptions WithMatcherName(string val)
        {
            MatcherName = val;
            return this;
        }

        public TemplateOptions WithNamespace(string val)
        {
            Namespace = val;
            return this;
        }

        public TemplateOptions WithIncludeParentProps(bool val)
        {
            IncludeParentProps = val;
            return this;
        }

        public TemplateOptions WithExcludeProperties(params string[] propertyNames)
        {
            WithExcludeProperties(MatcherForType, propertyNames);
            return this;
        }

        public TemplateOptions WithExcludeProperties<T>(params string[] propertyNames)
        {
            WithExcludeProperties(typeof(T), propertyNames);
            return this;
        }

        public TemplateOptions WithExcludeProperties(Type declaredOnType, params string[] propertyNames)
        {
            if (ExcludeProperties == null)
            {
                ExcludeProperties = new Dictionary<Type, IList<string>>();
            }
            IList<string> excludeNames;
            if (!ExcludeProperties.TryGetValue(declaredOnType, out excludeNames))
            {
                excludeNames = new List<string>();
                ExcludeProperties[declaredOnType] = excludeNames;
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
            if (PropertyMatchers == null)
            {
                PropertyMatchers = new List<PropertyMatchers>();
            }
            PropertyMatchers.Add(c);
            return c;
        }

        internal TemplateOptions Merge(TemplateOptions overrides)
        {
            overrides = overrides ?? Empty;

            var merged = new TemplateOptions
            {
                MatcherForType = overrides.MatcherForType ?? MatcherForType,
                MatcherName = overrides.MatcherName ?? MatcherName,
                Namespace = overrides.Namespace ?? Namespace,
                IncludeParentProps = overrides.IncludeParentProps ?? IncludeParentProps,
                ExcludeProperties = Merge(ExcludeProperties, overrides.ExcludeProperties),
                PropertyMatchers = SafeConcat(PropertyMatchers, overrides.PropertyMatchers)
            };
            return merged;
        }

        private static IDictionary<Type, IList<string>> Merge(IDictionary<Type, IList<string>> left, IDictionary<Type, IList<string>> right)
        {
            if (left != null && right != null)
            {
                // merge!
                var merged = new Dictionary<Type, IList<string>>();

                var allTypes = left.Keys.Concat(right.Keys).Distinct();
                foreach (var type in allTypes)
                {
                    IList<string> leftNames;
                    IList<string> rightNames;
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

        private static IList<string> Merge(IList<string> left, IList<string> right)
        {
            if (left != null && right != null)
            {
                // merge!
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
                // merge!
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

    public abstract class AbstractTemplate : ITemplate
    {
        private static string _indent = "    ";
        
        private StringWriter m_w;
        private int m_indentDepth;
        
        public static string Indent
        {
            get
            {
                return _indent;
            }
        }
        public string CurrentIndent { get; private set; }

        public string Render()
        {
            CurrentIndent = string.Empty;
            m_w = new StringWriter();
            Generate();
            var s = m_w.ToString();
            m_w = null;
            return s;
        }

        public void WriteLine()
        {
            m_w.WriteLine();
        }

        public void Write(string line, params object[] args)
        {
            line = ReplaceTokens(line);
            if (args == null || args.Length == 0)
            {
                m_w.Write(line);
            }
            else
            {
                m_w.Write(line, args);
            }
        }

        public void WriteLine(string line, params object[] args)
        {
            line = ReplaceTokens(line);
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

        protected abstract void Generate();

        private string ReplaceTokens(string s)
        {
            return s.Replace("[[nl]]", Environment.NewLine + CurrentIndent).Replace("[[indent]]", CurrentIndent);
        }

        private void SetIndent(int depth)
        {
            CurrentIndent = string.Empty;
            for (var i = 0; i < depth; i++)
            {
                CurrentIndent += Indent;
            }
        }
    }

    public class PropertyMatchers
    {
        private readonly List<CustomMatchMethodTemplate> m_methods = new List<CustomMatchMethodTemplate>();
        private Type m_propertyType;

        internal PropertyMatchers()
        {
        }

        public PropertyMatchers AddMatchMethodTaking<TMethodArgType>(string code)
        {
            m_methods.Add(new CustomMatchMethodTemplate { ArgType = typeof(TMethodArgType), Code = code });
            return this;
        }

        internal PropertyMatchers ForPropertyType<TPropertyType>()
        {
            m_propertyType = typeof(TPropertyType);
            return this;
        }

        internal bool Match(PropertyInfo p)
        {
            return m_propertyType.IsSuperclassOrInterfaceOf(p.PropertyType);
        }

        internal void Generate(ITemplate t, PropertyInfo p, string matcherName)
        {
            foreach (var m in m_methods)
            {
                m.Generate(t, p, matcherName);
            }
        }

        private class CustomMatchMethodTemplate
        {
            internal Type ArgType { get; set; }
            internal string Code { get; set; }

            internal void Generate(ITemplate t, PropertyInfo p, string matcherName)
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

    internal static class TypeExtensions 
    {
        private static readonly CSharpCodeProvider Compiler = new CSharpCodeProvider();

        public static string ToPrettyTypeName(this Type t)
        {
            var name = _ToPrettyTypeName(t);

            return name;
        }

        public static string _ToPrettyTypeName(this Type t)
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
                return GetRawTypeName(t) + "<" + string.Join(",", t.GetGenericArguments().Select(ToPrettyTypeName)) + ">";
            }
            if (t.IsArray)
            {
                return ToPrettyTypeName(t.GetElementType()) + "[]";
            }

            return Compiler.GetTypeOutput(new CodeTypeReference(t));
        }

        internal static bool IsEnumerableType(Type type)
        {
            if (!type.IsGenericType)
            {
                return false;
            }
            if (typeof(string) == type)
            {
                // implements IEnumerable<char>
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

        private static string GetRawTypeName(Type t)
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
