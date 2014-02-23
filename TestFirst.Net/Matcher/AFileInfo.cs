using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TestFirst.Net.Matcher {
    public class AFileInfo : PropertyMatcher<FileInfo>{

        private static readonly FileInfo PropertyNames = null;

        public static AFileInfo With() {
            return new AFileInfo();
        }

        public static IMatcher<FileInfo> EqualTo(FileInfo file) {
            return With().FullName(file.FullName);
        }

        public AFileInfo Exists(bool val) {
            WithProperty(()=>PropertyNames.Exists, ABool.EqualTo(val));
            return this;
        }

        public AFileInfo Name(String path) {
            Name(AString.EqualTo(path));
            return this;
        }

        public AFileInfo Name(IMatcher<String> matcher) {
            WithProperty(()=>PropertyNames.Name, matcher);
            return this;
        }

        public AFileInfo FullName(String path) {
            FullName(AString.EqualTo(path));
            return this;
        }

        public AFileInfo FullName(IMatcher<String> matcher) {
            WithProperty(()=>PropertyNames.FullName, matcher);
            return this;
        }

    }
}
