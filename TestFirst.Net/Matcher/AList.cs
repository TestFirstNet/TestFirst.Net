using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TestFirst.Net.Lang;
using TestFirst.Net.Matcher.Internal;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace TestFirst.Net.Matcher
{
    public static class AList
    {
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
               return WithOnly((IEnumerable<IMatcher<T>>) itemMatchers);
           }

           public IAcceptMoreMatchers<string> WithOnly(params string[] values)
           {
               return WithOnly(AList.From(val=>AString.EqualTo(val), values));
           }

           public IAcceptMoreMatchers<int?> WithOnly(params int?[] values)
           {
               return WithOnly(AList.From(val => AnInt.EqualTo(val), values));
           }

           /// <summary>
           /// Return a matcher which requires all items to match
           /// </summary>
           public IAcceptMoreMatchers<T> WithOnly<T,TVal>(Func<TVal,IMatcher<T>> valueToMatchFunc, IEnumerable<TVal> values)
           {               
               return WithOnly(Matchers.MatchersFromValues(valueToMatchFunc,values));
           }

           /// <summary>
           /// Return a matcher which requires all items to match
           /// </summary>
           public IAcceptMoreMatchers<T> WithOnly<T>(IEnumerable<IMatcher<T>> itemMatchers)
           {
               PreConditions.AssertNotNull(itemMatchers, "itemMatchers");
               return new AListInOrderWithOnlyMatcher<T>(itemMatchers);
           }

           public IAcceptMoreMatchers<string> WithAtLeast(params string[] values)
           {
               return WithAtLeast(AList.From(val => AString.EqualTo(val), values));
           }

           public IAcceptMoreMatchers<int?> WithAtLeast(params int?[] values)
           {
               return WithAtLeast(AList.From(val => AnInt.EqualTo(val), values));
           }

           /// <summary>
           /// Return a matcher which requires only that all matchers match once, so additional non matched items are allowed
           /// </summary>
           public IAcceptMoreMatchers<T> WithAtLeast<T>(params IMatcher<T>[] itemMatchers)
           {
               return WithAtLeast((IEnumerable<IMatcher<T>>)itemMatchers);
           }

           /// <summary>
           /// Return a matcher which requires only that all matchers match once, so additional non matched items are allowed
           /// </summary>
           public IAcceptMoreMatchers<T> WithAtLeast<T,TVal>(Func<TVal,IMatcher<T>> valueToMatchFunc, IEnumerable<TVal> values)
           {               
               return WithAtLeast(Matchers.MatchersFromValues(valueToMatchFunc,values));
           }

           /// <summary>
           /// Return a matcher which requires only that all matchers match once, so additional non matched items are allowed
           /// </summary>
           public IAcceptMoreMatchers<T> WithAtLeast<T>(IEnumerable<IMatcher<T>> itemMatchers)
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
               return new AListInAnyOrderWithOnly<T>();
           }

           public IAcceptMoreMatchers<string> WithOnly(params string[] values)
           {
               return WithOnly(AList.From(val => AString.EqualTo(val), values));
           }

           public IAcceptMoreMatchers<int?> WithOnly(params int?[] values)
           {
               return WithOnly(AList.From(val => AnInt.EqualTo(val), values));
           }

           /// <summary>
           /// Return a matcher which requires all items to match
           /// </summary>
           public IAcceptMoreMatchers<T> WithOnly<T>(params IMatcher<T>[] itemMatchers)
           {
               return WithOnly((IEnumerable<IMatcher<T>>) itemMatchers);
           }

           /// <summary>
           /// Return a matcher which requires all items to match
           /// </summary>
           public IAcceptMoreMatchers<T> WithOnly<T,TVal>(Func<TVal,IMatcher<T>> valueToMatchFunc, IEnumerable<TVal> values)
           {               
               return WithOnly(Matchers.MatchersFromValues(valueToMatchFunc,values));
           }

           /// <summary>
           /// Return a matcher which requires all items to match
           /// </summary>
           public IAcceptMoreMatchers<T> WithOnly<T>(IEnumerable<IMatcher<T>> itemMatchers)
           {
               PreConditions.AssertNotNull(itemMatchers, "itemMatchers");
               return new AListInAnyOrderWithOnly<T>(itemMatchers);
           }

           public IAcceptMoreMatchers<string> WithAtLeast(params string[] values)
           {
               return WithAtLeast(AList.From(val => AString.EqualTo(val), values));
           }

           public IAcceptMoreMatchers<int?> WithAtLeast(params int?[] values)
           {
               return WithAtLeast(AList.From(val => AnInt.EqualTo(val), values));
           }

           /// <summary>
           /// Return a matcher which requires only that all matchers match once, so additional non matched items are allowed
           /// </summary>
           public IAcceptMoreMatchers<T> WithAtLeast<T>(params IMatcher<T>[] itemMatchers)
           {
               return WithAtLeast((IEnumerable<IMatcher<T>>)itemMatchers);
           }

           /// <summary>
           /// Return a matcher which requires only that all matchers match once, so additional non matched items are allowed
           /// </summary>
           public IAcceptMoreMatchers<T> WithAtLeast<T,TVal>(Func<TVal,IMatcher<T>> valueToMatchFunc, IEnumerable<TVal> values)
           {               
               return WithAtLeast(Matchers.MatchersFromValues(valueToMatchFunc,values));
           }

           /// <summary>
           /// Return a matcher which requires only that all matchers match once, so additional non matched items are allowed
           /// </summary>
           public IAcceptMoreMatchers<T> WithAtLeast<T>(IEnumerable<IMatcher<T>> itemMatchers)
           {
                PreConditions.AssertNotNull(itemMatchers, "itemMatchers");
                return new AListInAnyOrderWithAtLeast<T>(itemMatchers);
           }


        }
         // ReSharper enable PossibleMultipleEnumeration

        public interface IListMatcher<in T> :IMatcher<IEnumerable<T>>,IMatcher<IEnumerable>
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
