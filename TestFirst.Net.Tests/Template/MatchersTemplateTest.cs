using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using System.Text.RegularExpressions;

namespace TestFirst.Net.Template
{
    [TestFixture]
    public class MatchersTemplateTest
    {
        [Test]
        public void MatchersAreCorrectlyGenerated()
        {
            var template = MatchersTemplate.With()
                .Defaults()
                .Build();

            template.GenerateFor<TestDto>();
            template.GenerateFor<TestDto2>();
            template.GenerateFor<TestDtoEnumerable>();

            var actualContent = template.Render();
            //Console.WriteLine("==============================ACTUAL=================================");
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
            var idx1 = dir.LastIndexOf("\\TestFirst.Net.Tests");
            var idx2 = dir.LastIndexOf("\\bin\\");
            if (idx1 > 0 && idx2 > 0 && (idx2 > idx1))
            {
                var path = dir.Substring(0, idx2);
                return new DirectoryInfo(path);
            }
            throw new InvalidOperationException("Can't find the current project dir");
        }
    }
}
