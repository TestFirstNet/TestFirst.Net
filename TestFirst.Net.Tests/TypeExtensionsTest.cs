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
            Assert.IsTrue(typeof(IList<String>).IsSuperclassOrInterfaceOf(typeof(List<String>)));
            Assert.IsTrue(typeof(List<String>).IsSuperclassOrInterfaceOf(typeof(List<String>)));
            Assert.IsTrue(typeof(IList<String>).IsSuperclassOrInterfaceOf(typeof(List<String>)));
            Assert.IsTrue(typeof(IList<>).IsSuperclassOrInterfaceOf(typeof(List<String>)));
            Assert.IsTrue(typeof(IDictionary<,>).IsSuperclassOrInterfaceOf(typeof(Dictionary<String, int>)));
            Assert.IsTrue(typeof(IDictionary<,>).IsSuperclassOrInterfaceOf(typeof(Dictionary<,>)));
            Assert.IsTrue(typeof(IDictionary<,>).IsSuperclassOrInterfaceOf(typeof(IDictionary<,>)));
            Assert.IsTrue(typeof(IDictionary<,>).IsSuperclassOrInterfaceOf(typeof(IDictionary<String, int>)));
            Assert.IsTrue(typeof(IDictionary<,>).IsSuperclassOrInterfaceOf(typeof(Dictionary<String, int>)));

            Assert.IsFalse(typeof(IList<String>).IsSuperclassOrInterfaceOf(typeof(List<int>)));
            Assert.IsFalse(typeof(IList<String>).IsSuperclassOrInterfaceOf(typeof(List<>)));
            Assert.IsFalse(typeof(IDictionary<String, String>).IsSuperclassOrInterfaceOf(typeof(Dictionary<String, int>)));
            Assert.IsFalse(typeof(IDictionary<String, String>).IsSuperclassOrInterfaceOf(typeof(Dictionary<,>)));

        }
    }
}
