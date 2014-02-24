using System;
using System.Text;

namespace TestFirst.Net.Matcher
{
    public static class AByteArray
    {
        public static IMatcher<byte[]> Null()
        {
            return Matchers.Function((byte[] actual) =>actual == null,"a null byte array");
        }

        public static IMatcher<byte[]> NotNull()
        {
            return Matchers.Function((byte[] actual) =>actual != null,"a non null byte array");
        }

        public static IMatcher<byte[]> NotEmpty()
        {
            return Matchers.Function((byte[] actual) =>actual != null && actual.Length > 0,"a non null or empty byte array");
        }

        public static IMatcher<byte[]> EqualTo(byte[] expect)
        {
            if (expect == null)
            {
                return AnInstance.Null();
            }
            return new ByteArrayMatcher(expect);
        }

        private class ByteArrayMatcher : AbstractMatcher<byte[]>
        {
            private readonly byte[] m_expect;

            internal ByteArrayMatcher(byte[] expect) : base(false)
            {
                m_expect = expect;
            }

            public static IMatcher<byte[]> EqualTo(byte[] expect)
            {
                if (expect == null)
                {
                    return AnInstance.Null();
                }
                return new ByteArrayMatcher(expect);
            }

            public override bool Matches(byte[] actual, IMatchDiagnostics diag)
            {    
                for (int i = 0; i < m_expect.Length ||  i < actual.Length; i++)
                {
                    if (i >= m_expect.Length)
                    {
                        diag.MisMatched("mismatched at byte[{0}], expected end of array, but got [{1}],\nlengths {2},\nexpect end\nbut got bytes\n{3}"
                            , i
                            , actual[i]
                            , LengthDiffMessage(actual, m_expect)
                            , GetSegmentWithOffsetAndLength(actual, i, 30)
                        );
                        return false;
                    }
                    else if (i >= actual.Length)
                    {
                        diag.MisMatched("mismatched at byte[{0}], expected [{1}], but got end, of array\nlengths {2},\nexpect bytes\n{3}\nbut got end of array"
                            , i
                            , m_expect[i]
                            , LengthDiffMessage(actual, m_expect)
                            , GetSegmentWithOffsetAndLength(m_expect, i, 30)
                        );
                        return false;
                    }
                    else
                    {
                        if (actual[i] != m_expect[i])
                        {
                            diag.MisMatched("mismatched at byte[{0}], expected {1}, but got [{2}], lengths {3},\nexpect bytes\n{4}\nbut got bytes\n{5}"
                                , i
                                , m_expect[i]
                                , actual[i]
                                , LengthDiffMessage(actual, m_expect)
                                , GetSegmentWithOffsetAndLength(m_expect, i, 30)
                                , GetSegmentWithOffsetAndLength(actual, i, 30)
                            );
                            return false;
                        }
                    }
                }
                return true;
            }

            private static string LengthDiffMessage(byte[] actual, byte[] expect){
                if( actual.Length == expect.Length){
                    return "match";
                }
                return "differ, actual length = " + actual.Length + ", expected " + expect.Length;
            }
        }
        
        public static IMatcher<byte[]> AsUtf8(IMatcher<String> matcher)
        {                 
            return As(BytesToUtf8, matcher);
        }

        public static IMatcher<byte[]> StartsWith(byte[] startsWith)
        {                 
            return Matchers.Function((byte[] actual,IMatchDiagnostics diag) =>
                {
                    if (actual == null || actual.Length < startsWith.Length)
                    {
                        return false;
                    }
                    for (int i = 0; i < startsWith.Length; i++)
                    {
                        if (actual[i] != startsWith[i])
                        {
                            diag.MisMatched("mismatched at byte[{0}], expected {1}, but got [{2}],\nexpect bytes\n{3}\nbut got bytes\n{4}"
                                , i
                                , startsWith[i]
                                , actual[i]
                                , GetSegmentWithOffsetAndLength(startsWith, i, 30)
                                , GetSegmentWithOffsetAndLength(actual, i, 30)
                            );
                            return false;
                        }
                    }
                    return true;
                },
            "a non null or empty byte array");
        }

        private static String GetSegmentWithOffsetAndLength(byte[] bytes,int offset, int segLen)
        {
            int from = offset - (segLen/2);
            if(from < 0)
            {
                from = 0;
            }
            int to = from + segLen;
            if(to > bytes.Length)
            {
                to = bytes.Length;
            }

            var seg = new byte[to-from];
            Array.Copy(bytes,from,seg,0,seg.Length);

            var s = "pos " + from + "->";
            
            s += "[";
            if (from > 0)
            {
                s += "...";
            }
            s += String.Join(",", seg);
            if (to < bytes.Length)
            {
                s += "...";
            }

            s += "]<-pos ";
            s += to;
            return s;
        }

        private static String BytesToUtf8(byte[] bytes)
        {
            try
            {
                return new UTF8Encoding().GetString(bytes);
            }
            catch (Exception e)
            {                
                throw new ArgumentException("Error converting bytes to utf8 string", e);
            }            
        }

        public static IMatcher<byte[]> As<T>(Func<byte[], T> transformFunc, IMatcher<T> matcher)
        {
            return Matchers.Function((byte[] actual, IMatchDiagnostics diag) =>
                {
                    if (actual == null)
                    {
                        return false;
                    }
                    T converted = transformFunc.Invoke(actual);
                    return diag.TryMatch(converted, matcher);
                },
                "a non null byte array which is converted to Utf8 and matches " + matcher
            );
        }

    }
}
