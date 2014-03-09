using System;
using System.Text;

namespace TestFirst.Net.Matcher
{
    public static class AByteArray
    {
        public static IMatcher<byte[]> Null()
        {
            return AnArray.Null<byte>();
        }

        public static IMatcher<byte[]> NotNull()
        {
            return AnArray.NotNull<byte>();
        }

        public static IMatcher<byte[]> NotEmpty()
        {
            return AnArray.NotEmpty<byte>();
        }

        public static IMatcher<byte[]> EqualTo(byte[] expect)
        {
            return AnArray.EqualTo(expect);
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
