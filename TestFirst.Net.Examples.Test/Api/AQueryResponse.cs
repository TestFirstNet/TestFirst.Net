using System.Collections.Generic;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Examples.Api
{
    public class AQueryResponse<TSelf,T> : PropertyMatcher<QueryResponse<T>> 
    where T : class 
    where TSelf:AQueryResponse<TSelf,T>
    {
        //allow us to access refactor safe proeprty names without resorting to magic strings
        private static readonly QueryResponse<T> PropertyNames = null;

        public TSelf NoResults()
        {
            NumResults(0);
            return Self();
        }

        public TSelf NumResults(int val)
        {
            NumResults(AnInt.EqualTo(val));
            return Self();
        }

        public TSelf NumResults(IMatcher<int?> matcher)
        {
            Results(AList.WithNumItems(matcher));
            return Self();
        }

        public TSelf Result(IMatcher<T> matcher)
        {
            Results(AList.WithOnly(matcher));
            return Self();
        }

        public TSelf Results(IMatcher<IEnumerable<T>> matcher)
        {
            WithProperty(() => PropertyNames.Results, matcher);
            return Self();
        }

        private TSelf Self()
        {
            return (TSelf) this;
        }
    }
}
