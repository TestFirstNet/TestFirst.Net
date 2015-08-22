using System;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace TestFirst.Net.Template
{
    [TestFixture]
    public class MatchersTemplateTest
    {
        [Test]
        public void MatchersAreCorrectlyGenerated()
        {
            var template = new MatchersTemplate();

            template.ForPropertyType<String>()
                .AddMatchMethodTaking<int>("$argName.ToString()");
            
            template.GenerateFor<TestDto>();
            template.GenerateFor<TestDto2>();
            template.GenerateFor<TestDto3>().MatcherName("MyCustomName").ExcludeProperties("MyExcludedProp");
            template.GenerateFor<TestDtoEnumerable>();
            template.GenerateFor<TestIndexedDto>().Namespace("MyNamespace");
            template.GenerateFor<TestDtoWithSubClass>();
            template.GenerateFor<TestDtoWithSubClass.SubTestDto>();

            var actualContent = template.Render();
            Console.WriteLine("==============================ACTUAL=================================");
            Console.WriteLine(actualContent); 
            
            var expectFile = new FileInfo(Path.Combine(GetProjectDir().Parent.FullName, "TestFirst.Net.Tests/Template/MatchersTemplateTest.ExpectGenerated.cs"));
            var expectContent = ReadFile(expectFile);
            //Console.WriteLine("==============================EXPECT=================================");
            //Console.WriteLine(expectContent);

            Assert.AreEqual(Clean(expectContent), Clean(actualContent), "Generated output does not match expected");
        }

        private static String Clean(String s)
        {
            return Regex.Replace(s, @"\s+", String.Empty).Replace("{", "\n{\n").Replace("}", "\n}\n");
        }
        private String ReadFile(FileInfo file)
        {
            using (var sr = file.OpenText())
            {
                var content = sr.ReadToEnd();
                return content;
            }
        }

        private static DirectoryInfo GetProjectDir()
        {
            var dir = Directory.GetCurrentDirectory();
            var idx1 = dir.LastIndexOf(Path.DirectorySeparatorChar  + "TestFirst.Net.Tests");
            var idx2 = dir.LastIndexOf(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar);
            if (idx1 > 0 && idx2 > 0 && (idx2 > idx1))
            {
                var path = dir.Substring(0, idx2);
                return new DirectoryInfo(path);
            }
            throw new InvalidOperationException("Can't find the current project dir");
        }
    }
}
