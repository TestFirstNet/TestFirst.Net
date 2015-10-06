using System;
using System.Collections;

namespace TestFirst.Net
{
    public interface IDescription : ISelfDescribing
    {
        /// <summary>
        /// Gets a value indicating whether this is a null description. That is, a description which ignores all input
        /// <para>
        /// Useful if some diagnostic information could be expensive to generate and we only want to 
        /// generate this on failure
        /// </para>
        /// </summary>
        /// <returns><c>true</c> if this instance is a null description; otherwise, <c>false</c>.</returns>
        bool IsNull 
        {
            get;
        }

        /// <summary>
        /// Append a line of text using passed in args to format. 
        /// </summary>
        /// <param name="line">The line of text to append</param>
        /// <param name="args">The arguments for formatting</param>
        /// <returns>the thing being described</returns>
        IDescription Text(string line, params object[] args);
        
        IDescription Value(object value); 
        IDescription Value(string label, object value); 
        
        /// <summary>
        /// Append a value as a child, which is added as a value which is indented one more level
        /// </summary>
        /// <param name="child">the child to describe</param>
        /// <returns>the thing being described</returns>
        IDescription Child(object child);
        
        /// <summary>
        /// Append the child value and if the child implements ISelfDescribing, then uses it's DescribeTo method
        /// </summary>
        /// <param name="label">the label to use</param>
        /// <param name="child">the child to append</param>
        /// <returns>the thing being described</returns>
        IDescription Child(string label, object child);
        
        /// <summary>
        /// Append the given values, which are added as values with an additional indentation level
        /// </summary>
        /// <param name="children">the list of children to append</param>
        /// <returns>the thing being described</returns>
        IDescription Children(IEnumerable children);
        IDescription Children(string label, IEnumerable children);
    }
}
