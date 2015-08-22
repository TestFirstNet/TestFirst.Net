using System;
using System.Collections.Generic;
using TestFirst.Net;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Template {

    public class ATestDto : PropertyMatcher<TestDto>{

        // provide IDE rename and find reference support
        private static readonly TestDto PropertyNames = null;


        public static ATestDto With(){
                return new ATestDto();
        }

        public static IMatcher<TestDto> Null(){
                return AnInstance.Null<TestDto>();
        }

        public static IMatcher<TestDto> NotNull(){
                return AnInstance.NotNull<TestDto>();
        }

        public static IMatcher<TestDto> Instance(TestDto expect){
                return AnInstance.SameAs(expect);
        }

        public ATestDto MyArrayOfNullIntsPropNull() {
            MyArrayOfNullIntsProp(AnInstance.EqualTo<int?[]>(null));
            return this;
        }

        public ATestDto MyArrayOfNullIntsProp(IMatcher<int?[]> matcher) {
            WithProperty(()=>PropertyNames.MyArrayOfNullIntsProp,matcher);
            return this;
        }

        public ATestDto MyArrayOfStringsPropNull() {
            MyArrayOfStringsProp(AnInstance.EqualTo<string[]>(null));
            return this;
        }

        public ATestDto MyArrayOfStringsProp(IMatcher<string[]> matcher) {
            WithProperty(()=>PropertyNames.MyArrayOfStringsProp,matcher);
            return this;
        }

        public ATestDto MyBoolProp(bool expect) {
            MyBoolProp(ABool.EqualTo(expect));
            return this;
        }

        public ATestDto MyBoolProp(IMatcher<bool?> matcher) {
            WithProperty(()=>PropertyNames.MyBoolProp,matcher);
            return this;
        }

        public ATestDto MyCharProp(IMatcher<char?> matcher) {
            WithProperty(()=>PropertyNames.MyCharProp,matcher);
            return this;
        }

        public ATestDto MyDecimalProp(decimal expect) {
            MyDecimalProp(ADecimal.EqualTo(expect));
            return this;
        }

        public ATestDto MyDecimalProp(IMatcher<decimal?> matcher) {
            WithProperty(()=>PropertyNames.MyDecimalProp,matcher);
            return this;
        }

        public ATestDto MyDictionaryNull() {
            MyDictionary(AnInstance.EqualTo<IDictionary<string,int?>>(null));
            return this;
        }

        public ATestDto MyDictionary(IMatcher<IDictionary<string,int?>> matcher) {
            WithProperty(()=>PropertyNames.MyDictionary,matcher);
            return this;
        }

        public ATestDto MyDictionaryOfStringsNull() {
            MyDictionaryOfStrings(AnInstance.EqualTo<IDictionary<string,string>>(null));
            return this;
        }

        public ATestDto MyDictionaryOfStrings(IMatcher<IDictionary<string,string>> matcher) {
            WithProperty(()=>PropertyNames.MyDictionaryOfStrings,matcher);
            return this;
        }

        public ATestDto MyDoubleProp(double expect) {
            MyDoubleProp(ADouble.EqualTo(expect));
            return this;
        }

        public ATestDto MyDoubleProp(IMatcher<double?> matcher) {
            WithProperty(()=>PropertyNames.MyDoubleProp,matcher);
            return this;
        }

        public ATestDto MyDto2Null() {
            MyDto2(AnInstance.EqualTo<TestDto2>(null));
            return this;
        }

        public ATestDto MyDto2(IMatcher<TestDto2> matcher) {
            WithProperty(()=>PropertyNames.MyDto2,matcher);
            return this;
        }

        public ATestDto MyDtoEnmerableNull() {
            MyDtoEnmerable(AnInstance.EqualTo<TestDtoEnumerable>(null));
            return this;
        }

        public ATestDto MyDtoEnmerable(IMatcher<TestDtoEnumerable> matcher) {
            WithProperty(()=>PropertyNames.MyDtoEnmerable,matcher);
            return this;
        }

        public ATestDto MyEnumerableOfStringsPropNull() {
            MyEnumerableOfStringsProp(AnInstance.EqualTo<IEnumerable<string>>(null));
            return this;
        }

        public ATestDto MyEnumerableOfStringsProp(IMatcher<IEnumerable<string>> matcher) {
            WithProperty(()=>PropertyNames.MyEnumerableOfStringsProp,matcher);
            return this;
        }

        public ATestDto MyGuidProp(Guid expect) {
            MyGuidProp(AGuid.EqualTo(expect));
            return this;
        }

        public ATestDto MyGuidProp(IMatcher<Guid?> matcher) {
            WithProperty(()=>PropertyNames.MyGuidProp,matcher);
            return this;
        }

        public ATestDto MyInt32Prop(int expect) {
            MyInt32Prop(AnInt.EqualTo(expect));
            return this;
        }

        public ATestDto MyInt32Prop(IMatcher<int?> matcher) {
            WithProperty(()=>PropertyNames.MyInt32Prop,matcher);
            return this;
        }

        public ATestDto MyInt64Prop(long expect) {
            MyInt64Prop(ALong.EqualTo(expect));
            return this;
        }

        public ATestDto MyInt64Prop(IMatcher<long?> matcher) {
            WithProperty(()=>PropertyNames.MyInt64Prop,matcher);
            return this;
        }

        public ATestDto MyListOfListsPropNull() {
            MyListOfListsProp(AnInstance.EqualTo<IEnumerable<IEnumerable<string>>>(null));
            return this;
        }

        public ATestDto MyListOfListsProp(IMatcher<IEnumerable<IEnumerable<string>>> matcher) {
            WithProperty(()=>PropertyNames.MyListOfListsProp,matcher);
            return this;
        }

        public ATestDto MyListOfNullIntsPropNull() {
            MyListOfNullIntsProp(AnInstance.EqualTo<IEnumerable<int?>>(null));
            return this;
        }

        public ATestDto MyListOfNullIntsProp(IMatcher<IEnumerable<int?>> matcher) {
            WithProperty(()=>PropertyNames.MyListOfNullIntsProp,matcher);
            return this;
        }

        public ATestDto MyListOfStringsPropNull() {
            MyListOfStringsProp(AnInstance.EqualTo<IEnumerable<string>>(null));
            return this;
        }

        public ATestDto MyListOfStringsProp(IMatcher<IEnumerable<string>> matcher) {
            WithProperty(()=>PropertyNames.MyListOfStringsProp,matcher);
            return this;
        }

        public ATestDto MyNullBoolProp(bool? expect) {
            MyNullBoolProp(ABool.EqualTo(expect));
            return this;
        }

        public ATestDto MyNullBoolPropNull() {
            MyNullBoolProp(ABool.Null());
            return this;
        }

        public ATestDto MyNullBoolProp(IMatcher<bool?> matcher) {
            WithProperty(()=>PropertyNames.MyNullBoolProp,matcher);
            return this;
        }

        public ATestDto MyNullBytePropNull() {
            MyNullByteProp(AnInstance.EqualTo<byte?>(null));
            return this;
        }

        public ATestDto MyNullByteProp(IMatcher<byte?> matcher) {
            WithProperty(()=>PropertyNames.MyNullByteProp,matcher);
            return this;
        }

        public ATestDto MyNullCharPropNull() {
            MyNullCharProp(AnInstance.EqualTo<char?>(null));
            return this;
        }

        public ATestDto MyNullCharProp(IMatcher<char?> matcher) {
            WithProperty(()=>PropertyNames.MyNullCharProp,matcher);
            return this;
        }

        public ATestDto MyNullDecimalProp(decimal? expect) {
            MyNullDecimalProp(ADecimal.EqualTo(expect));
            return this;
        }

        public ATestDto MyNullDecimalPropNull() {
            MyNullDecimalProp(ADecimal.Null());
            return this;
        }

        public ATestDto MyNullDecimalProp(IMatcher<decimal?> matcher) {
            WithProperty(()=>PropertyNames.MyNullDecimalProp,matcher);
            return this;
        }

        public ATestDto MyNullDoubleProp(double? expect) {
            MyNullDoubleProp(ADouble.EqualTo(expect));
            return this;
        }

        public ATestDto MyNullDoublePropNull() {
            MyNullDoubleProp(ADouble.Null());
            return this;
        }

        public ATestDto MyNullDoubleProp(IMatcher<double?> matcher) {
            WithProperty(()=>PropertyNames.MyNullDoubleProp,matcher);
            return this;
        }

        public ATestDto MyNullFloatProp(float? expect) {
            MyNullFloatProp(AFloat.EqualTo(expect));
            return this;
        }

        public ATestDto MyNullFloatPropNull() {
            MyNullFloatProp(AFloat.Null());
            return this;
        }

        public ATestDto MyNullFloatProp(IMatcher<float?> matcher) {
            WithProperty(()=>PropertyNames.MyNullFloatProp,matcher);
            return this;
        }

        public ATestDto MyNullGuidProp(Guid? expect) {
            MyNullGuidProp(AGuid.EqualTo(expect));
            return this;
        }

        public ATestDto MyNullGuidPropNull() {
            MyNullGuidProp(AGuid.Null());
            return this;
        }

        public ATestDto MyNullGuidProp(IMatcher<Guid?> matcher) {
            WithProperty(()=>PropertyNames.MyNullGuidProp,matcher);
            return this;
        }

        public ATestDto MyNullIntProp(int? expect) {
            MyNullIntProp(AnInt.EqualTo(expect));
            return this;
        }

        public ATestDto MyNullIntPropNull() {
            MyNullIntProp(AnInt.Null());
            return this;
        }

        public ATestDto MyNullIntProp(IMatcher<int?> matcher) {
            WithProperty(()=>PropertyNames.MyNullIntProp,matcher);
            return this;
        }

        public ATestDto MyNullLongProp(long? expect) {
            MyNullLongProp(ALong.EqualTo(expect));
            return this;
        }

        public ATestDto MyNullLongPropNull() {
            MyNullLongProp(ALong.Null());
            return this;
        }

        public ATestDto MyNullLongProp(IMatcher<long?> matcher) {
            WithProperty(()=>PropertyNames.MyNullLongProp,matcher);
            return this;
        }

        public ATestDto MyNullShortProp(short? expect) {
            MyNullShortProp(AShort.EqualTo(expect));
            return this;
        }

        public ATestDto MyNullShortPropNull() {
            MyNullShortProp(AShort.Null());
            return this;
        }

        public ATestDto MyNullShortProp(IMatcher<short?> matcher) {
            WithProperty(()=>PropertyNames.MyNullShortProp,matcher);
            return this;
        }

        public ATestDto MyNullTimeSpanNull() {
            MyNullTimeSpan(AnInstance.EqualTo<TimeSpan?>(null));
            return this;
        }

        public ATestDto MyNullTimeSpan(IMatcher<TimeSpan?> matcher) {
            WithProperty(()=>PropertyNames.MyNullTimeSpan,matcher);
            return this;
        }

        public ATestDto MyPrimBoolProp(bool expect) {
            MyPrimBoolProp(ABool.EqualTo(expect));
            return this;
        }

        public ATestDto MyPrimBoolProp(IMatcher<bool?> matcher) {
            WithProperty(()=>PropertyNames.MyPrimBoolProp,matcher);
            return this;
        }

        public ATestDto MyPrimByteProp(IMatcher<byte?> matcher) {
            WithProperty(()=>PropertyNames.MyPrimByteProp,matcher);
            return this;
        }

        public ATestDto MyPrimCharProp(IMatcher<char?> matcher) {
            WithProperty(()=>PropertyNames.MyPrimCharProp,matcher);
            return this;
        }

        public ATestDto MyPrimDecimalProp(decimal expect) {
            MyPrimDecimalProp(ADecimal.EqualTo(expect));
            return this;
        }

        public ATestDto MyPrimDecimalProp(IMatcher<decimal?> matcher) {
            WithProperty(()=>PropertyNames.MyPrimDecimalProp,matcher);
            return this;
        }

        public ATestDto MyPrimDoubleProp(double expect) {
            MyPrimDoubleProp(ADouble.EqualTo(expect));
            return this;
        }

        public ATestDto MyPrimDoubleProp(IMatcher<double?> matcher) {
            WithProperty(()=>PropertyNames.MyPrimDoubleProp,matcher);
            return this;
        }

        public ATestDto MyPrimFloatProp(float expect) {
            MyPrimFloatProp(AFloat.EqualTo(expect));
            return this;
        }

        public ATestDto MyPrimFloatProp(IMatcher<float?> matcher) {
            WithProperty(()=>PropertyNames.MyPrimFloatProp,matcher);
            return this;
        }

        public ATestDto MyPrimIntProp(int expect) {
            MyPrimIntProp(AnInt.EqualTo(expect));
            return this;
        }

        public ATestDto MyPrimIntProp(IMatcher<int?> matcher) {
            WithProperty(()=>PropertyNames.MyPrimIntProp,matcher);
            return this;
        }

        public ATestDto MyPrimLongProp(long expect) {
            MyPrimLongProp(ALong.EqualTo(expect));
            return this;
        }

        public ATestDto MyPrimLongProp(IMatcher<long?> matcher) {
            WithProperty(()=>PropertyNames.MyPrimLongProp,matcher);
            return this;
        }

        public ATestDto MyPrimShortProp(short expect) {
            MyPrimShortProp(AShort.EqualTo(expect));
            return this;
        }

        public ATestDto MyPrimShortProp(IMatcher<short?> matcher) {
            WithProperty(()=>PropertyNames.MyPrimShortProp,matcher);
            return this;
        }

        public ATestDto MyStringProp(int expect) {
            MyStringProp(expect.ToString());
            return this;
        }

        public ATestDto MyStringProp(string expect) {
            MyStringProp(AString.EqualTo(expect));
            return this;
        }

        public ATestDto MyStringPropNull() {
            MyStringProp(AString.Null());
            return this;
        }

        public ATestDto MyStringProp(IMatcher<string> matcher) {
            WithProperty(()=>PropertyNames.MyStringProp,matcher);
            return this;
        }

        public ATestDto MyTestEnum(TestEnum? expect) {
            MyTestEnum(AnInstance.EqualTo(expect));
            return this;
        }

        public ATestDto MyTestEnum(IMatcher<TestEnum?> matcher) {
            WithProperty(()=>PropertyNames.MyTestEnum,matcher);
            return this;
        }

        public ATestDto MyTimeSpan(IMatcher<TimeSpan?> matcher) {
            WithProperty(()=>PropertyNames.MyTimeSpan,matcher);
            return this;
        }

        public ATestDto MyUri(Uri expect) {
            MyUri(AnUri.EqualTo(expect));
            return this;
        }

        public ATestDto MyUriNull() {
            MyUri(AnInstance.EqualTo<Uri>(null));
            return this;
        }

        public ATestDto MyUri(IMatcher<Uri> matcher) {
            WithProperty(()=>PropertyNames.MyUri,matcher);
            return this;
        }
    }
}

namespace TestFirst.Net.Template {

    public class ATestDto2 : PropertyMatcher<TestDto2>{

        // provide IDE rename and find reference support
        private static readonly TestDto2 PropertyNames = null;


        public static ATestDto2 With(){
                return new ATestDto2();
        }

        public static IMatcher<TestDto2> Null(){
                return AnInstance.Null<TestDto2>();
        }

        public static IMatcher<TestDto2> NotNull(){
                return AnInstance.NotNull<TestDto2>();
        }

        public static IMatcher<TestDto2> Instance(TestDto2 expect){
                return AnInstance.SameAs(expect);
        }

        public ATestDto2 MyProp(int expect) {
            MyProp(expect.ToString());
            return this;
        }

        public ATestDto2 MyProp(string expect) {
            MyProp(AString.EqualTo(expect));
            return this;
        }

        public ATestDto2 MyPropNull() {
            MyProp(AString.Null());
            return this;
        }

        public ATestDto2 MyProp(IMatcher<string> matcher) {
            WithProperty(()=>PropertyNames.MyProp,matcher);
            return this;
        }
    }
}

namespace TestFirst.Net.Template {

    public class MyCustomName : PropertyMatcher<TestDto3>{

        // provide IDE rename and find reference support
        private static readonly TestDto3 PropertyNames = null;


        public static MyCustomName With(){
                return new MyCustomName();
        }

        public static IMatcher<TestDto3> Null(){
                return AnInstance.Null<TestDto3>();
        }

        public static IMatcher<TestDto3> NotNull(){
                return AnInstance.NotNull<TestDto3>();
        }

        public static IMatcher<TestDto3> Instance(TestDto3 expect){
                return AnInstance.SameAs(expect);
        }

        public MyCustomName MyProp(int expect) {
            MyProp(expect.ToString());
            return this;
        }

        public MyCustomName MyProp(string expect) {
            MyProp(AString.EqualTo(expect));
            return this;
        }

        public MyCustomName MyPropNull() {
            MyProp(AString.Null());
            return this;
        }

        public MyCustomName MyProp(IMatcher<string> matcher) {
            WithProperty(()=>PropertyNames.MyProp,matcher);
            return this;
        }
    }
}

namespace TestFirst.Net.Template {

    public class ATestDtoEnumerable : PropertyMatcher<TestDtoEnumerable>{

        // provide IDE rename and find reference support
        private static readonly TestDtoEnumerable PropertyNames = null;


        public static ATestDtoEnumerable With(){
                return new ATestDtoEnumerable();
        }

        public static IMatcher<TestDtoEnumerable> Null(){
                return AnInstance.Null<TestDtoEnumerable>();
        }

        public static IMatcher<TestDtoEnumerable> NotNull(){
                return AnInstance.NotNull<TestDtoEnumerable>();
        }

        public static IMatcher<TestDtoEnumerable> Instance(TestDtoEnumerable expect){
                return AnInstance.SameAs(expect);
        }

        public ATestDtoEnumerable Capacity(int expect) {
            Capacity(AnInt.EqualTo(expect));
            return this;
        }

        public ATestDtoEnumerable Capacity(IMatcher<int?> matcher) {
            WithProperty(()=>PropertyNames.Capacity,matcher);
            return this;
        }

        public ATestDtoEnumerable Count(int expect) {
            Count(AnInt.EqualTo(expect));
            return this;
        }

        public ATestDtoEnumerable Count(IMatcher<int?> matcher) {
            WithProperty(()=>PropertyNames.Count,matcher);
            return this;
        }

        public ATestDtoEnumerable MyProp(int expect) {
            MyProp(expect.ToString());
            return this;
        }

        public ATestDtoEnumerable MyProp(string expect) {
            MyProp(AString.EqualTo(expect));
            return this;
        }

        public ATestDtoEnumerable MyPropNull() {
            MyProp(AString.Null());
            return this;
        }

        public ATestDtoEnumerable MyProp(IMatcher<string> matcher) {
            WithProperty(()=>PropertyNames.MyProp,matcher);
            return this;
        }
    }
}

namespace MyNamespace {

    public class ATestIndexedDto : PropertyMatcher<TestFirst.Net.Template.TestIndexedDto>{

        // provide IDE rename and find reference support
        private static readonly TestFirst.Net.Template.TestIndexedDto PropertyNames = null;


        public static ATestIndexedDto With(){
                return new ATestIndexedDto();
        }

        public static IMatcher<TestFirst.Net.Template.TestIndexedDto> Null(){
                return AnInstance.Null<TestFirst.Net.Template.TestIndexedDto>();
        }

        public static IMatcher<TestFirst.Net.Template.TestIndexedDto> NotNull(){
                return AnInstance.NotNull<TestFirst.Net.Template.TestIndexedDto>();
        }

        public static IMatcher<TestFirst.Net.Template.TestIndexedDto> Instance(TestFirst.Net.Template.TestIndexedDto expect){
                return AnInstance.SameAs(expect);
        }
    }
}

namespace TestFirst.Net.Template {

    public class ATestDtoWithSubClass : PropertyMatcher<TestDtoWithSubClass>{

        // provide IDE rename and find reference support
        private static readonly TestDtoWithSubClass PropertyNames = null;


        public static ATestDtoWithSubClass With(){
                return new ATestDtoWithSubClass();
        }

        public static IMatcher<TestDtoWithSubClass> Null(){
                return AnInstance.Null<TestDtoWithSubClass>();
        }

        public static IMatcher<TestDtoWithSubClass> NotNull(){
                return AnInstance.NotNull<TestDtoWithSubClass>();
        }

        public static IMatcher<TestDtoWithSubClass> Instance(TestDtoWithSubClass expect){
                return AnInstance.SameAs(expect);
        }

        public ATestDtoWithSubClass SubDtoNull() {
            SubDto(AnInstance.EqualTo<TestDtoWithSubClass.SubTestDto>(null));
            return this;
        }

        public ATestDtoWithSubClass SubDto(IMatcher<TestDtoWithSubClass.SubTestDto> matcher) {
            WithProperty(()=>PropertyNames.SubDto,matcher);
            return this;
        }
    }
}

namespace TestFirst.Net.Template {

    public class ASubTestDto : PropertyMatcher<TestDtoWithSubClass.SubTestDto>{

        // provide IDE rename and find reference support
        private static readonly TestDtoWithSubClass.SubTestDto PropertyNames = null;


        public static ASubTestDto With(){
                return new ASubTestDto();
        }

        public static IMatcher<TestDtoWithSubClass.SubTestDto> Null(){
                return AnInstance.Null<TestDtoWithSubClass.SubTestDto>();
        }

        public static IMatcher<TestDtoWithSubClass.SubTestDto> NotNull(){
                return AnInstance.NotNull<TestDtoWithSubClass.SubTestDto>();
        }

        public static IMatcher<TestDtoWithSubClass.SubTestDto> Instance(TestDtoWithSubClass.SubTestDto expect){
                return AnInstance.SameAs(expect);
        }

        public ASubTestDto SomeProp(int expect) {
            SomeProp(expect.ToString());
            return this;
        }

        public ASubTestDto SomeProp(string expect) {
            SomeProp(AString.EqualTo(expect));
            return this;
        }

        public ASubTestDto SomePropNull() {
            SomeProp(AString.Null());
            return this;
        }

        public ASubTestDto SomeProp(IMatcher<string> matcher) {
            WithProperty(()=>PropertyNames.SomeProp,matcher);
            return this;
        }
    }
}

