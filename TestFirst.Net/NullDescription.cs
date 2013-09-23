using System;
using System.Collections;

namespace TestFirst.Net
{
    /// <summary>
    /// A description which does not collect anything. Useful when speed is required and the disgnostic results of
    /// matching is not required
    /// </summary>
    public class NullDescription : IDescription
    {
        /// <summary>
        /// Keep a single instance around to remove the need to create it over and over when a single shared
        /// instance exihibits the exact same behaviour (provided nothing locks on this)
        /// </summary>
        public static readonly NullDescription Instance = new NullDescription();

        public NullDescription()
        {}

        public static NullDescription With()
        {
            return new NullDescription();
        }

        public IDescription Text(string line, params object[] args)
        {                        
            return this;
        }

        public IDescription Child(string label, Object child)
        {
            return this;
        }

        public IDescription Child(Object child)
        {
            return this;
        }

        public IDescription Children(string label, IEnumerable children)
        {
            return this;
        }

        public IDescription Children(IEnumerable children)
        {
            return this;
        }

        public IDescription Value(String label, object value)
        {
            return this;
        }

        public IDescription Value(object value)
        {
            return this;
        }

        public void DescribeTo(IDescription desc)
        {
            if (desc != this)//prevent accidental self recursion
            {
                desc.Value(ToString());
            }            
        }
        
        public override string ToString()
        {
            return GetType().Name;
        }
    }

}
