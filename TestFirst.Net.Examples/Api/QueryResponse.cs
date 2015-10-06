using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TestFirst.Net.Examples.Api
{
    public abstract class QueryResponse<T> : IEnumerable<T>
    {        
        /// <summary>
        /// Gets a single results from the list of results, failing if there are no results or more than 1
        /// </summary>
        public T Result 
        {
            get 
            {
                if (Results == null || Results.Count == 0)
                {
                    throw new InvalidOperationException("No Results");
                }
                if (Results.Count > 1)
                {
                    throw new InvalidOperationException("More than 1 results");
                }
                return Results.First();
            }
        }

        /// <summary>
        /// Gets or sets the list of returned results
        /// </summary>
        public List<T> Results { get; set; }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Results.GetEnumerator();
        }
    }
}
