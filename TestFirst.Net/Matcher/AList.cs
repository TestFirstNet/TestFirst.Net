using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TestFirst.Net.Lang;
using TestFirst.Net.Matcher.Internal;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;

namespace TestFirst.Net.Matcher
{
    /// <summary>
    /// Usage:
    /// 
    /// AList
    ///     .InOrder()
    ///         .WithOnly(AString.containing("x"))
    ///         .And(AString.EqualTo("foo"))
    /// 
    /// </summary>
    public static class AList
    {
        public static ListOfMatchBuilder<int> OfInts(){
            return Of<int> ();
        }

        public static ListOfMatchBuilder<DateTime> OfDateTimes(){
            return Of<DateTime> ();
        }

        public static ListOfMatchBuilder<DateTimeOffset> OfDateTimeOffsets(){
            return Of<DateTimeOffset> ();
        }

        public static ListOfMatchBuilder<Guid> OfGuids(){
            return Of<Guid> ();
        }

        public static ListOfMatchBuilder<float> OfFloats(){
            return Of<float> ();
        }

        public static ListOfMatchBuilder<long> OfLongs(){
            return new ListOfMatchBuilder<long> ();
        }

        public static ListOfMatchBuilder<char> OfChars(){
            return Of<char> ();
        }

        public static ListOfMatchBuilder<double> OfDoubles(){
            return Of<double> ();
        }

        public static ListOfMatchBuilder<String> OfStrings(){
            return Of<String> ();
        }

        public static ListOfMatchBuilder<Object> OfObjects(){
            return Of<Object> ();
        }

        public static ListOfMatchBuilder<T> Of<T>(){
            return new ListOfMatchBuilder<T> ();
        }

        /// <summary>
        /// Create a list of matchers, one for each of the given instances using the factory method provided.
        /// 
        /// E,g,
        /// 
        /// AList.InAnyOrder().WithOnly(AList.From(AnInstance.EqualTo,items))
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<IMatcher<T>> From<T>(Func<T,IMatcher<T>> factory, IEnumerable<T> instances)
        {
            return instances.Select(factory.Invoke).ToList();
        }

        /// <summary>
        /// A list with only one element. If you need more use InOrder()/InAnyOrder() ...
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matcher"></param>
        /// <returns></returns>
        public static IListMatcher<T> WithOnly<T>(IMatcher<T> matcher)
        {
            return InOrder().WithOnly(matcher);
        }

        public static IAcceptMoreMatchers<T> Without<T>(IMatcher<T> matcher)
        {
            return new AListNotContains<T>().And(matcher);
        }

        /// <summary>
        /// Return a matcher which allows matchers to match in any order. Currently as soon as a matcher is matched
        /// it is removed from trying to match the remainign items. The matchers are applied in the order they were added
        /// against each item in the list. As soon as a matcher matches a given item both the items and matcher are removed
        /// from further processing. The process then moves on to the next item in the list
        /// </summary>
        /// <returns></returns>
        public static InAnyOrderMatchBuilder InAnyOrder()
        {
            return new InAnyOrderMatchBuilder();
        }

        /// <summary>
        /// Return a matcher which requires all matchers to match in order
        /// </summary>
        /// <returns></returns>
        public static InOrderMatchBuilder InOrder()
        {
            return new InOrderMatchBuilder();
        }

        public static IListMatcher<T> NoItems<T>()
        {
            return new IsEmptyMatcher<T>();
        }

        public static IListMatcher<Object> WithNumItems(int count)
        {
            return WithNumItems(AnInt.EqualTo(count));
        } 

        public static IListMatcher<Object> WithNumItems(IMatcher<int?> matcher)
        {
            return new NumItemsMatcher<Object>(matcher);
        } 

        public class ListOfMatchBuilder<T>{

            /// <summary>
            /// A list with only one element. If you need more use InOrder()/InAnyOrder() ...
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="matcher"></param>
            /// <returns></returns>
            public IListMatcher<T> WithOnly(IMatcher<T> matcher)
            {
                return InOrder().WithOnly(matcher);
            }

            public IAcceptMoreMatchers<T> Without(IMatcher<T> matcher)
            {
                return new AListNotContains<T>().And(matcher);
            }

            /// <summary>
            /// Return a matcher which allows matchers to match in any order. Currently as soon as a matcher is matched
            /// it is removed from trying to match the remainign items. The matchers are applied in the order they were added
            /// against each item in the list. As soon as a matcher matches a given item both the items and matcher are removed
            /// from further processing. The process then moves on to the next item in the list
            /// </summary>
            /// <returns></returns>
            public InAnyOrderMatchBuilder InAnyOrder()
            {
                return new InAnyOrderMatchBuilder();
            }

            /// <summary>
            /// Return a matcher which requires all matchers to match in order
            /// </summary>
            /// <returns></returns>
            public InOrderMatchBuilder InOrder()
            {
                return new InOrderMatchBuilder();
            }

            public IListMatcher<T> NoItems()
            {
                return new IsEmptyMatcher<T>();
            }

            public IListMatcher<T> WithNumItems(int count)
            {
                return WithNumItems(AnInt.EqualTo(count));
            } 

            public IListMatcher<T> WithNumItems(IMatcher<int?> matcher)
            {
                return new NumItemsMatcher<T>(matcher);
            } 
        }

        /// <summary>
        /// For items matchers where item order is important
        /// </summary>
        // ReSharper disable PossibleMultipleEnumeration
        public class InOrderMatchBuilder
        {
           /// <summary>
           /// Return a matcher which requires all items to match
           /// </summary>
           public IAcceptMoreMatchers<T> WithOnly<T>(params IMatcher<T>[] itemMatchers)
           {
                return OfType<T>().WithOnly(itemMatchers);
           }
           
           [Obsolete("Use WithOnlyValues(...) instead")]
           public IAcceptMoreMatchers<int?> WithOnly(params int?[] values)
           {
                return OfType<int?>().WithOnlyValues(values);
           }

           [Obsolete("Use WithOnlyValues(...) instead")]
           public IAcceptMoreMatchers<String> WithOnly(params String[] values)
           {
                return OfType<String>().WithOnlyValues(values);
           }
           /// <summary>
           /// Return a matcher which requires all items to match
           /// </summary>
           public IAcceptMoreMatchers<T> WithOnly<T,TVal>(Func<TVal,IMatcher<T>> valueToMatchFunc, IEnumerable<TVal> values)
           {    
                return OfType<T>().WithOnly(valueToMatchFunc, values);
           }

           /// <summary>
           /// Return a matcher which requires all items to match
           /// </summary>
           public IAcceptMoreMatchers<T> WithOnly<T>(IEnumerable<IMatcher<T>> itemMatchers)
           {
                return OfType<T>().WithOnly(itemMatchers);
           }

            public IAcceptMoreMatchers<T> WithAtLeastValues<T>(params T[] values)
            {
                return OfType<T>().WithAtLeastValues(values);
            }

            [Obsolete("Use WithAtLeastValues(...) instead")]
            public IAcceptMoreMatchers<String> WithAtLeast(params String[] values)
            {
                return OfType<String>().WithAtLeastValues(values);
            }

            [Obsolete("Use WithAtLeastValues(...) instead")]
            public IAcceptMoreMatchers<int?> WithAtLeast(params int?[] values)
            {
                return OfType<int?>().WithAtLeastValues(values);
            }

           /// <summary>
           /// Return a matcher which requires only that all matchers match once, so additional non matched items are allowed
           /// </summary>
           public IAcceptMoreMatchers<T> WithAtLeast<T>(params IMatcher<T>[] itemMatchers)
           {
                return OfType<T>().WithAtLeast(itemMatchers);
           }

           /// <summary>
           /// Return a matcher which requires only that all matchers match once, so additional non matched items are allowed
           /// </summary>
           public IAcceptMoreMatchers<T> WithAtLeast<T,TVal>(Func<TVal,IMatcher<T>> valueToMatchFunc, IEnumerable<TVal> values)
           {               
                return OfType<T>().WithAtLeast(valueToMatchFunc,values);
           }

           /// <summary>
           /// Return a matcher which requires only that all matchers match once, so additional non matched items are allowed
           /// </summary>
           public IAcceptMoreMatchers<T> WithAtLeast<T>(IEnumerable<IMatcher<T>> itemMatchers)
           {
                return OfType<T>().WithAtLeast (itemMatchers);
           }

           private InOrderMatchBuilder<T> OfType<T>(){
                return new InOrderMatchBuilder<T> ();
           }
        }

        /// <summary>
        /// For items matchers where item order is important
        /// </summary>
        // ReSharper disable PossibleMultipleEnumeration
        public class InOrderMatchBuilder<T>
        {
            /// <summary>
            /// Return a matcher which requires all items to match
            /// </summary>
            public IAcceptMoreMatchers<T> WithOnly(params IMatcher<T>[] itemMatchers)
            {
                return WithOnly((IEnumerable<IMatcher<T>>) itemMatchers);
            }

            public IAcceptMoreMatchers<T> WithOnlyValues(params T[] values)
            {
                return WithOnly(AList.From(val=>AnInstance.EqualTo(val), values));
            }

            /// <summary>
            /// Return a matcher which requires only that all matchers match once, so additional non matched items are allowed
            /// 
            /// Usage:
            /// 
            ///     AList.OfStrings().InOrder().WithOnly(x=>AString.Containing("x"),listOfStrings);
            /// 
            /// </summary>
            public IAcceptMoreMatchers<T> WithOnly<TVal>(Func<TVal,IMatcher<T>> valueToMatchFunc, IEnumerable<TVal> values)
            {
                return WithOnly(Matchers.MatchersFromValues(valueToMatchFunc,values));
            }

            /// <summary>
            /// Return a matcher which requires all items to match
            /// </summary>
            public IAcceptMoreMatchers<T> WithOnly(IEnumerable<IMatcher<T>> itemMatchers)
            {
                PreConditions.AssertNotNull(itemMatchers, "itemMatchers");
                return new AListInOrderWithOnlyMatcher<T>(itemMatchers);
            }

            public IAcceptMoreMatchers<T> WithAtLeastValues(params T[] values)
            {
                return WithAtLeast(AList.From(val => AnInstance.EqualTo(val), values));
            }

            /// <summary>
            /// Return a matcher which requires only that all matchers match once, so additional non matched items are allowed
            /// </summary>
            public IAcceptMoreMatchers<T> WithAtLeast(params IMatcher<T>[] itemMatchers)
            {
                return WithAtLeast((IEnumerable<IMatcher<T>>)itemMatchers);
            }

            /// <summary>
            /// Return a matcher which requires only that all matchers match once, so additional non matched items are allowed
            /// </summary>
            public IAcceptMoreMatchers<T> WithAtLeast<TVal>(Func<TVal,IMatcher<T>> valueToMatchFunc, IEnumerable<TVal> values)
            {               
                return WithAtLeast(Matchers.MatchersFromValues(valueToMatchFunc,values));
            }

            /// <summary>
            /// Return a matcher which requires only that all matchers match once, so additional non matched items are allowed
            /// </summary>
            public IAcceptMoreMatchers<T> WithAtLeast(IEnumerable<IMatcher<T>> itemMatchers)
            {
                PreConditions.AssertNotNull(itemMatchers, "itemMatchers");
                return new AListInOrderAtLeast<T>(itemMatchers);
            }
        }
        public class InAnyOrderMatchBuilder
        {
           /// <summary>
           /// Return a matcher which requires all items to match
           /// </summary>
           public IAcceptMoreMatchers<T> WithOnly<T>()
           {
                return ofType<T> ().WithOnly();
           }

           [Obsolete("Use WithOnly(...) instead")]
           public IAcceptMoreMatchers<string> WithOnly(params string[] values)
           {
                return ofType<String> ().WithOnlyValues(values);
           }

           [Obsolete("Use WithOnly(...) instead")]
           public IAcceptMoreMatchers<int?> WithOnly(params int?[] values)
           {
                return ofType<int?> ().WithOnlyValues(values);
           }

           public IAcceptMoreMatchers<T> WithOnlyValues<T>(params T[] values)
           {
                return ofType<T> ().WithOnlyValues(values);
           }

           /// <summary>
           /// Return a matcher which requires all items to match
           /// </summary>
           public IAcceptMoreMatchers<T> WithOnly<T>(params IMatcher<T>[] itemMatchers)
           {
                return ofType<T> ().WithOnly(itemMatchers);
           }

           /// <summary>
           /// Return a matcher which requires all items to match
           /// </summary>
           public IAcceptMoreMatchers<T> WithOnly<T,TVal>(Func<TVal,IMatcher<T>> valueToMatchFunc, IEnumerable<TVal> values)
           {               
                return ofType<T> ().WithOnly(valueToMatchFunc,values);
           }

           /// <summary>
           /// Return a matcher which requires all items to match
           /// </summary>
           public IAcceptMoreMatchers<T> WithOnly<T>(IEnumerable<IMatcher<T>> itemMatchers)
           {
                return ofType<T> ().WithOnly(itemMatchers);
           }

           [Obsolete("Use WithAtLeast(...) instead")]
           public IAcceptMoreMatchers<string> WithAtLeast(params string[] values)
           {
                return ofType<String> ().WithAtLeastValues(values);
           }

           [Obsolete("Use WithAtLeast(...) instead")]
           public IAcceptMoreMatchers<int?> WithAtLeast(params int?[] values)
           {
                return ofType<int?> ().WithAtLeastValues(values);
           }
           
           public IAcceptMoreMatchers<T> WithAtLeastValues<T>(params T[] values)
           {
                return ofType<T> ().WithAtLeastValues(values);
           }
           /// <summary>
           /// Return a matcher which requires only that all matchers match once, so additional non matched items are allowed
           /// </summary>
           public IAcceptMoreMatchers<T> WithAtLeast<T>(params IMatcher<T>[] itemMatchers)
           {
                return ofType<T> ().WithAtLeast (itemMatchers);
           }

           /// <summary>
           /// Return a matcher which requires only that all matchers match once, so additional non matched items are allowed
           /// </summary>
           public IAcceptMoreMatchers<T> WithAtLeast<T,TVal>(Func<TVal,IMatcher<T>> valueToMatchFunc, IEnumerable<TVal> values)
           {    
                return ofType<T> ().WithAtLeast (valueToMatchFunc,values);
           }

           /// <summary>
           /// Return a matcher which requires only that all matchers match once, so additional non matched items are allowed
           /// </summary>
           public IAcceptMoreMatchers<T> WithAtLeast<T>(IEnumerable<IMatcher<T>> itemMatchers)
           {
                return ofType<T> ().WithAtLeast (itemMatchers);
           }

            private InAnyOrderMatchBuilder<T> ofType<T>(){
                return new InAnyOrderMatchBuilder<T> ();
            }

        }

        public class InAnyOrderMatchBuilder<T>
        {
            /// <summary>
            /// Return a matcher which requires all items to match
            /// </summary>
            public IAcceptMoreMatchers<T> WithOnly()
            {
                return new AListInAnyOrderWithOnly<T>();
            }

            public IAcceptMoreMatchers<T> WithOnlyValues(params T[] values)
            {
                return WithOnly(AList.From(val => AnInstance.EqualTo(val), values));
            }

            /// <summary>
            /// Return a matcher which requires all items to match
            /// </summary>
            public IAcceptMoreMatchers<T> WithOnly(params IMatcher<T>[] itemMatchers)
            {
                return WithOnly((IEnumerable<IMatcher<T>>) itemMatchers);
            }

            /// <summary>
            /// Return a matcher which requires all items to match
            /// </summary>
            public IAcceptMoreMatchers<T> WithOnly<TVal>(Func<TVal,IMatcher<T>> valueToMatchFunc, IEnumerable<TVal> values)
            {               
                return WithOnly(Matchers.MatchersFromValues(valueToMatchFunc,values));
            }

            /// <summary>
            /// Return a matcher which requires all items to match
            /// </summary>
            public IAcceptMoreMatchers<T> WithOnly(IEnumerable<IMatcher<T>> itemMatchers)
            {
                PreConditions.AssertNotNull(itemMatchers, "itemMatchers");
                return new AListInAnyOrderWithOnly<T>(itemMatchers);
            }

            public IAcceptMoreMatchers<T> WithAtLeastValues(params T[] values)
            {
                return WithAtLeast(AList.From(val => AnInstance.EqualTo(val), values));
            }

            /// <summary>
            /// Return a matcher which requires only that all matchers match once, so additional non matched items are allowed
            /// </summary>
            public IAcceptMoreMatchers<T> WithAtLeast(params IMatcher<T>[] itemMatchers)
            {
                return WithAtLeast((IEnumerable<IMatcher<T>>)itemMatchers);
            }

            /// <summary>
            /// Return a matcher which requires only that all matchers match once, so additional non matched items are allowed
            /// </summary>
            public IAcceptMoreMatchers<T> WithAtLeast<TVal>(Func<TVal,IMatcher<T>> valueToMatchFunc, IEnumerable<TVal> values)
            {               
                return WithAtLeast(Matchers.MatchersFromValues(valueToMatchFunc,values));
            }

            /// <summary>
            /// Return a matcher which requires only that all matchers match once, so additional non matched items are allowed
            /// </summary>
            public IAcceptMoreMatchers<T> WithAtLeast(IEnumerable<IMatcher<T>> itemMatchers)
            {
                PreConditions.AssertNotNull(itemMatchers, "itemMatchers");
                return new AListInAnyOrderWithAtLeast<T>(itemMatchers);
            }


        }

         // ReSharper enable PossibleMultipleEnumeration

        public interface IListMatcher<in T> :IMatcher<IEnumerable<T>>,IMatcher<IEnumerable>,IMatcher
        {
        }

        /// <summary>
        /// A list matcher which can have additional matchers added
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public interface IAcceptMoreMatchers<T> : IListMatcher<T>
        {
            IAcceptMoreMatchers<T> And(IMatcher<T> itemMatcher);
        }

        internal abstract class AbstractListMatcher<T>: AbstractMatcher<IEnumerable<T>>,IListMatcher<T>, IProvidePrettyTypeName
        {
            private readonly String m_shortName;
            internal AbstractListMatcher(String shortName)
            {
                m_shortName = shortName;
            }
        
            public string GetPrettyTypeName()
            {
                return "AList." + m_shortName + "(IEnumerable<" + ProvidePrettyTypeName.GetPrettyTypeNameFor<T>() + ">)";
            }

            public override bool Matches(IEnumerable<T> actual, IMatchDiagnostics diagnostics)
            {
                return Matches(actual as IEnumerable, diagnostics);
            }

            protected override bool isValidType(Object actual){
                Type itemType = GetItemType (actual);
                return itemType != null && typeof(T).IsAssignableFrom(itemType);
            }

            protected override IEnumerable<T> wrap(Object actual){
                //Mono seems to have issue casting IList<T> to IEnumerable<Nullable<T>> when T is a value type
                Type itemType = GetItemType (actual);
                if (itemType.IsValueType) {
                    return new ValueTypeEnumerableWrapper<T>((IEnumerable)actual);
                }
                return (IEnumerable<T>)actual;
            }

            private Type GetItemType(object someCollection)
            {
                var type = someCollection.GetType();
                var ienum = type.GetInterface(typeof(IEnumerable<>).Name);
                return ienum != null
                    ? ienum.GetGenericArguments()[0]
                        : null;
            }

            internal static IList AsEfficientList(IEnumerable items)
            {
                if (items == null)
                {
                    return null;
                }
                var list = items as IList;
                if (list != null)
                {
                    return list;
                }
                list = new List<Object>(15);
                foreach (var item in items)
                {
                    list.Add(item);
                }
                return list;
            }

            public abstract bool Matches(IEnumerable actual, IMatchDiagnostics diagnostics);

        }

        private class IsEmptyMatcher<T>:NumItemsMatcher<T>
        {
            public IsEmptyMatcher():base(AnInt.EqualTo(0))
            {}
        }

        private class NumItemsMatcher<T>:AbstractListMatcher<T>
        {
            private readonly IMatcher<int?> m_countMatcher;

            public NumItemsMatcher(IMatcher<int?> countMatcher):base("NumItems")
            {
                m_countMatcher = countMatcher;
            }

            public override bool Matches(IEnumerable instance, IMatchDiagnostics diag)
            {
                var enummerator = instance.GetEnumerator();
                int count = 0;
                while (enummerator.MoveNext())
                {
                    count++;
                }
                return diag.TryMatch(count, "count", m_countMatcher);
            }

        }

        private class ValueTypeEnumerableWrapper<T> : System.Collections.Generic.IEnumerable<T>,IEnumerable
        {
            private readonly IEnumerable actual;

            public ValueTypeEnumerableWrapper(IEnumerable actual){
                this.actual = actual;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return new ValueTypeEnumerator(actual.GetEnumerator());
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            public override string ToString ()
            {
                return actual.ToString();
            }

            class ValueTypeEnumerator : IEnumerator<T> {
                private readonly IEnumerator actual;

                public ValueTypeEnumerator(IEnumerator actual){
                    this.actual = actual;
                }

                object System.Collections.IEnumerator.Current
                {
                    get { return this.Current; }
                }

                public T Current {
                    get { return (T)actual.Current; }
                }

                public bool MoveNext (){
                    return actual.MoveNext();
                }

                public void Reset (){
                    actual.Reset ();
                }

                public void Dispose (){
                    IDisposable d = actual as IDisposable;
                    if (d != null) {
                        d.Dispose ();
                    }
                }

            }
        }


    }
}
