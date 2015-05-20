using System;
using System.Text;

namespace TestFirst.Net.Matcher
{
    public static class AnArray
    {
        public static IMatcher<T[]> EqualTo<T>(T[] expect)
        {
            if (expect == null)
            {
                return Null<T>();
            }
            return new ArrayMatcher<T>(expect);
        }

        public static IMatcher<T[]> EqualTo<T>(T[] expect, Func<T, T, bool> equalMatcher, Func<T,object> toPrettyValConverter)
        {
            if (expect == null)
            {
                return Null<T>();
            }
            return new ArrayMatcher<T>(expect, equalMatcher, toPrettyValConverter);
        }

        public static IMatcher<T[]> Null<T>()
        {
            return Matchers.Function((T[] actual) => actual == null, "a null array of " + typeof(T).Name);
        }

        public static IMatcher<T[]> NotNull<T>()
        {
            return Matchers.Function((T[] actual) => actual != null, "a non null array of " + typeof(T).Name);
        }

        public static IMatcher<T[]> NotEmpty<T>()
        {
            return Matchers.Function((T[] actual) =>actual != null && actual.Length > 0,"a non null or empty array of " + typeof(T).Name);
        }

        public static IMatcher<T[]> EmptyOrNull<T>()
        {
            return Matchers.Function((T[] actual) =>actual == null || actual.Length == 0,"a null or empty array of " + typeof(T).Name);
        }

        public static IMatcher<T[]> Empty<T>()
        {
            return Matchers.Function((T[] actual) =>actual != null && actual.Length == 0,"a non null empty array of " + typeof(T).Name);
        }

        private class ArrayMatcher<T> : AbstractMatcher<T[]>
        {
            private static readonly Func<T, T, bool> DefaultObjectEqualsMatcher = new Func<T, T, bool>((expect, actual) => { return expect.Equals(actual); });
            private static readonly Func<T, object> DefaultPrettyValueConverter = new Func<T, object>(val=>val);
            
            private readonly T[] m_expect;
            private readonly Func<T, T, bool> m_equalMatcher;
            private readonly Func<T, object> m_toPrettyValConverter;

            internal ArrayMatcher(T[] expect)
                : this(expect, DefaultObjectEqualsMatcher, DefaultPrettyValueConverter)
            {}

            internal ArrayMatcher(T[] expect, Func<T, T, bool> equalMatcher, Func<T, object> toPrettyValConverter)
                : base(false)
            {
                m_expect = expect;
                m_equalMatcher = equalMatcher;
                m_toPrettyValConverter = toPrettyValConverter;
            }

            public override bool Matches(T[] actual, IMatchDiagnostics diag)
            {
                for (int i = 0; i < m_expect.Length || i < actual.Length; i++)
                {
                    if (i >= m_expect.Length)
                    {
                        diag.MisMatched("mismatched at [{0}], expected end of array, but got [{1}],\nlengths {2},\nexpect end\nbut got\n{3}"
                            , i
                            , ToPrettyVal(actual[i])
                            , LengthDiffMessage(actual, m_expect)
                            , GetSegmentWithOffsetAndLength(actual, i, 30)
                        );
                        return false;
                    }
                    if (i >= actual.Length)
                    {
                        diag.MisMatched("mismatched at [{0}], expected [{1}], but got end, of array\nlengths {2},\nexpect \n{3}\nbut got end of array"
                            , i
                            , ToPrettyVal(m_expect[i])
                            , LengthDiffMessage(actual, m_expect)
                            , GetSegmentWithOffsetAndLength(m_expect, i, 30)
                        );
                        return false;
                    }
                    if(!m_equalMatcher.Invoke(m_expect[i],actual[i]))
                    {
                        diag.MisMatched("mismatched at [{0}], expected {1}, but got [{2}], lengths {3},\nexpect\n{4}\nbut got\n{5}"
                            , i
                            , ToPrettyVal(m_expect[i])
                            , ToPrettyVal(actual[i])
                            , LengthDiffMessage(actual, m_expect)
                            , GetSegmentWithOffsetAndLength(m_expect, i, 30)
                            , GetSegmentWithOffsetAndLength(actual, i, 30)
                        );
                        return false;
                    }
                }
                return true;
            }

            private static string LengthDiffMessage(T[] actual, T[] expect)
            {
                if (actual.Length == expect.Length)
                {
                    return "match";
                }
                return "differ, actual length " + actual.Length + ", expected " + expect.Length;
            }

            private Object ToPrettyVal(T obj){
                if (obj != null && m_toPrettyValConverter != null) {
                    return m_toPrettyValConverter.Invoke (obj);
                }
                return obj;
            }
        }


        private static string GetSegmentWithOffsetAndLength<T>(T[] data, int offset, int segLen,Func<T, object> toPrettyValConverter = null)
        {
            int from = offset - (segLen / 2);
            if (from < 0)
            {
                from = 0;
            }
            int to = from + segLen;
            if (to > data.Length)
            {
                to = data.Length;
            }
            var s = new StringBuilder("index " + from + "->");
            s.Append("[");
            if (from > 0)
            {
                s.Append("...");
            }
            for (var i = from; i < to; i++) {
                if (i > from) {
                    s.Append (',');
                }
                if (toPrettyValConverter != null) {
                    s.Append(toPrettyValConverter.Invoke(data[i]));
                } else {
                    s.Append(data[i]);
                }
            }
            if (to < data.Length)
            {
                s.Append("...");
            }
            s.Append("]<-index ");
            s.Append(to);
            return s.ToString();
        }
    }
}
