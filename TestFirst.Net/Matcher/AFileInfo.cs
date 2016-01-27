using System;
using System.IO;

namespace TestFirst.Net.Matcher 
{
    public class AFileInfo : PropertyMatcher<FileInfo>
    {
        private static readonly FileInfo PropertyNames = null;

        public static AFileInfo With() 
        {
            return new AFileInfo();
        }

        public static IMatcher<FileInfo> EqualTo(FileInfo file) 
        {
            return With().FullName(file.FullName);
        }

        public AFileInfo Exists(bool val) 
        {
            WithProperty(() => PropertyNames.Exists, ABool.EqualTo(val));
            return this;
        }

        public AFileInfo Name(string path) 
        {
            Name(AString.EqualTo(path));
            return this;
        }

        public AFileInfo Name(IMatcher<string> matcher) 
        {
            WithProperty(() => PropertyNames.Name, matcher);
            return this;
        }

        public AFileInfo FullName(string path) 
        {
            FullName(AString.EqualTo(path));
            return this;
        }

        public AFileInfo FullName(IMatcher<string> matcher) 
        {
            WithProperty(() => PropertyNames.FullName, matcher);
            return this;
        }
    }
}
