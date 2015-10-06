using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace TestFirst.Net.Rand
{
    public class Random
    {
        public static readonly Random Instance = new Random();

        private const int DefaultMinStringLen = 1;
        private const int DefaultMaxStringLen = 20;
        private const string AlphaNumeric = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ01234567890";

        private static readonly long MinDateTimeTicks = new DateTime(1, 1, 1, 0, 0, 0, 0).Ticks;
        private static readonly long MaxDateTimeTicks = new DateTime(2200, 1, 1, 0, 0, 0, 0).Ticks;

        private readonly System.Random m_random = new System.Random(Environment.TickCount);
        private readonly Utf8CharProvider m_utf8CharProvider = new Utf8CharProvider();

        private int m_count;
        private int m_charCount = 32;

        public T EnumOf<T>() 
            where T : struct, IConvertible
        {
            return (T)EnumOf(typeof(T));
        }

        public object EnumOf(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException(string.Format("Expect an enumerated type but got {0}", enumType.FullName));
            }
            var values = Enum.GetValues(enumType);
            var randomIdx = Int(0, values.Length);
            return values.GetValue(randomIdx);
        }

        public TValue ValueFrom<TKey, TValue>(IDictionary<TKey, TValue> items)
        {
            var key = ItemFrom(items.Keys);
            return items[key];
        }

        public T ItemFrom<T>(ICollection<T> items)
        {
            var index = Int(0, items.Count);
            return items.ElementAt(index);
        }

        public string AlphaNumericString()
        {
            var len = Int(DefaultMinStringLen, DefaultMaxStringLen + 1);
            return AlphaNumericString(len);
        }

        public string AlphaNumericString(int len)
        {
            var chars = new char[len];
            for (int i = 0; i < len; i++)
            {
                chars[i] = AlphaNumericChar();
            }
            return new string(chars);
        }

        public string AsciiString()
        {
            var len = Int(DefaultMinStringLen, DefaultMaxStringLen + 1);
            return AsciiString(len);
        }

        public string AsciiString(int len)
        {
            var chars = new char[len];
            for (int i = 0; i < len; i++)
            {
                chars[i] = Convert.ToChar(m_random.Next(32, 126 + 1));
            }
            return new string(chars);
        }

        public string GuidString()
        {
            return Guid().ToString();
        }

        public string IncrementingString()
        {
            return "RandomString" + m_count++;
        }

        /// <summary>
        /// Returns a utf8 string which uses the full utf8 code plane, including supplementary planes
        /// </summary>
        /// <returns>a random utf8 string</returns>
        public string Utf8String()
        {
            var len = Int(DefaultMinStringLen, DefaultMaxStringLen + 1);
            return Utf8String(len);
        }

        public string Utf8String(int len)
        {                                    
            var chars = new char[len];
            for (int i = 0; i < len; i++)
            {
                chars[i] = m_utf8CharProvider.NextFullChar();
            }
            return new string(chars);
        }

        /// <summary>
        /// Return a utf8 string which uses only the basic utf8 code plane, meaning no supplementary planes used
        /// </summary>
        /// <returns>a random string using only the basic utf8 code plane</returns>
        public string BasicUtf8String()
        {
            var len = Int(DefaultMinStringLen, DefaultMaxStringLen + 1);
            return BasicUtf8String(len);
        }

        public string BasicUtf8String(int len)
        {                                    
            var chars = new char[len];
            for (int i = 0; i < len; i++)
            {
                chars[i] = m_utf8CharProvider.NextBasicChar();
            }
            return new string(chars);
        }

        public bool Bool()
        {
            return m_random.Next(0, 2) == 0;
        }

        public byte Byte()
        {
            return (byte)m_random.Next(byte.MinValue, byte.MaxValue + 1);
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

        public sbyte SByte()
        {
            return (sbyte)m_random.Next(sbyte.MinValue, sbyte.MaxValue + 1);
        }

        public char Char()
        {
            return (char)m_random.Next(char.MinValue, char.MaxValue + 1);
        }

        public char Utf8Char()
        {
            return m_utf8CharProvider.NextFullChar();
        }
        
        public char AlphaNumericChar()
        {
            return AlphaNumeric[m_random.Next(0, AlphaNumeric.Length)];
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
            return (short)m_random.Next(short.MinValue, short.MaxValue + 1);
        }

        public int Int()
        {
            return m_random.Next();
        }

        [Obsolete("Use Int(min, maxInclusive) instead")]
        public int IntBetween(int minInclusive, int maxExclusive)
        {
            return Int(minInclusive, maxExclusive);
        }

        public int Int(int minInclusive, int maxExclusive)
        {
            return m_random.Next(minInclusive, maxExclusive);
        }

        public ushort UInt16()
        {
            return (ushort)m_random.Next(ushort.MinValue, ushort.MaxValue + 1);
        }

        public short Int16()
        {
            return (short)m_random.Next(short.MinValue, short.MaxValue + 1);
        }

        public uint UInt32()
        {
            var bytes = new byte[4];
            m_random.NextBytes(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }

        public int Int32()
        {
            var bytes = new byte[4];
            m_random.NextBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        public ulong UInt64()
        {
            var bytes = new byte[8];
            m_random.NextBytes(bytes);
            return BitConverter.ToUInt64(bytes, 0);
        }

        public long Int64()
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
            return Long(long.MinValue, long.MaxValue);
        }

        public long Long(long min, long maxInclusive)
        {
            var bytes = new byte[8];
            m_random.NextBytes(bytes);
            long longRand = BitConverter.ToInt64(bytes, 0);

            return Math.Abs(longRand % (maxInclusive - min)) + min;
        }

        public float Float()
        {
            double mantissa = (m_random.NextDouble() * 2.0) - 1.0;
            double exponent = Math.Pow(2.0, m_random.Next(-126, 128));
            return (float)(mantissa * exponent);
        }

        // taken from http://stackoverflow.com/questions/609501/generating-a-random-decimal-in-c-sharp
        public decimal Decimal()
        {
            byte scale = (byte)m_random.Next(29);
            bool sign = Bool();
            return new decimal(Int32() /*low*/, Int32() /*mid*/, Int32() /*high*/, sign, scale);
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
            return new DateTimeOffset(Long(MinDateTimeTicks, MaxDateTimeTicks), System.TimeSpan.FromHours(Int(0, 13)));
        }

        public TimeSpan TimeSpan()
        {
            // want duration to keep working, hence we stay just below the min/max range
            var ticks = Long(System.TimeSpan.MinValue.Ticks + 1, System.TimeSpan.MaxValue.Ticks - 1);
            return new TimeSpan(ticks);
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

        public object Object()
        {
            return new RandomObj(GuidString());
        }

        [DataContract]
        private class RandomObj
        {
            [DataMember]
            private readonly object m_value;

            internal RandomObj(object value)
            {
                m_value = value;
            }

            public override string ToString()
            {
                return string.Format("{0}[{1}]", GetType().Name, m_value);
            }
        }

        /// <summary>
        /// based on <a href="https://github.com/erikrozendaal/scalacheck-blog/blob/master/src/main/scala/Solution2.scala">Solution2.scala</a>
        /// </summary>
        private class Utf8CharProvider
        {
            private static readonly Range All = new Range(0x0000, 0xFFFF);

            // surrogates
            private static readonly Range Leading = new Range(0xD800, 0xDBFF);
            private static readonly Range Trailing = new Range(0xDC00, 0xDFFF);

            private readonly System.Random m_random = new System.Random(Environment.TickCount);

            private readonly char[] m_unicodeBasicMultilingualPlane;
            
            internal Utf8CharProvider()
            {
                // TODO:don't generate all, calc on the fly instead to save memory
                // currently about 65K entries
                m_unicodeBasicMultilingualPlane = CharRange(All)
                    .Except(CharRange(Leading))
                    .Except(CharRange(Trailing))
                    .ToArray();
            }

            public char NextFullChar()
            {
                // 90% of the time use the basic plane unicode
                var distribution = m_random.Next(0, 10);
                if (distribution < 9)
                {
                    return NextBasicChar();
                }
                return NextSupplementarChar();
            }

            public char NextBasicChar()
            {
                // next random char between limits which is not in the surrogate ranges
                return m_unicodeBasicMultilingualPlane[m_random.Next(0, m_unicodeBasicMultilingualPlane.Length)];
            }

            public char NextSupplementarChar()
            {
                var highChar = NextCharInRange(Leading);
                var lowChar = NextCharInRange(Trailing);

                return (char)char.ConvertToUtf32(highChar, lowChar);
            }

            private static char[] CharRange(Range range)
            {
                var chars = new char[range.Num];
                for (int i = 0; i < chars.Length; i++)
                {
                    chars[i] = (char)(range.Min + i);
                }
                return chars;
            }

            private char NextCharInRange(Range range)
            {
                return (char)m_random.Next(range.Min, range.Max + 1);
            }

            private class Range
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
        }
    }
}