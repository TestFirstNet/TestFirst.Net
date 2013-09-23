using System;
using TestFirst.Net.Util;

namespace TestFirst.Net.Matcher
{
    public static class AnInstance
    {
        public static IMatcher<Object> Any()
        {
            return Any<Object>();
        }

        public static IMatcher<T> Any<T>()
        {
            return Matchers.Function((T t) => true, "Any " + typeof(T).FullName);
        }

        public static IMatcher<Object> Null()
        {
            return Null<Object>();
        }
  
        public static IMatcher<T> Null<T>() where T : class
        {
            return EqualTo((T)null);
        }

        public static IMatcher<Object> NotNull()
        {
            return NotNull<Object>();
        }

        public static IMatcher<T> NotNull<T>()// where T : class
        {
            return Matchers.Function((T actual) => actual != null, String.Format("a non null <{0}>", typeof(T).FullName));
        }
        
        public static IMatcher<T> EqualTo<T>(T expect)
        {
            return EqualTo(expect, () =>
                {
                    if (expect != null)
                    {
                       return "The instance " + expect.GetHashCode() + "@" + typeof (T).FullName + " => " + expect.ToPrettyString();
                    }
                    else
                    {
                        return "Null instance of type " +  typeof (T).FullName;
                    }
                });
        }

        public static IMatcher<T> EqualTo<T>(T expect,Func<String> mismatchMessageFactory)
        {
            return Matchers.Function((T actual) =>
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

        public static IMatcher<T> NotEqualTo<T>(T expect, Func<String> mismatchMessageFactory)
        {
            return Matchers.Function((T actual) =>
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

        public static IMatcher<Object> OfTypeMatching<T>(IMatcher<T> matcher)
        {
            return new TypeMatcher<T>(matcher);
        }

        public static IMatcher<Object> OfType<T>()
        {
            return new TypeMatcher<T>();
        }

        public static IMatcher<Object> OfType(Type expectType)
        {
            return new TypeMatcher<Object>(expectType);
        }

        /// <summary>
        /// Equal by reference
        /// </summary>
        public static IMatcher<T> SameAs<T>(T expect)
        {
            return Matchers.Function((T actual) =>
                {
                    if (actual == null && expect == null)
                    {
                        return true;
                    }
                    if (expect == null)
                    {
                        return false;
                    }
                    return Object.ReferenceEquals(expect,actual);
                },
                ()=>expect.ToPrettyString());
        }

        private class TypeMatcher<T>:IMatcher<Object>
        {
            private readonly Type m_expectType;
            private readonly IMatcher<T> m_matcher;

            internal TypeMatcher():this(typeof(T),null)
            {}

            internal TypeMatcher(IMatcher<T> matcher):this(typeof(T),matcher)
            {}
            
            internal TypeMatcher(Type expectType):this(expectType,null)
            {}

            internal TypeMatcher(Type expectType,IMatcher<T> matcher)
            {
                m_expectType = expectType;
                m_matcher = matcher;
            }

            /// <summary>
            /// Calls <see cref="Matches(object,IMatchDiagnostics)"/> with a <see cref="NullMatchDiagnostics"/>
            /// </summary>
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

                if(!m_expectType.IsInstanceOfType(actual) )
                {
                    diag.MisMatched(diag.NewChild().Text("Wrong type").Value("actualType", actual.GetType().FullName));
                    return false;
                }
                if (m_matcher != null)
                {
                    return diag.TryMatch((T) actual, m_matcher);
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
