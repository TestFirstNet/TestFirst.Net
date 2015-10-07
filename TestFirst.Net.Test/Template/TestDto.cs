using System;
using System.Collections.Generic;

namespace TestFirst.Net.Template
{
    public enum TestEnum
    {
        One, Two
    }

    public class TestDto
    {
        public string MyStringProp { get; set; }

        public bool MyPrimBoolProp { get; set; }
        public byte MyPrimByteProp { get; set; }
        public char MyPrimCharProp { get; set; }
        public short MyPrimShortProp { get; set; }
        public int MyPrimIntProp { get; set; }
        public long MyPrimLongProp { get; set; }
        public float MyPrimFloatProp { get; set; }
        public double MyPrimDoubleProp { get; set; }
        public decimal MyPrimDecimalProp { get; set; }

        public bool MyBoolProp { get; set; }
        public char MyCharProp { get; set; }
        public int MyInt32Prop { get; set; }
        public long MyInt64Prop { get; set; }
        
        public double MyDoubleProp { get; set; }
        public decimal MyDecimalProp { get; set; }
        public Guid MyGuidProp { get; set; }
        public TimeSpan MyTimeSpan { get; set; }
        public Uri MyUri { get; set; }
        public IDictionary<string, int?> MyDictionary { get; set; }

        public bool? MyNullBoolProp { get; set; }
        public byte? MyNullByteProp { get; set; }
        public char? MyNullCharProp { get; set; }
        public short? MyNullShortProp { get; set; }
        public int? MyNullIntProp { get; set; }
        public long? MyNullLongProp { get; set; }
        public float? MyNullFloatProp { get; set; }
        public double? MyNullDoubleProp { get; set; }
        public decimal? MyNullDecimalProp { get; set; }
        public Guid? MyNullGuidProp { get; set; }
        public TimeSpan? MyNullTimeSpan { get; set; }

        public string[] MyArrayOfStringsProp { get; set; }
        public int?[] MyArrayOfNullIntsProp { get; set; }

        public IList<string> MyListOfStringsProp { get; set; }
        public IList<int?> MyListOfNullIntsProp { get; set; }

        public IList<IList<string>> MyListOfListsProp { get; set; }
        public IDictionary<string, string> MyDictionaryOfStrings { get; set; }
        public IEnumerable<string> MyEnumerableOfStringsProp { get; set; }

        public TestDto2 MyDto2 { get; set;  }
        public TestDtoEnumerable MyDtoEnmerable { get; set; }
        public TestEnum MyTestEnum { get; set; }
    }

    public class TestDto2
    {
        public string MyProp { get; set; }
    }

    public class TestDto3 // to be renamed
    {
        public string MyProp { get; set; }
        public string MyExcludedProp { get; set; }
    }

    public class TestDtoEnumerable : List<string>
    {
        public string MyProp { get; set; }
    }

    public class TestIndexedDto
    {
        public string this[string key]
        {
            get
            {
                return null;
            }
            set
            {
               // do nothing
            }
        }
    }

    public class TestDtoWithSubClass
    {
        public SubTestDto SubDto { get; set; }

        public class SubTestDto
        {
            public string SomeProp { get; set;  }
        }
    }
}
