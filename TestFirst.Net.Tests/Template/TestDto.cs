using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestFirst.Net.Template
{
    public class TestDto
    {
        public String MyStringProp { get; set; }

        public bool MyPrimBoolProp { get; set; }
        public byte MyPrimByteProp { get; set; }
        public char MyPrimCharProp { get; set; }
        public short MyPrimShortProp { get; set; }
        public int MyPrimIntProp { get; set; }
        public long MyPrimLongProp { get; set; }
        public float MyPrimFloatProp { get; set; }
        public double MyPrimDoubleProp { get; set; }
        public decimal MyPrimDecimalProp { get; set; }

        public Boolean MyBoolProp { get; set; }
        public Char MyCharProp { get; set; }
        public Int32 MyInt32Prop { get; set; }
        public Int64 MyInt64Prop { get; set; }
        
        public Double MyDoubleProp { get; set; }
        public Decimal MyDecimalProp { get; set; }
        public Guid MyGuidProp { get; set; }
        public TimeSpan MyTimeSpan { get; set; }
        public Uri MyUri { get; set; }
        public IDictionary<String,int?> MyDictionary { get; set; }

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

        public String[] MyArrayOfStringsProp { get; set; }
        public int?[] MyArrayOfNullIntsProp { get; set; }

        public IList<String> MyListOfStringsProp { get; set; }
        public IList<int?> MyListOfNullIntsProp { get; set; }

        public IList<IList<String>> MyListOfListsProp { get; set;  }

        public TestDto2 MyDto2 { get; set;  }

        public TestDtoEnumerable MyDtoEnmerable { get; set; }

    }

    public class TestDto2
    {
        public String MyString { get; set; }
    }

    public class TestDtoEnumerable : List<String>
    {
        public String MyProp { get; set; }
    }
}
