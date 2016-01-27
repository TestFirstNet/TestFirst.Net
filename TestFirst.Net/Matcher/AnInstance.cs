using System;
using TestFirst.Net.Util;

namespace TestFirst.Net.Matcher
{
    public static class AnInstance
    {
        public static IMatcher<object> Any()
        {
            return Any<object>();
        }

        public static IMatcher<T> Any<T>()
        {
            return Matchers.Function((T t) => true, "Any " + typeof(T).FullName);
        }

        public static IMatcher Null()
        {
            return Null<object>();
        }
  
        public static IMatcher<T> Null<T>() where T : class
        {
            return Matchers.Function((T actual) => actual == null, string.Format("a null <{0}>", typeof(T).FullName));
        }

        public static IMatcher<object> NotNull()
        {
            return NotNull<object>();
        }

        public static IMatcher<T> NotNull<T>() 
        {
            return Matchers.Function((T actual) => actual != null, string.Format("a non null <{0}>", typeof(T).FullName));
        }
        
        public static IMatcher<T> EqualTo<T>(T expect)
        {
            if (expect != null) 
            {
                if (expect is string) 
                {
                   return (IMatcher<T>)AString.EqualTo(expect as string);
                }
                Type t = typeof(T);
                if (t.IsPrimitive || t.IsValueType) 
                {
                    return Matchers.Function((T actual) => actual.Equals(expect), typeof(T).Name + " == " + expect);
                }
                if (expect is TimeSpan) 
                {
                    return (IMatcher<T>)ATimeSpan.EqualTo(expect as TimeSpan?);
                }
                if (expect is Uri) 
                {
                    return (IMatcher<T>)AnUri.EqualTo(expect as Uri);
                }
                if (expect is DateTime) 
                {
                    return (IMatcher<T>)ADateTime.EqualTo(expect as DateTime?);
                }
                if (expect is DateTimeOffset) 
                {
                    return (IMatcher<T>)ADateTimeOffset.EqualTo(expect as DateTimeOffset?);
                }
            }

            return EqualTo(
                expect, 
                () =>
                    {
                        if (expect != null)
                            {
                               return "The instance " + expect.GetHashCode() + "@" + typeof(T).FullName + " => " + expect.ToPrettyString();
                            }
                        return "Null instance of type " + typeof(T).FullName;
                    });
        }

        public static IMatcher<T> EqualTo<T>(T expect, Func<string> mismatchMessageFactory)
        {
            return Matchers.Function(
                (T actual) =>
                {
                    if (actual == null && expect == null)
                    {
                        return true;
                    }
                    if (expect == null)
                    {
                        return false;
                    }
                    return expect.Equals(actual);
                },
                mismatchMessageFactory);
        }

        public static IMatcher<T> NotEqualTo<T>(T expect)
        {
            return NotEqualTo(expect, () => expect.ToPrettyString());
        }

        public static IMatcher<T> NotEqualTo<T>(T expect, Func<string> mismatchMessageFactory)
        {
            return Matchers.Function(
                (T actual) =>
                {
                    if (actual == null && expect == null)
                    {
                        return false;
                    }
                    if (expect == null)
                    {
                        return true;
                    }
                    return !expect.Equals(actual);
                },
                mismatchMessageFactory);
        }

        public static IMatcher<object> OfTypeMatching<T>(IMatcher<T> matcher)
        {
            return new TypeMatcher<T>(matcher);
        }

        public static IMatcher<object> OfType<T>()
        {
            return new TypeMatcher<T>();
        }

        public static IMatcher<object> OfType(Type expectType)
        {
            return new TypeMatcher<object>(expectType);
        }

        /// <summary>
        /// Equal by reference
        /// </summary>
        /// <param name="expect">the expected instance</param>
        /// <typeparam name="T">The type of the object to compare against</typeparam>
        /// <returns>A matcher that asserts it is given the exact same instance as expect</returns>
        public static IMatcher<T> SameAs<T>(T expect)
        {
            return Matchers.Function(
                (T actual) =>
                {
                    if (actual == null && expect == null)
                    {
                        return true;
                    }
                    if (expect == null)
                    {
                        return false;
                    }
                    return ReferenceEquals(expect, actual);
                },
                () => expect.ToPrettyString());
        }

        private class TypeMatcher<T> : IMatcher<object>
        {
            private readonly Type m_expectType;
            private readonly IMatcher<T> m_matcher;

            internal TypeMatcher() :
                this(typeof(T), null)
            {
            }

            internal TypeMatcher(IMatcher<T> matcher)
                : this(typeof(T), matcher)
            {
            }
            
            internal TypeMatcher(Type expectType)
                : this(expectType, null)
            {
            }

            internal TypeMatcher(Type expectType, IMatcher<T> matcher)
            {
                m_expectType = expectType;
                m_matcher = matcher;
            }

            /// <summary>
            /// Calls <see cref="Matches(object, IMatchDiagnostics)"/> with a <see cref="NullMatchDiagnostics"/>
            /// </summary>
            /// <param name="actual">The object to match against</param>
            /// <returns>true if the given object matches</returns>
            public bool Matches(object actual)
            {
                return Matches(actual, NullMatchDiagnostics.Instance);
            }

            public bool Matches(object actual, IMatchDiagnostics diag)
            {
                if (actual == null)
                {
                    diag.MisMatched("null instance");
                    return false;
                }

                if (!m_expectType.IsInstanceOfType(actual))
                {
                    diag.MisMatched(diag.NewChild().Text("Wrong type").Value("actualType", actual.GetType().FullName));
                    return false;
                }
                if (m_matcher != null)
                {
                    return diag.TryMatch((T)actual, m_matcher);
                }
                return true;
            }

            public void DescribeTo(IDescription desc)
            {
                desc.Text("not null");
                desc.Child("type", m_expectType.FullName);
                if (m_matcher != null)
                {
                    desc.Child("matching", m_matcher);
                }
            }
        }
    }
}
