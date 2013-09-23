using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TestFirst.Net.Lang;
using TestFirst.Net.Util;

namespace TestFirst.Net
{
    public class Description : IDescription
    {
        public static readonly string Indent = "    ";
        private static readonly Regex ReNewlineReplace = new Regex("\r\n|\n");
        private static readonly Regex ReTabReplace = new Regex("\t");
        private static readonly IAppendListener NullListener = new NullAppendListener();

        private readonly IAppendListener m_listener;
        
        private String m_currentIndent;
        private readonly Stack<string> m_indents = new Stack<string>();

        //don' call append directly unless you also set the m_currentlyOnNewLine flag
        private readonly StringBuilder m_sb = new StringBuilder();
        //since we can't peek at the string builder, maintain a flag to determine if the last character was a newline
        //this is to prevent too many newlines from being added
        private bool m_currentlyOnNewLine = true;

        public interface IAppendListener
        {
            void Append(string text);
            void AppendFormat(string text,params object[] args);
            void AppendLine();
        }

        private class NullAppendListener : IAppendListener 
        {
            public void Append(string text)
            {}

            public void AppendFormat(string text, params object[] args)
            {}

            public void AppendLine()
            {}
        }

        public Description():this(NullListener)
        {}

        public Description(IAppendListener listener)
        {
            PreConditions.AssertNotNull(listener, "TextListener");
            m_currentIndent = null;
            m_listener = listener;
        }

        public static Description With()
        {
            return new Description();
        }

        public IDescription Text(string line, params object[] args)
        {                        
            RequireIndent();
            FormatAppend(line, args);
            return this;
        }

        public IDescription Child(string label, Object child)
        {
            Text(label + ":");
            AppendChild(child);
            return this;
        }

        public IDescription Child(Object child)
        {
            AppendChild(child);
            return this;
        }

        private void AppendChild(Object child)
        {
            IncreaseIndent();
            var childString = ValueToString(child);
            RequireIndent();
            Append(childString);
            DecreaseIndent();
        }

        public IDescription Children(string label, IEnumerable children)
        {
            Text(label + ":");
            return AppendChildren(children);
        }

        public IDescription Children(IEnumerable children)
        {
            return AppendChildren(children);
        }

        private IDescription AppendChildren(IEnumerable children)
        {   
            //inset the actual child values
            IncreaseIndent();
            var primitiveItems = ObjectExtensions.IsEnumerationTypeWithPrimitiveElements(children.GetType());
            if (primitiveItems)
            {
                var first = true;
                foreach (var child in children)
                {
                    if (!first)
                    {
                        Append(",");                        
                    }
                    first = false;
                    AppendValue(child);
                }
            }
            else
            {
                foreach (var child in children)
                {
                    RequireIndent();
                    AppendValue(child);
                }
            }

            DecreaseIndent();
            return this;
        }

        public IDescription Value(String label, object value)
        {
            Text(label + ":");
            var s = ValueToString(value);
            //keep the child text aligned
            if (s.ContainsNewLines())
            {
                RequireIndent();
                Append(s);
            }
            else
            {
                Append(s);
            }

            return this;
        }

        public IDescription Value(object value)
        {
            RequireIndent();
            AppendValue(value);
            return this;
        }

        private void AppendValue(object value)
        {
            Append(ValueToString(value));
        }

        private String ValueToString(Object value)
        {
            if (value == null)
            {
                return "null";
            }
            String s;
            var selfDesc = value as ISelfDescribing;            
            if (selfDesc != null)
            {
                var d = new Description();
                selfDesc.DescribeTo(d);
                s = d.ToString();
            }
            else
            {
                s = value.ToPrettyString();
            }
            //remove any start/end cruft
            var cleaned = ReTabReplace.Replace(s.Trim(),Indent);
            //ensure properly indented
            var indented = ReNewlineReplace.Replace(cleaned, Environment.NewLine + m_currentIndent);
            return indented;
        }        

        private void RequireNewLine()
        {
            if (NewLineRequired())
            {
                NewLine();
            }
        }

        private bool NewLineRequired()
        {
            return (m_sb.Length > 0 && !m_currentlyOnNewLine);
        }

        private IDescription NewLine()
        {
            m_sb.AppendLine();
            m_listener.AppendLine();
            m_currentlyOnNewLine = true;
            return this;
        }

        private void RequireIndent()
        {  
            RequireNewLine();
            if (m_currentIndent != null)
            {
                Append(m_currentIndent);
            }
        }

        private IDescription FormatAppend(string text, params object[] args)
        {
            if (text == null)
            {
                Append("null");
            }
            else if (args == null || args.Length == 0)
            {
                Append(text);
            }
            else
            {
                //convert args 
                try
                {
                    m_sb.AppendFormat(text, args);
                    m_listener.AppendFormat(text,args);
                }
                catch (FormatException e)
                {
                    throw new FormatException("Error formatting text '" + text + "'", e);
                }
            }
            m_currentlyOnNewLine = false;
            return this;
        }

        private void Append(String s)
        {
            m_sb.Append(s);
            m_listener.Append(s);
            m_currentlyOnNewLine = false;
        }

        private void IncreaseIndent()
        {
            m_indents.Push(m_currentIndent);
            m_currentIndent = m_currentIndent + Indent;
        }

        private void DecreaseIndent()
        {            
            m_currentIndent = m_indents.Pop();
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
            return m_sb.ToString();
        }
    }

    internal static class StringExtensions
    {
        internal static bool ContainsNewLines(this String val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return false;
            }
            return val.IndexOf("\n", StringComparison.InvariantCulture) != -1;
        }
    }
}
