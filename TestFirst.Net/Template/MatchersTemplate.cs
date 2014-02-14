using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using TestFirst.Net.Lang;
using Microsoft.CSharp;
using System.CodeDom;

namespace TestFirst.Net.Template 
{
    /// <summary>
    /// I generate matcher classes
    /// </summary>
    public class MatchersTemplate {

        private static readonly CSharpCodeProvider Compiler = new CSharpCodeProvider();
        
        private readonly IDictionary<String, String> m_equalMatchersByTypeName;
        private StringBuilder m_buff = new StringBuilder();
        private ISet<String> m_imports = new SortedSet<String>();

        public static Builder With() {
            return new Builder();
        }

        private MatchersTemplate(IDictionary<String, String> equalMatchers)
        {
            m_equalMatchersByTypeName = new Dictionary<String, String>(equalMatchers);
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

        public void GenerateFor<T>(String matcherName) 
        {
            GenerateFor(typeof(T), matcherName);
        }

        public void GenerateFor<T>() {
            GenerateFor(typeof(T));
        }

        public void GenerateFor(Type t)
        {
            GenerateFor(t, "A" + t.Name);
        }

        public void GenerateFor(Type t, String matcherName) {
            var builder = new MatcherBuilder(t, matcherName, m_equalMatchersByTypeName);
            m_buff.Append(builder.Generate(m_imports));
        }

        public void RenderTo(FileInfo file) {
            WriteToFile(Render(), file);
        }

        public String Render()
        {
            var content = new StringBuilder();
            foreach (var import in m_imports)
            {
                content.AppendLine(import);
            }
            content.AppendLine(m_buff.ToString());
            return content.ToString();
        }

        private void WriteToFile(String content, FileInfo file) {
            file.Directory.Create();

            Console.WriteLine("Writing generated file to:" + file.FullName);

            using (var stream = file.Open(FileMode.Create, FileAccess.Write)) {
                var bytes = System.Text.Encoding.UTF8.GetBytes(content);
                stream.Write(bytes, 0, bytes.Length);
            }
        }

        class MatcherBuilder {
            private StringWriter m_w;
            private IList<String> m_imports = new List<String>();
            private IList<String> m_methods = new List<String>();

            private IDictionary<String, String> m_equalMatcherSnippetsByTypeName;

            private String MatcherType { get;set; }
            private Type m_type;
            private const String Indent = "    ";

            private String m_indent = "";
            private int m_indentDepth = 0;

            public MatcherBuilder(Type type, String matcherName, IDictionary<String, String> equalMatchersByType)
            {
                m_type = type;
                MatcherType = matcherName;

                m_imports.Add("System");
                m_imports.Add("System.Collections.Generic");
                m_imports.Add("System.Linq");
                m_imports.Add("System.Text");
                m_imports.Add("TestFirst.Net");
                m_imports.Add("TestFirst.Net.Matcher");
                
                m_equalMatcherSnippetsByTypeName = equalMatchersByType;
            }

            public String Generate(ISet<String> imports) {
                m_w = new StringWriter();
                //imports
                foreach (var import in m_imports) {
                    imports.Add("using " + import + ";");
                }

                //namespace
                WriteLine();
                WriteLine("namespace TestFirst.Net.Matcher {");
                
                //class
                WriteLine();
                IncrementIndent();
                WriteLine("public partial class " + MatcherType + " : PropertyMatcher<" + m_type.FullName + ">{");

                IncrementIndent();
                //static property access
                WriteLine();
                WriteLine("// provide IDE rename and find reference support");
                WriteLine("private static readonly " + m_type.FullName + " PropertyNames = null;");
                WriteLine();
                
                //With() method
                WriteLine();
                WriteLine("public static " + MatcherType + " With(){");
                WriteLine(m_indent + "return new " + MatcherType + "();");
                WriteLine("}");
                
                //custom methods
                foreach (var method in m_methods) {
                    WriteLine();
                    WriteLine(method);
                }
                GenerateMethods();

                DecrementIndent();
                WriteLine("}");//class
                DecrementIndent(); 
                WriteLine("}");//namespace

                return m_w.ToString();
            }

            private void GenerateMethods() {
                var props = m_type
                        .GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                        .OrderBy(p => p.Name);

                foreach (var p in props) {
                    var pType = p.PropertyType;
                    var cleanName = ToPrettyType(p.PropertyType);
                    if (m_equalMatcherSnippetsByTypeName.ContainsKey(p.PropertyType.FullName)) {
                        var equalMatcherSnippet = m_equalMatcherSnippetsByTypeName[p.PropertyType.FullName];
                        
                        WriteLine();
                        WriteLine("public " + MatcherType + " " + p.Name + "(" + cleanName + " val) {");
                        IncrementIndent();
                        WriteLine(p.Name + "(" + equalMatcherSnippet + "(val));");
                        WriteLine("return this;");
                        DecrementIndent();
                        WriteLine("}");
                    }
                    WriteLine();
                    var addNullable = pType.IsValueType && Nullable.GetUnderlyingType(pType) == null;
                    WriteLine("public " + MatcherType + " " + p.Name + "(IMatcher<" + cleanName + (addNullable?"?":"") + "> matcher) {");
                    IncrementIndent();
                    WriteLine("WithProperty(()=>PropertyNames."+ p.Name + ",matcher);");
                    WriteLine("return this;");
                    DecrementIndent();
                    WriteLine("}");
                }

                
            }

            private static String ToPrettyType(Type t) {
                var underlyingType = Nullable.GetUnderlyingType(t);
                if (underlyingType != null)
                {
                    return ToPrettyType(underlyingType) + "?";
                }
                if (IsEnumerableType(t))
                {
                    if (t.GetGenericArguments().Length == 1)
                    {
                        return "System.Collections.Generic.IEnumerable<" + ToPrettyType(t.GetGenericArguments()[0]) + ">";
                    }
                    return GetRawTypeName(t) + "<" + String.Join(",", t.GetGenericArguments().Select(ToPrettyType)) + ">";
                }
                if (t.IsArray)
                {
                    return ToPrettyType(t.GetElementType()) + "[]";
                }

                return Compiler.GetTypeOutput(new CodeTypeReference(t));
            }

            private static String GetRawTypeName(Type t)
            {
                var name = t.FullName;
                var idx = name.IndexOf('`');
                if(idx !=-1){
                    return name.Substring(0,idx);
                }
                return name;
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

            private void WriteLine() {
                m_w.WriteLine();
            }
            
            private void Write(String line, params Object[] args) {
                if (args == null || args.Length == 0) {
                    m_w.Write(line);
                } else {
                    m_w.Write(line, args);
                }
            }
            
            private void WriteLine(String line, params Object[] args){
                m_w.Write(m_indent);
                if (args == null || args.Length == 0) {
                    m_w.WriteLine(line);
                } else {
                    m_w.WriteLine(line, args);       
                }
            }

            private void IncrementIndent() {
                SetIndent(++m_indentDepth);
            }

            private void DecrementIndent() {
                SetIndent(--m_indentDepth);
            }

            private void SetIndent(int depth) {
                m_indent = "";
                for (var i = 0; i < depth; i++) {
                    m_indent += Indent;
                }
            }

        }

        public class Builder {
            private Dictionary<String, String> m_equalMatcherSnippets = new Dictionary<string, string>();
            
            public MatchersTemplate Build() {
   
                return new MatchersTemplate(m_equalMatcherSnippets);
            }

            public Builder Defaults()
            {
                EqualMatcherFor<string>("AString.EqualTo");
                EqualMatcherFor<bool>("ABool.EqualTo");
                EqualMatcherFor<bool?>("ABool.EqualTo");
                EqualMatcherFor<short>("AShort.EqualTo");
                EqualMatcherFor<short?>("AShort.EqualTo");
                EqualMatcherFor<int>("AnInt.EqualTo");
                EqualMatcherFor<int?>("AnInt.EqualTo");
                EqualMatcherFor<float>("AFloat.EqualTo");
                EqualMatcherFor<float?>("AFloat.EqualTo");
                EqualMatcherFor<long>("ALong.EqualTo");
                EqualMatcherFor<long?>("ALong.EqualTo");
                EqualMatcherFor<double>("ADouble.EqualTo");
                EqualMatcherFor<double?>("ADouble.EqualTo");
                EqualMatcherFor<decimal>("ADecimal.EqualTo");
                EqualMatcherFor<decimal?>("ADecimal.EqualTo");
                EqualMatcherFor<DateTime>("ADateTime.EqualTo");
                EqualMatcherFor<DateTime?>("ADateTime.EqualTo");
                EqualMatcherFor<Guid>("AGuid.EqualTo");
                EqualMatcherFor<Guid?>("AGuid.EqualTo");
                EqualMatcherFor<FileInfo>("AFileInfo.EqualTo");
                EqualMatcherFor<Uri>("AnUri.EqualTo");

                return this;
            }
            public Builder EqualMatcherFor<T>(String equalSnippet) {
                EqualMatcherFor(typeof(T).FullName,equalSnippet);
                return this;
            }

            public Builder EqualMatcherFor(String fullType,String equalSnippet) {
                m_equalMatcherSnippets[fullType] = equalSnippet;
                return this;
            }
        }
    }
}
