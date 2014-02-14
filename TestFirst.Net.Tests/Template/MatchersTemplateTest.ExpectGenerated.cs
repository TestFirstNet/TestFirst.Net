using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using TestFirst.Net.Matcher;
using TestFirst.Net;

namespace TestFirst.Net.Matcher {

    public partial class ATestDto : PropertyMatcher<TestFirst.Net.Template.TestDto>{

        // provide IDE rename and find reference support
        private static readonly TestFirst.Net.Template.TestDto PropertyNames = null;


        public static ATestDto With(){
                return new ATestDto();
        }

        public ATestDto MyArrayOfNullIntsProp(IMatcher<int?[]> matcher) {
            WithProperty(()=>PropertyNames.MyArrayOfNullIntsProp,matcher);
            return this;
        }

        public ATestDto MyArrayOfStringsProp(IMatcher<string[]> matcher) {
            WithProperty(()=>PropertyNames.MyArrayOfStringsProp,matcher);
            return this;
        }

        public ATestDto MyBoolProp(bool val) {
            MyBoolProp(ABool.EqualTo(val));
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

        public ATestDto MyDecimalProp(decimal val) {
            MyDecimalProp(ADecimal.EqualTo(val));
            return this;
        }

        public ATestDto MyDecimalProp(IMatcher<decimal?> matcher) {
            WithProperty(()=>PropertyNames.MyDecimalProp,matcher);
            return this;
        }

        public ATestDto MyDictionary(IMatcher<System.Collections.Generic.IDictionary<string,int?>> matcher) {
            WithProperty(()=>PropertyNames.MyDictionary,matcher);
            return this;
        }

        public ATestDto MyDoubleProp(double val) {
            MyDoubleProp(ADouble.EqualTo(val));
            return this;
        }

        public ATestDto MyDoubleProp(IMatcher<double?> matcher) {
            WithProperty(()=>PropertyNames.MyDoubleProp,matcher);
            return this;
        }

        public ATestDto MyDto2(IMatcher<TestFirst.Net.Template.TestDto2> matcher) {
            WithProperty(()=>PropertyNames.MyDto2,matcher);
            return this;
        }

        public ATestDto MyDtoEnmerable(IMatcher<TestFirst.Net.Template.TestDtoEnumerable> matcher) {
            WithProperty(()=>PropertyNames.MyDtoEnmerable,matcher);
            return this;
        }

        public ATestDto MyGuidProp(System.Guid val) {
            MyGuidProp(AGuid.EqualTo(val));
            return this;
        }

        public ATestDto MyGuidProp(IMatcher<System.Guid?> matcher) {
            WithProperty(()=>PropertyNames.MyGuidProp,matcher);
            return this;
        }

        public ATestDto MyInt32Prop(int val) {
            MyInt32Prop(AnInt.EqualTo(val));
            return this;
        }

        public ATestDto MyInt32Prop(IMatcher<int?> matcher) {
            WithProperty(()=>PropertyNames.MyInt32Prop,matcher);
            return this;
        }

        public ATestDto MyInt64Prop(long val) {
            MyInt64Prop(ALong.EqualTo(val));
            return this;
        }

        public ATestDto MyInt64Prop(IMatcher<long?> matcher) {
            WithProperty(()=>PropertyNames.MyInt64Prop,matcher);
            return this;
        }

        public ATestDto MyListOfListsProp(IMatcher<System.Collections.Generic.IEnumerable<System.Collections.Generic.IEnumerable<string>>> matcher) {
            WithProperty(()=>PropertyNames.MyListOfListsProp,matcher);
            return this;
        }

        public ATestDto MyListOfNullIntsProp(IMatcher<System.Collections.Generic.IEnumerable<int?>> matcher) {
            WithProperty(()=>PropertyNames.MyListOfNullIntsProp,matcher);
            return this;
        }

        public ATestDto MyListOfStringsProp(IMatcher<System.Collections.Generic.IEnumerable<string>> matcher) {
            WithProperty(()=>PropertyNames.MyListOfStringsProp,matcher);
            return this;
        }

        public ATestDto MyNullBoolProp(bool? val) {
            MyNullBoolProp(ABool.EqualTo(val));
            return this;
        }

        public ATestDto MyNullBoolProp(IMatcher<bool?> matcher) {
            WithProperty(()=>PropertyNames.MyNullBoolProp,matcher);
            return this;
        }

        public ATestDto MyNullByteProp(IMatcher<byte?> matcher) {
            WithProperty(()=>PropertyNames.MyNullByteProp,matcher);
            return this;
        }

        public ATestDto MyNullCharProp(IMatcher<char?> matcher) {
            WithProperty(()=>PropertyNames.MyNullCharProp,matcher);
            return this;
        }

        public ATestDto MyNullDecimalProp(decimal? val) {
            MyNullDecimalProp(ADecimal.EqualTo(val));
            return this;
        }

        public ATestDto MyNullDecimalProp(IMatcher<decimal?> matcher) {
            WithProperty(()=>PropertyNames.MyNullDecimalProp,matcher);
            return this;
        }

        public ATestDto MyNullDoubleProp(double? val) {
            MyNullDoubleProp(ADouble.EqualTo(val));
            return this;
        }

        public ATestDto MyNullDoubleProp(IMatcher<double?> matcher) {
            WithProperty(()=>PropertyNames.MyNullDoubleProp,matcher);
            return this;
        }

        public ATestDto MyNullFloatProp(float? val) {
            MyNullFloatProp(AFloat.EqualTo(val));
            return this;
        }

        public ATestDto MyNullFloatProp(IMatcher<float?> matcher) {
            WithProperty(()=>PropertyNames.MyNullFloatProp,matcher);
            return this;
        }

        public ATestDto MyNullGuidProp(System.Guid? val) {
            MyNullGuidProp(AGuid.EqualTo(val));
            return this;
        }

        public ATestDto MyNullGuidProp(IMatcher<System.Guid?> matcher) {
            WithProperty(()=>PropertyNames.MyNullGuidProp,matcher);
            return this;
        }

        public ATestDto MyNullIntProp(int? val) {
            MyNullIntProp(AnInt.EqualTo(val));
            return this;
        }

        public ATestDto MyNullIntProp(IMatcher<int?> matcher) {
            WithProperty(()=>PropertyNames.MyNullIntProp,matcher);
            return this;
        }

        public ATestDto MyNullLongProp(long? val) {
            MyNullLongProp(ALong.EqualTo(val));
            return this;
        }

        public ATestDto MyNullLongProp(IMatcher<long?> matcher) {
            WithProperty(()=>PropertyNames.MyNullLongProp,matcher);
            return this;
        }

        public ATestDto MyNullShortProp(short? val) {
            MyNullShortProp(AShort.EqualTo(val));
            return this;
        }

        public ATestDto MyNullShortProp(IMatcher<short?> matcher) {
            WithProperty(()=>PropertyNames.MyNullShortProp,matcher);
            return this;
        }

        public ATestDto MyNullTimeSpan(IMatcher<System.TimeSpan?> matcher) {
            WithProperty(()=>PropertyNames.MyNullTimeSpan,matcher);
            return this;
        }

        public ATestDto MyPrimBoolProp(bool val) {
            MyPrimBoolProp(ABool.EqualTo(val));
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

        public ATestDto MyPrimDecimalProp(decimal val) {
            MyPrimDecimalProp(ADecimal.EqualTo(val));
            return this;
        }

        public ATestDto MyPrimDecimalProp(IMatcher<decimal?> matcher) {
            WithProperty(()=>PropertyNames.MyPrimDecimalProp,matcher);
            return this;
        }

        public ATestDto MyPrimDoubleProp(double val) {
            MyPrimDoubleProp(ADouble.EqualTo(val));
            return this;
        }

        public ATestDto MyPrimDoubleProp(IMatcher<double?> matcher) {
            WithProperty(()=>PropertyNames.MyPrimDoubleProp,matcher);
            return this;
        }

        public ATestDto MyPrimFloatProp(float val) {
            MyPrimFloatProp(AFloat.EqualTo(val));
            return this;
        }

        public ATestDto MyPrimFloatProp(IMatcher<float?> matcher) {
            WithProperty(()=>PropertyNames.MyPrimFloatProp,matcher);
            return this;
        }

        public ATestDto MyPrimIntProp(int val) {
            MyPrimIntProp(AnInt.EqualTo(val));
            return this;
        }

        public ATestDto MyPrimIntProp(IMatcher<int?> matcher) {
            WithProperty(()=>PropertyNames.MyPrimIntProp,matcher);
            return this;
        }

        public ATestDto MyPrimLongProp(long val) {
            MyPrimLongProp(ALong.EqualTo(val));
            return this;
        }

        public ATestDto MyPrimLongProp(IMatcher<long?> matcher) {
            WithProperty(()=>PropertyNames.MyPrimLongProp,matcher);
            return this;
        }

        public ATestDto MyPrimShortProp(short val) {
            MyPrimShortProp(AShort.EqualTo(val));
            return this;
        }

        public ATestDto MyPrimShortProp(IMatcher<short?> matcher) {
            WithProperty(()=>PropertyNames.MyPrimShortProp,matcher);
            return this;
        }

        public ATestDto MyStringProp(string val) {
            MyStringProp(AString.EqualTo(val));
            return this;
        }

        public ATestDto MyStringProp(IMatcher<string> matcher) {
            WithProperty(()=>PropertyNames.MyStringProp,matcher);
            return this;
        }

        public ATestDto MyTimeSpan(IMatcher<System.TimeSpan?> matcher) {
            WithProperty(()=>PropertyNames.MyTimeSpan,matcher);
            return this;
        }

        public ATestDto MyUri(System.Uri val) {
            MyUri(AnUri.EqualTo(val));
            return this;
        }

        public ATestDto MyUri(IMatcher<System.Uri> matcher) {
            WithProperty(()=>PropertyNames.MyUri,matcher);
            return this;
        }
    }
}

namespace TestFirst.Net.Matcher {

    public partial class ATestDto2 : PropertyMatcher<TestFirst.Net.Template.TestDto2>{

        // provide IDE rename and find reference support
        private static readonly TestFirst.Net.Template.TestDto2 PropertyNames = null;


        public static ATestDto2 With(){
                return new ATestDto2();
        }

        public ATestDto2 MyString(string val) {
            MyString(AString.EqualTo(val));
            return this;
        }

        public ATestDto2 MyString(IMatcher<string> matcher) {
            WithProperty(()=>PropertyNames.MyString,matcher);
            return this;
        }
    }
}

namespace TestFirst.Net.Matcher {

    public partial class ATestDtoEnumerable : PropertyMatcher<TestFirst.Net.Template.TestDtoEnumerable>{

        // provide IDE rename and find reference support
        private static readonly TestFirst.Net.Template.TestDtoEnumerable PropertyNames = null;


        public static ATestDtoEnumerable With(){
                return new ATestDtoEnumerable();
        }

        public ATestDtoEnumerable MyProp(string val) {
            MyProp(AString.EqualTo(val));
            return this;
        }

        public ATestDtoEnumerable MyProp(IMatcher<string> matcher) {
            WithProperty(()=>PropertyNames.MyProp,matcher);
            return this;
        }
    }
}

