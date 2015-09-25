using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TestFirst.Net.Matcher.Internal;

namespace TestFirst.Net.Matcher
{
    /// <summary>
    /// A matcher which collects and applies a number of matchers
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectMatcher<T> : AbstractMatcher<T>
    {
        //Matchers for the whole poco. Run these the order they were added as callers may want to make certain assertions before others
        //to ease diagnostics
        private readonly IList<IMatcher<T>> m_pocoMatchers = new List<IMatcher<T>>();

        public override bool Matches(T actual, IMatchDiagnostics diagnostics)
        {
            if (actual == null)
            {
                diagnostics.MisMatched("Expected non null instance");
                return false;
            }
            foreach (var matcher in m_pocoMatchers)
            {
                if (!diagnostics.TryMatch(actual, matcher))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Add a matcher which will match on the result of the given extractor
        /// </summary>
        /// <typeparam name="TType">the type of the value extracted from the instance matched against (i.e. the proeprty type)</typeparam>
        /// <param name="valueDescription">the label given to the expression in case of a non match (e.g. "Foo.Bar.GetSomeValue()")</param>
        /// <param name="valueExtractor">the function which extracts the given value (e.g. (T instance)=>instance.Foo.Bar.GetSomeValue())</param>
        /// <param name="valueMatcher">the matcher used to validate the extracted value</param>
        /// <returns></returns>
        protected ObjectMatcher<T> WithMatcher<TType>(string valueDescription, Func<T, TType> valueExtractor, IMatcher<TType> valueMatcher)
        {
            var instanceMatcher = Matchers.Function((T instance, IMatchDiagnostics diag) =>
                {
                    TType valueFromInstance = valueExtractor.Invoke(instance);
                    return diag.TryMatch(valueFromInstance, valueMatcher);
                },
                "'" + valueDescription + "' is " + valueMatcher
            );
            WithMatcher(instanceMatcher);
            return this;
        }
        
        protected ObjectMatcher<T> WithMatcher(IMatcher<T> matcher)
        {
            m_pocoMatchers.Add(matcher);
            return this;
        }

        public override void DescribeTo(IDescription desc)
        {
            var type = typeof (T);
            desc.Text("A {0}.{1} where", type.Namespace,  type.Name);
            desc.Children(m_pocoMatchers);
        }
    }
}