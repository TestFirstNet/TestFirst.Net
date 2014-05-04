using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TestFirst.Net.Examples.Api;

namespace TestFirst.Net.Examples
{
    [TestFixture]
    public class CodeGenerator
    {
        public static void Main(string[] args)
        {
            new CodeGenerator().GenerateCode();
        }
        
        [Test]
        public void Generate()
        {
            GenerateCode();
        }

        private void GenerateCode()
        {
            var template = new TestFirst.Net.Template.MatchersTemplate();

            template.Defaults().Namespace("TestFirst.Net.Examples");
            
            //template.ForPropertyType<Classifiers>()
            //    .AddMatchMethodTaking<String>("$propertyName(AClassifier.EqualTo($argName));")
            //    .AddMatchMethodTaking<Classifiers>("$propertyName(AClassifier.EqualTo($argName));");

            //template.ForPropertyType<NRequire.Model.Version>()
            //    .AddMatchMethodTaking<String>("$propertyName(NRequire.Model.Version.Parse($argName));")
            //    .AddMatchMethodTaking<NRequire.Model.Version>("$propertyName(AnInstance.EqualTo($argName));");

            //template.ForPropertyType<VersionMatcher>()
            //    .AddMatchMethodTaking<String>("$propertyName(Matchers.Function<VersionMatcher>(actual => actual.ToString().Equals($argName), () => $argName));")
            //    .AddMatchMethodTaking<NRequire.Model.Version>("$propertyName($argName.ToString());");

            Console.WriteLine("Starting generation");
            
            template.GenerateForAssembly(typeof(Notification).Assembly, "TestFirst.Net.Examples.Api.*");

            template.RenderToFile("Generated.cs");

        }
    }
}
