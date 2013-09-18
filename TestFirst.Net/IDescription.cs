using System;
using System.Collections;

namespace TestFirst.Net
{
    public interface IDescription:ISelfDescribing
    {
        /// <summary>
        /// Append a line of text using passed in args to format. 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="args"></param>
        /// <returns>this</returns>
        IDescription Text(string line, params Object[] args);
        
        IDescription Value(Object value); 
        IDescription Value(String label, Object value); 
        
        /// <summary>
        /// Append a value as a child, which is added as a value which is indented one more level
        /// </summary>
        /// <param name="child"></param>
        /// <returns>this</returns>
        IDescription Child(Object child);
        
        /// <summary>
        /// Append the child value and if the child implements ISelfDescribing, then uses it's DesribeTo method
        /// </summary>
        /// <param name="label"></param>
        /// <param name="child"></param>
        /// <returns>this</returns>
        IDescription Child(string label, Object child);
        
        /// <summary>
        /// Append the given values, which are added as values with an additional indentation level
        /// </summary>
        /// <param name="children"></param>
        /// <returns>this</returns>
        IDescription Children(IEnumerable children);
        IDescription Children(string label, IEnumerable children);
    }
}
