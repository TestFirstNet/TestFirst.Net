using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace TestFirst.Net.Test
{
    [TestFixture]
    public class DescriptionTest
    {
        private static readonly string Indent = Description.Indent;

        [Test]
        public void TextTest()
        {
            var actual = new Description()
                .Text("Line 1")
                .Text("Line 2")
                .Value("Val")
                .ToString();

            var expect = new StringBuilder();
            expect.AppendLine("Line 1");
            expect.AppendLine("Line 2");
            expect.AppendLine("Val");

            AssertEqual(expect.ToString(), actual);
        }
        
        [Test]
        public void ValueTest()
        {
            var actual = new Description()
                .Text("Line 1")
                .Text("Line 2")
                .Value("value1")
                .Value("value2")
                .Value("labelA", "valueA")
                .Value("labelB", "valueB")
                .ToString();

            var expect = new StringBuilder();
            expect.AppendLine("Line 1");
            expect.AppendLine("Line 2");
            expect.AppendLine("value1");
            expect.AppendLine("value2");
            expect.AppendLine("labelA:valueA");
            expect.AppendLine("labelB:valueB");

            AssertEqual(expect.ToString(), actual);
        }

        [Test]
        public void ValueSelfDescribingTest()
        {
            var actual = new Description()
                .Text("Line 1")
                .Value(new MySelfDescriber(desc =>
                {
                    desc.Text("Self1.A");
                    desc.Text("Self1.B");
                    desc.Text("Self1.C");
                }))
                .Value("label", "value")
                .Value(
                    "Self2", 
                    new MySelfDescriber(desc =>
                        {
                            desc.Text("Self2.A");
                            desc.Text("Self2.B");
                            desc.Text("Self2.C");
                        }))
                    .ToString();

            var expect = new StringBuilder();
            expect.AppendLine("Line 1");
            expect.AppendLine("Self1.A");
            expect.AppendLine("Self1.B");
            expect.AppendLine("Self1.C");
            expect.AppendLine("label:value");
            expect.AppendLine("Self2:");
            expect.AppendLine("Self2.A");
            expect.AppendLine("Self2.B");
            expect.AppendLine("Self2.C");

            AssertEqual(expect.ToString(), actual);
        }

        [Test]
        public void ChildTest()
        {
            var actual = new Description()
                .Text("Line 1")
                .Child(
                    "Child1", 
                    new MySelfDescriber(desc =>
                        {
                            desc.Text("Child1.A");
                            desc.Text("Child1.B");
                        }))
                .Child(
                    "Child2", 
                    new MySelfDescriber(desc =>
                        {
                            desc.Text("Child2.A");
                            desc.Text("Child2.B");
                        }))
                .ToString();

            var expect = new StringBuilder();
            expect.AppendLine("Line 1");
            expect.AppendLine("Child1:");
            expect.AppendLine(Indent + "Child1.A");
            expect.AppendLine(Indent + "Child1.B");
            expect.AppendLine("Child2:");
            expect.AppendLine(Indent + "Child2.A");
            expect.AppendLine(Indent + "Child2.B");
            AssertEqual(expect.ToString(), actual);
        }

        [Test]
        public void NullChildWithLabelTest()
        {
            var actual = new Description()
                .Text("Line 1")
                .Child("Child1", null)
                .ToString();

            var expect = new StringBuilder();
            expect.AppendLine("Line 1");
            expect.AppendLine("Child1:");
            expect.AppendLine(Indent + "null");

            AssertEqual(expect.ToString(), actual);
        }

        [Test]
        public void NullChildTest()
        {
            var actual = new Description()
                .Text("Line 1")
                .Child(null)
                .ToString();

            var expect = new StringBuilder();
            expect.AppendLine("Line 1");
            expect.AppendLine(Indent + "null");

            AssertEqual(expect.ToString(), actual);
        }

        [Test]
        public void ChildWithNewLinesTest()
        {
            var actual = new Description()
                .Text("Line 1")
                .Child(
                    "Child1", 
                    new MySelfDescriber(desc =>
                        {
                            desc.Text("Child1.A\nChild1.B");
                        }))
                .ToString();

            var expect = new StringBuilder();
            expect.AppendLine("Line 1");
            expect.AppendLine("Child1:");
            expect.AppendLine(Indent + "Child1.A");
            expect.AppendLine(Indent + "Child1.B");
            AssertEqual(expect.ToString(), actual);
        }

        [Test]
        public void ChildrenTest()
        {
            var children = new List<ISelfDescribing>
            {
                new MySelfDescriber(desc =>
                {
                    desc.Text("ChildValueA.1");
                    desc.Text("ChildValueA.2");
                }),
                new MySelfDescriber(desc =>
                {
                    desc.Text("ChildValueB.1");
                    desc.Text("ChildValueB.2");
                }),

                new MySelfDescriber(desc =>
                {
                    desc.Text("ChildValueC.1");
                    desc.Text("ChildValueC.2");
                })
            };
            var actual = new Description()
                .Text("Line 1")
                .Children("MyChildren", children)
                .ToString();

            var expect = new StringBuilder();
            expect.AppendLine("Line 1");
            expect.AppendLine("MyChildren:");
            expect.AppendLine(Indent + "ChildValueA.1");
            expect.AppendLine(Indent + "ChildValueA.2");
            expect.AppendLine(Indent + "ChildValueB.1");
            expect.AppendLine(Indent + "ChildValueB.2");
            expect.AppendLine(Indent + "ChildValueC.1");
            expect.AppendLine(Indent + "ChildValueC.2");

            AssertEqual(expect.ToString(), actual);
        }

        [Test]
        public void ChildrenChildrenTest()
        {
            var children = new List<ISelfDescribing>
            {
                new MySelfDescriber((IDescription desc) =>
                {
                    desc.Text("Child1.A");
                    desc.Text("Child1.B");
                }),
                new MySelfDescriber((IDescription desc) =>
                {
                    desc.Text("Child2.A");
                    desc.Text("Child2.B");

                    var children2 = new List<ISelfDescribing>
                    {
                            new MySelfDescriber((IDescription desc2) =>
                                {
                                    desc2.Text("Child2.1.A");
                                    desc2.Text("Child2.1.B");
                                }),
                            new MySelfDescriber((IDescription desc2) =>
                                {
                                    desc2.Text("Child2.2.A");
                                    desc2.Text("Child2.2.B");
                                })
                        };

                    // no label
                    desc.Children(children2);
                })
            };
            var actual = new Description()
                .Text("Line 1")

                // label
                .Children("MyChildren", children)
                .ToString();

            var expect = new StringBuilder();
            expect.AppendLine("Line 1");
            expect.AppendLine("MyChildren:");
            expect.AppendLine(Indent + "Child1.A");
            expect.AppendLine(Indent + "Child1.B");
            expect.AppendLine(Indent + "Child2.A");
            expect.AppendLine(Indent + "Child2.B");
            expect.AppendLine(Indent + Indent + "Child2.1.A");
            expect.AppendLine(Indent + Indent + "Child2.1.B");
            expect.AppendLine(Indent + Indent + "Child2.2.A");
            expect.AppendLine(Indent + Indent + "Child2.2.B");

            AssertEqual(expect.ToString(), actual);
        }

        [Test]
        public void ChildrenChildTest()
        {
            var children = new List<ISelfDescribing>
            {
                new MySelfDescriber(desc =>
                {
                    desc.Text("Child1.A");
                    desc.Text("Child1.B");
                }),
                new MySelfDescriber(desc =>
                {
                    desc.Text("Child2.A");
                    desc.Text("Child2.B");

                    desc.Child(
                        "Child3", 
                        new MySelfDescriber(desc2 =>
                            {
                                desc2.Text("Child3.A");
                                desc2.Text("Child3.B");
                            }));
                    desc.Child(
                        "Child4", 
                        new MySelfDescriber(desc2 =>
                            {
                                desc2.Text("Child4.A");
                                desc2.Text("Child4.B");
                            }));
                    desc.Child(new MySelfDescriber(desc2 =>
                    {
                        desc2.Text("Child5.A");
                        desc2.Text("Child5.B");
                    }));
                })
            };
            var actual = new Description()
                .Text("Line 1")

                // label
                .Children("MyChildren", children)
                .ToString();

            var expect = new StringBuilder();
            expect.AppendLine("Line 1");
            expect.AppendLine("MyChildren:");
            expect.AppendLine(Indent + "Child1.A");
            expect.AppendLine(Indent + "Child1.B");
            expect.AppendLine(Indent + "Child2.A");
            expect.AppendLine(Indent + "Child2.B");
            expect.AppendLine(Indent + "Child3:");
            expect.AppendLine(Indent + Indent + "Child3.A");
            expect.AppendLine(Indent + Indent + "Child3.B");
            expect.AppendLine(Indent + "Child4:");
            expect.AppendLine(Indent + Indent + "Child4.A");
            expect.AppendLine(Indent + Indent + "Child4.B");
            expect.AppendLine(Indent + Indent + "Child5.A");
            expect.AppendLine(Indent + Indent + "Child5.B");
            AssertEqual(expect.ToString(), actual);
        }
        [Test]
        public void ChildChildTest()
        {
            var child = new MySelfDescriber((IDescription desc1) =>
            {
                desc1.Text("Child1.A");
                desc1.Text("Child1.B");

                var child2 = new MySelfDescriber((IDescription desc2) =>
                {
                    desc2.Text("Child2.A");
                    desc2.Text("Child2.B");
                    desc2.Text("Child2.C");

                    var child3 = new MySelfDescriber((IDescription desc3) =>
                    {
                        desc3.Text("Child3.A");
                        desc3.Text("Child3.B");
                        desc3.Text("Child3.C");
                    });

                    desc2.Child("Child3", child3);
                });

                desc1.Child("Child2", child2);
            });
            var actual = new Description()
                .Text("My Line 1")
                .Child("Child1", child)
                .ToString();

            var expect = new StringBuilder();
            expect.AppendLine("My Line 1");
            expect.AppendLine("Child1:");
            expect.AppendLine(Indent + "Child1.A");
            expect.AppendLine(Indent + "Child1.B");
            expect.AppendLine(Indent + "Child2:");
            expect.AppendLine(Indent + Indent + "Child2.A");
            expect.AppendLine(Indent + Indent + "Child2.B");
            expect.AppendLine(Indent + Indent + "Child2.C");
            expect.AppendLine(Indent + Indent + "Child3:");
            expect.AppendLine(Indent + Indent + Indent + "Child3.A");
            expect.AppendLine(Indent + Indent + Indent + "Child3.B");
            expect.AppendLine(Indent + Indent + Indent + "Child3.C");

            AssertEqual(expect.ToString(), actual);
        }

        private void AssertEqual(string expect, string actual)
        {
            Assert.AreEqual(expect.Trim(), actual);
        }
    }

    internal class MySelfDescriber : ISelfDescribing
    {
        private readonly Action<IDescription> m_action;

        public MySelfDescriber(Action<IDescription> action)
        {
            m_action = action;
        }

        public void DescribeTo(IDescription desc)
        {
            m_action.Invoke(desc);
        }
    }
}
