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

            template.ForPropertyType<string>()
                .AddMatchMethodTaking<int>("$argName.ToString()");
            
            template.GenerateFor<TestDto>();
            template.GenerateFor<TestDto2>();
            template.GenerateFor<TestDto3>().WithMatcherName("MyCustomName").WithExcludeProperties("MyExcludedProp");
            template.GenerateFor<TestDtoEnumerable>();
            template.GenerateFor<TestIndexedDto>().WithNamespace("MyNamespace");
            template.GenerateFor<TestDtoWithSubClass>();
            template.GenerateFor<TestDtoWithSubClass.SubTestDto>();

            var actualContent = template.Render();
            
            var expectFile = new FileInfo(Path.Combine(GetProjectDir().Parent.FullName, "TestFirst.Net.Test/Template/MatchersTemplateTest.Expect.generated.cs"));
            var expectContent = ReadFile(expectFile);
            
            Assert.AreEqual(Clean(expectContent), Clean(actualContent), "Generated output does not match expected. Actual:\n {0} \n Expected: \n{1}", actualContent, expectContent);
        }

        private static string Clean(string s)
        {
            return Regex.Replace(s, @"\s+", string.Empty).Replace("{", "\n{\n").Replace("}", "\n}\n");
        }

        private static DirectoryInfo GetProjectDir()
        {
            var dir = Directory.GetCurrentDirectory();
            var idx1 = dir.LastIndexOf(Path.DirectorySeparatorChar + "TestFirst.Net.Test");
            var idx2 = dir.LastIndexOf(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar);
            if (idx1 > 0 && idx2 > 0 && (idx2 > idx1))
            {
                var path = dir.Substring(0, idx2);
                return new DirectoryInfo(path);
            }
            throw new InvalidOperationException("Can't find the current project dir");
        }

        private string ReadFile(FileInfo file)
        {
            using (var sr = file.OpenText())
            {
                var content = sr.ReadToEnd();
                return content;
            }
        }
    }
}
