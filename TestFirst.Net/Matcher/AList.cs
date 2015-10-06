using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TestFirst.Net.Lang;
using TestFirst.Net.Matcher.Internal;

namespace TestFirst.Net.Matcher
{
    /// <summary>
    /// Usage:
    /// <para>
    /// AList
    ///     .InOrder()
    ///         .WithOnly(AString.containing("x"))
    ///         .And(AString.EqualTo("foo"))
    /// </para>
    /// </summary>
    public static class AList
    {
        // ReSharper enable PossibleMultipleEnumeration
        public interface IListMatcher<in T> : IMatcher<IEnumerable<T>>, IMatcher<IEnumerable>, IMatcher
        {
        }

        /// <summary>
        /// A list matcher which can have additional matchers added
        /// </summary>
        /// <typeparam name="T">The type being matched</typeparam>
        public interface IAcceptMoreMatchers<T> : IListMatcher<T>
        {
            IAcceptMoreMatchers<T> And(IMatcher<T> itemMatcher);
        }

        public static ListOfMatchBuilder<int> OfInts()
        {
            return Of<int>();
        }

        public static ListOfMatchBuilder<DateTime> OfDateTimes()
        {
            return Of<DateTime>();
        }

        public static ListOfMatchBuilder<DateTimeOffset> OfDateTimeOffsets()
        {
            return Of<DateTimeOffset>();
        }

        public static ListOfMatchBuilder<Guid> OfGuids()
        {
            return Of<Guid>();
        }

        public static ListOfMatchBuilder<float> OfFloats()
        {
            return Of<float>();
        }

        public static ListOfMatchBuilder<long> OfLongs()
        {
            return new ListOfMatchBuilder<long>();
        }

        public static ListOfMatchBuilder<char> OfChars()
        {
            return Of<char>();
        }

        public static ListOfMatchBuilder<double> OfDoubles()
        {
            return Of<double>();
        }

        public static ListOfMatchBuilder<string> OfStrings()
        {
            return Of<string>();
        }

        public static ListOfMatchBuilder<object> OfObjects()
        {
            return Of<object>();
        }

        public static ListOfMatchBuilder<T> Of<T>()
        {
            return new ListOfMatchBuilder<T>();
        }

        /// <summary>
        /// Create a list of matchers, one for each of the given instances using the factory method provided.
        /// <para>
        /// E.g.
        /// </para>
        /// <para>
        /// AList.InAnyOrder().WithOnly(AList.From(AnInstance.EqualTo, items))
        /// </para>
        /// </summary>
        /// <returns>A list of matchers, one per instance</returns>
        /// <param name="factory">The factory method which will create matchers for each instance in the list</param>
        /// <param name="instances">The list of items to create matchers from</param>
        /// <typeparam name="T">The type of element in the list</typeparam>
        public static IEnumerable<IMatcher<T>> From<T>(Func<T, IMatcher<T>> factory, IEnumerable<T> instances)
        {
            return instances.Select(factory.Invoke).ToList();
        }

        /// <summary>
        /// A list with only one element. If you need more use InOrder()/InAnyOrder() ...
        /// </summary>
        /// <typeparam name="T">The type being matched</typeparam>
        /// <param name="matcher">The matcher</param>
        /// <returns>A list matcher</returns>
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
        /// it is removed from trying to match the remaining items. The matchers are applied in the order they were added
        /// against each item in the list. As soon as a matcher matches a given item both the items and matcher are removed
        /// from further processing. The process then moves on to the next item in the list
        /// </summary>
        /// <returns>A builder for matching in any order</returns>
        public static InAnyOrderMatchBuilder InAnyOrder()
        {
            return new InAnyOrderMatchBuilder();
        }

        /// <summary>
        /// Return a matcher which requires all matchers to match in order
        /// </summary>
        /// <returns>A builder for matching in order</returns>
        public static InOrderMatchBuilder InOrder()
        {
            return new InOrderMatchBuilder();
        }

        public static IListMatcher<T> NoItems<T>()
        {
            return new IsEmptyMatcher<T>();
        }

        public static IListMatcher<object> WithNumItems(int count)
        {
            return WithNumItems(AnInt.EqualTo(count));
        } 

        public static IListMatcher<object> WithNumItems(IMatcher<int?> matcher)
        {
            return new NumItemsMatcher<object>(matcher);
        } 

        public class ListOfMatchBuilder<T>
        {
            /// <summary>
            /// A list with only one element. If you need more use InOrder()/InAnyOrder() ...
            /// </summary>
            /// <param name="matcher">The matcher</param>
            /// <returns>A list matcher</returns>
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
            /// it is removed from trying to match the remaining items. The matchers are applied in the order they were added
            /// against each item in the list. As soon as a matcher matches a given item both the items and matcher are removed
            /// from further processing. The process then moves on to the next item in the list
            /// </summary>
            /// <returns>A builder for matching in any order</returns>
            public InAnyOrderMatchBuilder InAnyOrder()
            {
                return new InAnyOrderMatchBuilder();
            }

            /// <summary>
            /// Return a matcher which requires all matchers to match in order
            /// </summary>
            /// <returns>A builder for matching in order</returns>
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
           /// <param name="itemMatchers">The list of matchers</param>
           /// <returns>Builder for continuing to create the matcher</returns>
           /// <typeparam name="T">The type being matched</typeparam>
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
           public IAcceptMoreMatchers<string> WithOnly(params string[] values)
           {
                return OfType<string>().WithOnlyValues(values);
           }

           public IAcceptMoreMatchers<T> WithOnlyValues<T>(params T[] values)
           {
                return OfType<T>().WithOnlyValues(values);
           }

           /// <summary>
           /// Return a matcher which requires all items to match
           /// </summary>
           /// <param name="valueToMatchFunc">The factory function to create matchers</param>
           /// <param name="values">The list of vales</param>
           /// <returns>The matcher builder</returns>
           /// <typeparam name="T">The type of the item being matched</typeparam>
           /// <typeparam name="TVal">The type of value being enumerated</typeparam>
           public IAcceptMoreMatchers<T> WithOnly<T, TVal>(Func<TVal, IMatcher<T>> valueToMatchFunc, IEnumerable<TVal> values)
           {    
                return OfType<T>().WithOnly(valueToMatchFunc, values);
           }

           /// <summary>
           /// Return a matcher which requires all items to match
           /// </summary>
           /// <param name="itemMatchers">The list of matchers</param>
           /// <returns>The matcher builder</returns>
           /// <typeparam name="T">The type being matched</typeparam>
           public IAcceptMoreMatchers<T> WithOnly<T>(IEnumerable<IMatcher<T>> itemMatchers)
           {
                return OfType<T>().WithOnly(itemMatchers);
           }

           public IAcceptMoreMatchers<T> WithAtLeastValues<T>(params T[] values)
           {
               return OfType<T>().WithAtLeastValues(values);
           }

           [Obsolete("Use WithAtLeastValues(...) instead")]
           public IAcceptMoreMatchers<string> WithAtLeast(params string[] values)
           {
               return OfType<string>().WithAtLeastValues(values);
           }

           [Obsolete("Use WithAtLeastValues(...) instead")]
           public IAcceptMoreMatchers<int?> WithAtLeast(params int?[] values)
           {
               return OfType<int?>().WithAtLeastValues(values);
           }

           /// <summary>
           /// Return a matcher which requires only that all matchers match once, so additional non matched items are allowed
           /// </summary>
           /// <param name="itemMatchers">THe list of matchers</param>
           /// <returns>The matcher builder</returns>
           /// <typeparam name="T">The type being matched</typeparam>
           public IAcceptMoreMatchers<T> WithAtLeast<T>(params IMatcher<T>[] itemMatchers)
           {
                return OfType<T>().WithAtLeast(itemMatchers);
           }

           /// <summary>
           /// Return a matcher which requires only that all matchers match once, so additional non matched items are allowed
           /// </summary>
           /// <param name="valueToMatchFunc">Function to get matcher for a given value</param>
           /// <param name="values">The list of values</param>
           /// <returns>The matcher builder</returns>
           /// <typeparam name="T">The type being matched</typeparam>
           /// <typeparam name="TVal">The type being enumerated</typeparam>
           public IAcceptMoreMatchers<T> WithAtLeast<T, TVal>(Func<TVal, IMatcher<T>> valueToMatchFunc, IEnumerable<TVal> values)
           {               
                return OfType<T>().WithAtLeast(valueToMatchFunc, values);
           }

           /// <summary>
           /// Return a matcher which requires only that all matchers match once, so additional non matched items are allowed
           /// </summary>
           /// <param name="itemMatchers">The list of matchers</param>
           /// <typeparam name="T">The type being matched</typeparam>
           /// <returns>The matcher builder</returns>
           public IAcceptMoreMatchers<T> WithAtLeast<T>(IEnumerable<IMatcher<T>> itemMatchers)
           {
                return OfType<T>().WithAtLeast(itemMatchers);
           }

           private InOrderMatchBuilder<T> OfType<T>()
        {
                return new InOrderMatchBuilder<T>();
           }
        }

        /// <summary>
        /// For items matchers where item order is important
        /// </summary>
        /// <typeparam name="T">The type being matched</typeparam>
        // ReSharper disable PossibleMultipleEnumeration
        public class InOrderMatchBuilder<T>
        {
            /// <summary>
            /// Return a matcher which requires all items to match
            /// </summary>
            /// <param name="itemMatchers">The list of matchers</param>
            /// <returns>The matcher builder</returns>
            public IAcceptMoreMatchers<T> WithOnly(params IMatcher<T>[] itemMatchers)
            {
                return WithOnly((IEnumerable<IMatcher<T>>)itemMatchers);
            }

            public IAcceptMoreMatchers<T> WithOnlyValues(params T[] values)
            {
                return WithOnly(AList.From(val => AnInstance.EqualTo(val), values));
            }

            /// <summary>
            /// Return a matcher which requires only that all matchers match once, so additional non matched items are allowed
            /// <para>
            /// Usage:
            /// </para>
            /// <para>
            ///     AList.OfStrings().InOrder().WithOnly(x => AString.Containing("x"), listOfStrings);
            /// </para>
            /// </summary>
            /// <param name="valueToMatchFunc">The factory to create a matcher for each value</param>
            /// <param name="values">The list of values</param>
            /// <typeparam name="TVal">The type being enumerated</typeparam>
            /// <returns>The matcher builder</returns>
            public IAcceptMoreMatchers<T> WithOnly<TVal>(Func<TVal, IMatcher<T>> valueToMatchFunc, IEnumerable<TVal> values)
            {
                return WithOnly(Matchers.MatchersFromValues(valueToMatchFunc, values));
            }

            /// <summary>
            /// Return a matcher which requires all items to match
            /// </summary>
            /// <param name="itemMatchers">The list of matchers</param>
            /// <returns>The matcher builder</returns>
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
            /// <param name="itemMatchers">The list of matchers</param>
            /// <returns>The matcher builder</returns>
            public IAcceptMoreMatchers<T> WithAtLeast(params IMatcher<T>[] itemMatchers)
            {
                return WithAtLeast((IEnumerable<IMatcher<T>>)itemMatchers);
            }

            /// <summary>
            /// Return a matcher which requires only that all matchers match once, so additional non matched items are allowed
            /// </summary>
            /// <param name="valueToMatchFunc">The factory to create a matcher for each value</param>
            /// <param name="values">The list of values</param>
            /// <typeparam name="TVal">The type being enumerated</typeparam>
            /// <returns>The matcher builder</returns>
            public IAcceptMoreMatchers<T> WithAtLeast<TVal>(Func<TVal, IMatcher<T>> valueToMatchFunc, IEnumerable<TVal> values)
            {               
                return WithAtLeast(Matchers.MatchersFromValues(valueToMatchFunc, values));
            }

            /// <summary>
            /// Return a matcher which requires only that all matchers match once, so additional non matched items are allowed
            /// </summary>
            /// <param name="itemMatchers">The list of matchers</param>
            /// <returns>The matcher builder</returns>
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
           /// <returns>The matcher builder</returns>
           /// <typeparam name="T">The type being matched</typeparam>
           public IAcceptMoreMatchers<T> WithOnly<T>()
           {
                return OfType<T>().WithOnly();
           }

           [Obsolete("Use WithOnly(...) instead")]
           public IAcceptMoreMatchers<string> WithOnly(params string[] values)
           {
                return OfType<string>().WithOnlyValues(values);
           }

           [Obsolete("Use WithOnly(...) instead")]
           public IAcceptMoreMatchers<int?> WithOnly(params int?[] values)
           {
                return OfType<int?>().WithOnlyValues(values);
           }

           public IAcceptMoreMatchers<T> WithOnlyValues<T>(params T[] values)
           {
                return OfType<T>().WithOnlyValues(values);
           }

           /// <summary>
           /// Return a matcher which requires all items to match
           /// </summary>
           /// <param name="itemMatchers">The list of matchers</param>
           /// <returns>The matcher builder</returns>
           /// <typeparam name="T">The type being matched</typeparam>
           public IAcceptMoreMatchers<T> WithOnly<T>(params IMatcher<T>[] itemMatchers)
           {
                return OfType<T>().WithOnly(itemMatchers);
           }

           /// <summary>
           /// Return a matcher which requires all items to match
           /// </summary>
           /// <param name="valueToMatchFunc">The factory to create a matcher for each value</param>
           /// <param name="values">The list of values</param>
           /// <typeparam name="T">The type being matched</typeparam>
           /// <typeparam name="TVal">The type being enumerated</typeparam>
           /// <returns>The matcher builder</returns>
           public IAcceptMoreMatchers<T> WithOnly<T, TVal>(Func<TVal, IMatcher<T>> valueToMatchFunc, IEnumerable<TVal> values)
           {               
                return OfType<T>().WithOnly(valueToMatchFunc, values);
           }

           /// <summary>
           /// Return a matcher which requires all items to match
           /// </summary>
           /// <param name="itemMatchers">The list of matchers</param>
           /// <typeparam name="T">The type being matched</typeparam>
           /// <returns>The matcher builder</returns>
           public IAcceptMoreMatchers<T> WithOnly<T>(IEnumerable<IMatcher<T>> itemMatchers)
           {
                return OfType<T>().WithOnly(itemMatchers);
           }

           [Obsolete("Use WithAtLeast(...) instead")]
           public IAcceptMoreMatchers<string> WithAtLeast(params string[] values)
           {
                return OfType<string>().WithAtLeastValues(values);
           }

           [Obsolete("Use WithAtLeast(...) instead")]
           public IAcceptMoreMatchers<int?> WithAtLeast(params int?[] values)
           {
                return OfType<int?>().WithAtLeastValues(values);
           }
           
           public IAcceptMoreMatchers<T> WithAtLeastValues<T>(params T[] values)
           {
                return OfType<T>().WithAtLeastValues(values);
           }

           /// <summary>
           /// Return a matcher which requires only that all matchers match once, so additional non matched items are allowed
           /// </summary>
           /// <param name="itemMatchers">The list of matchers</param>
           /// <typeparam name="T">The type being matched</typeparam>
           /// <returns>The matcher builder</returns>
           public IAcceptMoreMatchers<T> WithAtLeast<T>(params IMatcher<T>[] itemMatchers)
           {
                return OfType<T>().WithAtLeast(itemMatchers);
           }

           /// <summary>
           /// Return a matcher which requires only that all matchers match once, so additional non matched items are allowed
           /// </summary>
           /// <param name="valueToMatchFunc">The factory to create a matcher for each value</param>
           /// <param name="values">The list of values</param>
           /// <typeparam name="T">The type being matched</typeparam>
           /// <typeparam name="TVal">The type being enumerated</typeparam>
           /// <returns>The matcher builder</returns>
           public IAcceptMoreMatchers<T> WithAtLeast<T, TVal>(Func<TVal, IMatcher<T>> valueToMatchFunc, IEnumerable<TVal> values)
           {    
                return OfType<T>().WithAtLeast(valueToMatchFunc, values);
           }

           /// <summary>
           /// Return a matcher which requires only that all matchers match once, so additional non matched items are allowed
           /// </summary>
           /// <param name="itemMatchers">The list of matchers</param>
           /// <typeparam name="T">The type being matched</typeparam>
           /// <returns>The matcher builder</returns>
           public IAcceptMoreMatchers<T> WithAtLeast<T>(IEnumerable<IMatcher<T>> itemMatchers)
           {
                return OfType<T>().WithAtLeast(itemMatchers);
           }

            private InAnyOrderMatchBuilder<T> OfType<T>()
            {
                return new InAnyOrderMatchBuilder<T>();
            }
        }

        public class InAnyOrderMatchBuilder<T>
        {
            /// <summary>
            /// Return a matcher which requires all items to match
            /// </summary>
            /// <returns>The matcher builder</returns>
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
            /// <param name="itemMatchers">The list of matchers</param>
            /// <returns>The matcher builder</returns>
            public IAcceptMoreMatchers<T> WithOnly(params IMatcher<T>[] itemMatchers)
            {
                return WithOnly((IEnumerable<IMatcher<T>>)itemMatchers);
            }

            /// <summary>
            /// Return a matcher which requires all items to match
            /// </summary>
            /// <param name="valueToMatchFunc">The factory to create a matcher for each value</param>
            /// <param name="values">The list of values</param>
            /// <typeparam name="TVal">The type being enumerated</typeparam>
            /// <returns>The matcher builder</returns>
            public IAcceptMoreMatchers<T> WithOnly<TVal>(Func<TVal, IMatcher<T>> valueToMatchFunc, IEnumerable<TVal> values)
            {
                return WithOnly(Matchers.MatchersFromValues(valueToMatchFunc, values));
            }

            /// <summary>
            /// Return a matcher which requires all items to match
            /// </summary>
            /// <param name="itemMatchers">The list of matchers</param>
            /// <returns>The matcher builder</returns>
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
            /// <param name="itemMatchers">The list of matchers</param>
            /// <returns>The matcher builder</returns>
            public IAcceptMoreMatchers<T> WithAtLeast(params IMatcher<T>[] itemMatchers)
            {
                return WithAtLeast((IEnumerable<IMatcher<T>>)itemMatchers);
            }

            /// <summary>
            /// Return a matcher which requires only that all matchers match once, so additional non matched items are allowed
            /// </summary>
            /// <param name="valueToMatchFunc">The factory to create a matcher for each value</param>
            /// <param name="values">The list of values</param>
            /// <returns>The matcher builder</returns>
            /// <typeparam name="TVal">The type being matched</typeparam>
            public IAcceptMoreMatchers<T> WithAtLeast<TVal>(Func<TVal, IMatcher<T>> valueToMatchFunc, IEnumerable<TVal> values)
            {
                return WithAtLeast(Matchers.MatchersFromValues(valueToMatchFunc, values));
            }

            /// <summary>
            /// Return a matcher which requires only that all matchers match once, so additional non matched items are allowed
            /// </summary>
            /// <param name="itemMatchers">The list of matchers</param>
            /// <returns>The matcher builder</returns>
            public IAcceptMoreMatchers<T> WithAtLeast(IEnumerable<IMatcher<T>> itemMatchers)
            {
                PreConditions.AssertNotNull(itemMatchers, "itemMatchers");
                return new AListInAnyOrderWithAtLeast<T>(itemMatchers);
            }
        }

        internal abstract class AbstractListMatcher<T> : AbstractMatcher<IEnumerable<T>>, IListMatcher<T>, IProvidePrettyTypeName
        {
            private readonly string m_shortName;

            internal AbstractListMatcher(string shortName)
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

            public abstract bool Matches(IEnumerable actual, IMatchDiagnostics diagnostics);

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
                list = new List<object>(15);
                foreach (var item in items)
                {
                    list.Add(item);
                }
                return list;
            }

            protected override bool IsValidType(object actual)
            {
                Type itemType = GetItemType(actual);
                return itemType != null && typeof(T).IsAssignableFrom(itemType);
            }

            protected override IEnumerable<T> Wrap(object actual)
            {
                // Mono seems to have issue casting IList<T> to IEnumerable<Nullable<T>> when T is a value type
                Type itemType = GetItemType(actual);
                if (itemType.IsValueType)
                {
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
        }

        private class IsEmptyMatcher<T> : NumItemsMatcher<T>
        {
            public IsEmptyMatcher()
                : base(AnInt.EqualTo(0))
            {
            }
        }

        private class NumItemsMatcher<T> : AbstractListMatcher<T>
        {
            private readonly IMatcher<int?> m_countMatcher;

            public NumItemsMatcher(IMatcher<int?> countMatcher)
                : base("NumItems")
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

        private class ValueTypeEnumerableWrapper<T> : IEnumerable<T>, IEnumerable
        {
            private readonly IEnumerable actual;

            public ValueTypeEnumerableWrapper(IEnumerable actual)
            {
                this.actual = actual;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return new ValueTypeEnumerator(actual.GetEnumerator());
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public override string ToString()
            {
                return actual.ToString();
            }

            internal class ValueTypeEnumerator : IEnumerator<T> 
            {
                private readonly IEnumerator m_actual;

                public ValueTypeEnumerator(IEnumerator actual)
                {
                    m_actual = actual;
                }

                object IEnumerator.Current
                {
                    get { return Current; }
                }

                public T Current 
                {
                    get { return (T)m_actual.Current; }
                }

                public bool MoveNext()
                {
                    return m_actual.MoveNext();
                }

                public void Reset()
                {
                    m_actual.Reset();
                }

                public void Dispose()
                {
                    var d = m_actual as IDisposable;
                    if (d != null) 
                    {
                        d.Dispose();
                    }
                }
            }
        }
    }
}
