using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace TestFirst.Net
{
    [TestFixture]
    public class TypeExtensionsTest
    {
        [Test]
        public void IsSuperclassOf()
        {
            Assert.IsTrue(typeof(IList<string>).IsSuperclassOrInterfaceOf(typeof(List<string>)));
            Assert.IsTrue(typeof(List<string>).IsSuperclassOrInterfaceOf(typeof(List<string>)));
            Assert.IsTrue(typeof(IList<string>).IsSuperclassOrInterfaceOf(typeof(List<string>)));
            Assert.IsTrue(typeof(IList<>).IsSuperclassOrInterfaceOf(typeof(List<string>)));
            Assert.IsTrue(typeof(IDictionary<,>).IsSuperclassOrInterfaceOf(typeof(Dictionary<string, int>)));
            Assert.IsTrue(typeof(IDictionary<,>).IsSuperclassOrInterfaceOf(typeof(Dictionary<,>)));
            Assert.IsTrue(typeof(IDictionary<,>).IsSuperclassOrInterfaceOf(typeof(IDictionary<,>)));
            Assert.IsTrue(typeof(IDictionary<,>).IsSuperclassOrInterfaceOf(typeof(IDictionary<string, int>)));
            Assert.IsTrue(typeof(IDictionary<,>).IsSuperclassOrInterfaceOf(typeof(Dictionary<string, int>)));

            Assert.IsFalse(typeof(IList<string>).IsSuperclassOrInterfaceOf(typeof(List<int>)));
            Assert.IsFalse(typeof(IList<string>).IsSuperclassOrInterfaceOf(typeof(List<>)));
            Assert.IsFalse(typeof(IDictionary<string, string>).IsSuperclassOrInterfaceOf(typeof(Dictionary<string, int>)));
            Assert.IsFalse(typeof(IDictionary<string, string>).IsSuperclassOrInterfaceOf(typeof(Dictionary<,>)));
        }
    }
}
