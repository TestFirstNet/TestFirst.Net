using System;
using System.Collections.Generic;
using NUnit.Framework;
using TestFirst.Net.Rand;

namespace TestFirst.Net.Test.Rand
{
    [TestFixture]
    public class RandomFillerTest
    {
        [Test]
        public void PrimitiveFillTest()
        {
            var dto = NewFiller().FillWithRandom(new MyPrimitiveDto());
            Assert.IsNotNullOrEmpty(dto.MyString);

            Assert.NotNull(dto.MyBool);
            Assert.NotNull(dto.MyChar);
            Assert.NotNull(dto.MyByte);
            Assert.NotNull(dto.MyShort);
            Assert.NotNull(dto.MyInt);
            Assert.NotNull(dto.MyLong);
            Assert.NotNull(dto.MyDouble);
            Assert.NotNull(dto.MyFloat);
        }

        [Test]
        public void GuidFillTest()
        {
            var dto = NewFiller().FillWithRandom(new MyGuidDto());
            Assert.NotNull(dto.MyGuid);
            Assert.False(dto.MyGuid.Value == Guid.Empty);
        }

        [Test]
        public void ObjectFillTest()
        {
            var dto = NewFiller().FillWithRandom(new MyObjectDto());
            Assert.NotNull(dto.MyPrimtiveDto);
            var childDto = dto.MyPrimtiveDto;
            
            Assert.NotNull(childDto.MyBool);
            Assert.NotNull(childDto.MyChar);
            Assert.NotNull(childDto.MyByte);
            Assert.NotNull(childDto.MyShort);
            Assert.NotNull(childDto.MyInt);
            Assert.NotNull(childDto.MyLong);
            Assert.NotNull(childDto.MyDouble);
            Assert.NotNull(childDto.MyFloat);
        }

        [Test]
        public void EnumFillTest()
        {            
            var dto = new MyEnumDto();
            Assert.Null(dto.MyEnumValue);
            NewFiller().FillWithRandom(dto);
            
            Assert.NotNull(dto.MyEnumValue);
        }

        [Test]
        public void ListFillTest()
        {
            var dto = NewFiller().FillWithRandom(new MyListDto());
            Assert.NotNull(dto.MyList);
            Assert.True(dto.MyList.Count > 0);
            foreach (var item in dto.MyList)
            {
                Assert.IsNotNullOrEmpty(item);
            }
        }

        
        [Test]
        public void ConcreteListFillTest()
        {
            var dto = NewFiller().FillWithRandom(new MyConcreteListDto());
            Assert.NotNull(dto.MyList);
            Assert.True(dto.MyList.Count > 0);
            foreach (var item in dto.MyList)
            {
                Assert.IsNotNullOrEmpty(item);
            }
        }

        [Test]
        public void DictFillTest()
        {
            var dto = NewFiller().FillWithRandom(new MyDictDto());
            Assert.NotNull(dto.MyDict);
            Assert.True(dto.MyDict.Count > 0);
            foreach (var entry in dto.MyDict)
            {
                Assert.IsNotNullOrEmpty(entry.Key);
                Assert.NotNull(entry.Value);
            }
        }

        [Test]
        public void ConcreteDictFillTest()
        {
            var dto = NewFiller().FillWithRandom(new MyConcreteDictDto());
            Assert.NotNull(dto.MyDict);
            Assert.True(dto.MyDict.Count > 0);
            foreach (var entry in dto.MyDict)
            {
                Assert.IsNotNullOrEmpty(entry.Key);
                Assert.NotNull(entry.Value);
            }
        }

        [Test]
        public void DateTimeFillTest()
        {            
            var dto = new MyDateTimeDto();
            Assert.Null(dto.MyDateTime);
            NewFiller().FillWithRandom(dto);
            
            Assert.NotNull(dto.MyDateTime);
        }

        [Test]
        public void TimeSpanFillTest()
        {            
            var dto = new MyTimeSpanDto();
            Assert.Null(dto.MyTimeSpan);
            NewFiller().FillWithRandom(dto);
            
            Assert.NotNull(dto.MyTimeSpan);
        }

        [Test]
        public void ArrayFillTest()
        {            
            var dto = new MyArrayDto();
            Assert.Null(dto.MyArray);
            NewFiller().FillWithRandom(dto);
            
            Assert.NotNull(dto.MyArray);
        }
        
        [Test]
        public void CustomGeneratorFillTest()
        {
            var filler = new RandomFiller.Builder()
                .ValueFactoryForType(typeof(String), () => "MyCustomValue")
                .EnableLogging(true)
                .Build();

            var dto = filler.FillWithRandom(new MyStringDto());
            Assert.NotNull(dto.MyString);
            Assert.AreEqual("MyCustomValue", dto.MyString);

            var dtoNested = filler.FillWithRandom(new MyNestedStringDto());
            Assert.NotNull(dtoNested.MyStringDto);
            Assert.NotNull(dtoNested.MyStringDto.MyString);
            Assert.AreEqual("MyCustomValue", dtoNested.MyStringDto.MyString);
        }

        private RandomFiller NewFiller()
        {
            return new RandomFiller.Builder().EnableLogging(false).Build();
        }

        class MyPrimitiveDto
        {
            public String MyString { get; set; }
            public bool? MyBool { get; set; }
            public char? MyChar { get; set; }
            public byte? MyByte { get; set; }
            public short? MyShort { get; set; }
            public int? MyInt { get; set; }
            public int? MyLong { get; set; }            
            public double? MyDouble { get; set; }
            public float? MyFloat { get; set; }
        }

        class MyStringDto
        {
            public String MyString { get; set; }
        }

        class MyNestedStringDto
        {
            public MyStringDto MyStringDto { get; set; }
        }

        class MyObjectDto
        {
            public MyPrimitiveDto MyPrimtiveDto { get; set; }
        }

        class MyGuidDto
        {
            public Guid? MyGuid { get; set; }
        }

        class MyEnumDto
        {
            public enum MyEnum
            {
                One,Two,Three
            }

            public MyEnum? MyEnumValue { get; set; }
        }

        class MyListDto
        {
            public IList<String> MyList { get; set; }
        }
        
        class MyConcreteListDto
        {
            public List<String> MyList { get; set; }
        }

        class MyDictDto
        {
            public IDictionary<String,Double?> MyDict { get; set; }
        }

        class MyConcreteDictDto
        {
            public Dictionary<String,Double?> MyDict { get; set; }
        }

        class MyDateTimeDto
        {
            public DateTime? MyDateTime { get; set; }
        }

        class MyTimeSpanDto
        {
            public TimeSpan? MyTimeSpan { get; set; }
        }

        class MyArrayDto
        {
            public String[] MyArray { get; set; }
        }
    }
}
