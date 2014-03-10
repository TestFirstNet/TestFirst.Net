using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace TestFirst.Net.Rand
{
    //[NotThreadSafe]
    public class Random
    {
        private readonly System.Random m_random = new System.Random(Environment.TickCount);

        private readonly Utf8CharProvider m_utf8CharProvider = new Utf8CharProvider();

        private const int DefaultMinStringLen = 1;
        private const int DefaultMaxStringLen = 20;

        private int m_count = 0;
        private int m_charCount = 32;

        private static long MinDateTimeTicks = new DateTime(1, 1, 1, 0, 0, 0, 0).Ticks;
        private static long MaxDateTimeTicks = new DateTime(2200, 1, 1, 0, 0, 0, 0).Ticks;

        private const String AlphaNumeric = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ01234567890";

        public static readonly Random Instance = new Random();

        public T EnumOf<T>() where T:struct,IConvertible
        {
            return (T)EnumOf(typeof(T));
        }

        public Object EnumOf(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException(String.Format("Expect an enumerated type but got {0}", enumType.FullName));
            }
            Array values = Enum.GetValues(enumType);
            var randomIdx = Int(0, values.Length);
            return values.GetValue(randomIdx);
        }

        public TValue ValueFrom<TKey,TValue>(IDictionary<TKey,TValue> items)
        {
            var key = ItemFrom(items.Keys);
            return items[key];
        }

        public T ItemFrom<T>(ICollection<T> items)
        {
            var index = Int(0, items.Count);
            return items.ElementAt(index);
        }

        public String AlphaNumericString()
        {
            var len = Int(DefaultMinStringLen, DefaultMaxStringLen + 1);
            return AlphaNumericString(len);
        }

        public String AlphaNumericString(int len)
        {
            var chars = new char[len];
            for (int i = 0; i < len; i++)
            {
                chars[i] = AlphaNumericChar();
            }
            return new String(chars);
        }

        public String AsciiString()
        {
            var len = Int(DefaultMinStringLen, DefaultMaxStringLen + 1);
            return AsciiString(len);
        }

        public String AsciiString(int len)
        {
            var chars = new char[len];
            for (int i = 0; i < len; i++)
            {
                chars[i] = Convert.ToChar(m_random.Next(32, 126 + 1));
            }
            return new String(chars);
        }

        public String GuidString()
        {
            return Guid().ToString();
        }

        public String IncrementingString()
        {
            return "RandomString" + m_count++;
        }

        /// <summary>
        /// Return a utf8 string which uses the full utf8 code plane, including supplementary planes
        /// </summary>
        /// <returns></returns>
        public String Utf8String()
        {
            var len = Int(DefaultMinStringLen, DefaultMaxStringLen + 1);
            return Utf8String(len);
        }

        public String Utf8String(int len)
        {                                    
            var chars = new char[len];
            for (int i = 0; i < len; i++)
            {
                chars[i] = m_utf8CharProvider.NextFullChar();
            }
            return new String(chars);
        }

        /// <summary>
        /// Return a utf8 string which uses only the basic utf8 code plane, meaning no supplementary planes used
        /// </summary>
        /// <returns></returns>
        public String BasicUtf8String()
        {
            var len = Int(DefaultMinStringLen, DefaultMaxStringLen + 1);
            return BasicUtf8String(len);
        }

        public String BasicUtf8String(int len)
        {                                    
            var chars = new char[len];
            for (int i = 0; i < len; i++)
            {
                chars[i] = m_utf8CharProvider.NextBasicChar();
            }
            return new String(chars);
        }

        public bool Bool()
        {
            return m_random.Next(0,2)==0;
        }

        public byte Byte()
        {
            return (byte)m_random.Next(System.Byte.MinValue,System.Byte.MaxValue + 1 );
        }

        public byte[] Bytes()
        {
            return BytesOfLengthBetween(0, 100);
        }

        public byte[] BytesOfLengthBetween(int minsizeInclusive, int maxSizeExclusive)
        {
            return BytesOfLength(Int(minsizeInclusive, maxSizeExclusive));
        }

        public byte[] BytesOfLength(int numBytes)
        {
            var bytes = new byte[numBytes];
            for (var i = 0; i < numBytes; i++)
            {
                bytes[i] = Byte();
            }
            return bytes;
        }

        public SByte SByte()
        {
            return (SByte)m_random.Next(System.SByte.MinValue,System.SByte.MaxValue + 1);
        }

        public char Char()
        {
            return (char)m_random.Next(System.Char.MinValue,System.Char.MaxValue + 1);
        }

        public char Utf8Char()
        {
            return m_utf8CharProvider.NextFullChar();
        }
        
        public char AlphaNumericChar()
        {
            return AlphaNumeric[m_random.Next(0,AlphaNumeric.Length)];
        }

        public char BasicUtf8Char()
        {
            return m_utf8CharProvider.NextBasicChar();
        }

        public char AsciiChar()
        {
            m_charCount++;
            if (m_charCount > 126)
            {
                m_charCount = 32;
            }
            return (char)m_charCount;
        }

        public short Short()
        {
            return (short)m_random.Next(short.MinValue,short.MaxValue + 1);
        }

        public int Int()
        {
            return m_random.Next();
        }

        [Obsolete("Use Int(min,maxInclusive) instead")]
        public int IntBetween(int minInclusive, int maxExclusive)
        {
            return Int(minInclusive, maxExclusive);
        }

        public int Int(int minInclusive, int maxExclusive)
        {
            return m_random.Next(minInclusive, maxExclusive);
        }

        public UInt16 UInt16()
        {
            return (UInt16)m_random.Next(System.UInt16.MinValue,System.UInt16.MaxValue + 1);
        }

        public Int16 Int16()
        {
            return (Int16)m_random.Next(System.Int16.MinValue,System.Int16.MaxValue + 1);
        }

        public UInt32 UInt32()
        {
            var bytes = new byte[4];
            m_random.NextBytes(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }

        public Int32 Int32()
        {
            var bytes = new byte[4];
            m_random.NextBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        public UInt64 UInt64()
        {
            var bytes = new byte[8];
            m_random.NextBytes(bytes);
            return BitConverter.ToUInt64(bytes, 0);
        }

        public Int64 Int64()
        {
            var bytes = new byte[8];
            m_random.NextBytes(bytes);
            return BitConverter.ToInt64(bytes, 0);
        }

        public double Double()
        {
            return m_random.NextDouble();
        }

        public long Long()
        {
            return Long(System.Int64.MinValue,System.Int64.MaxValue);
        }

        public long Long(long min, long maxInclusive)
        {
            var bytes = new byte[8];
            m_random.NextBytes(bytes);
            long longRand = BitConverter.ToInt64(bytes, 0);

            return (Math.Abs(longRand % (maxInclusive - min)) + min);
        }

        public float Float()
        {
            double mantissa = (m_random.NextDouble() * 2.0) - 1.0;
            double exponent = Math.Pow(2.0, m_random.Next(-126, 128));
            return (float)(mantissa * exponent);
        }

        //taken from http://stackoverflow.com/questions/609501/generating-a-random-decimal-in-c-sharp
        public Decimal Decimal()
        {
            byte scale = (byte)m_random.Next(29);
            bool sign = Bool();
            return new Decimal(Int32()/*low*/, Int32()/*mid*/, Int32()/*high*/, sign, scale);
        }

        public Guid Guid()
        {
            return System.Guid.NewGuid();
        }

        public DateTime DateTime()
        {
            return new DateTime(Long(MinDateTimeTicks, MaxDateTimeTicks));
        }

        public DateTimeOffset DateTimeOffset()
        {
            return new DateTimeOffset(Long(MinDateTimeTicks, MaxDateTimeTicks), System.TimeSpan.FromHours(Int(0,13)));
        }

        public TimeSpan TimeSpan()
        {
            //want duration to keep working, hence we stay just below the min/max range
            var ticks = Long(System.TimeSpan.MinValue.Ticks + 1, System.TimeSpan.MaxValue.Ticks - 1);
            return new System.TimeSpan(ticks);
        }

        public TimeSpan TimeSpanWithinDay()
        {
            return TimeSpan(System.TimeSpan.FromMilliseconds(0), System.TimeSpan.FromDays(1));
        }

        public TimeSpan TimeSpan(TimeSpan min, TimeSpan maxInclusive)
        {
            var ticks = Long(min.Ticks, maxInclusive.Ticks);
            return new TimeSpan(ticks);
        }

        public Object Object()
        {
            return new RandomObj(GuidString());
        }

        [DataContract]
        private class RandomObj
        {
            [DataMember]
            private readonly Object m_value;

            internal RandomObj(Object value)
            {
                m_value = value;
            }

            public override String ToString()
            {
                return String.Format("{0}[{1}]", GetType().Name, m_value);
            }
        }

        /// <summary>
        /// based on https://github.com/erikrozendaal/scalacheck-blog/blob/master/src/main/scala/Solution2.scala
        /// </summary>
        class Utf8CharProvider
        {            
            private readonly System.Random m_random = new System.Random(Environment.TickCount);

            private readonly char[] m_unicodeBasicMultilingualPlane;
     
            private static readonly Range All = new Range(0x0000,0xFFFF);
            //surrogates
            private static readonly Range Leading = new Range(0xD800,0xDBFF);
            private static readonly Range Trailing = new Range(0xDC00,0xDFFF);
            
            class Range
            {
                public readonly int Min;
                public readonly int Max;
                public readonly int Num;
                
                internal Range(int min, int max)
                {
                    Min = min;
                    Max = max;
                    Num = Max - Min + 1;
                }
            }

            internal Utf8CharProvider()
            {
                //TODO:don't generate all, calc on the fly instead to save memory
                //currently about 65K entries
                m_unicodeBasicMultilingualPlane = CharRange(All)
                    .Except(CharRange(Leading))
                    .Except(CharRange(Trailing))
                    .ToArray();
            }

            private static char[] CharRange(Range range)
            {
                var chars = new char[range.Num];
                for (int i = 0; i < chars.Length; i++)
                {
                    chars[i]=(char)(range.Min + i);
                }
                return chars;
            }

            public char NextFullChar()
            {
                //90% of the time use the basic plane unicode
                var distribution = m_random.Next(0, 10);
                if( distribution < 9 )
                {
                    return NextBasicChar();
                }
                else
                {
                    return NextSupplementarChar();
                }
            }

            public char NextBasicChar()
            {
                //next random char between limits which is not in the surrogate ranges
                return m_unicodeBasicMultilingualPlane[m_random.Next(0, m_unicodeBasicMultilingualPlane.Length)];
            }

            public char NextSupplementarChar()
            {
                var highChar = NextCharInRange(Leading);
                var lowChar = NextCharInRange(Trailing);

                return (char) char.ConvertToUtf32(highChar, lowChar);
            }

            private char NextCharInRange(Range range)
            {
                return (char)m_random.Next(range.Min,range.Max + 1);
            }
                        
 
        }
    }
}