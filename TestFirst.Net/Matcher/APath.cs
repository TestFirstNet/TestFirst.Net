using System;
using System.IO;

namespace TestFirst.Net.Matcher
{
    public static class APath
    {
        public static IMatcher<string> IgnoringSlashType(string path)
        {
            path = path.Replace("\\", "/");
            return WithForwardSlashes(AString.EqualTo(path));
        }

        public static IMatcher<string> WithForwardSlashes(IMatcher<string> matcher)
        {            
            return Matchers.Function((string actual,IMatchDiagnostics diag) =>
            {
                if (actual != null)
                {
                    actual = actual.Replace("\\", "/");
                }
                return matcher.Matches(actual, diag);
            },
            "ignoring slash type, " + matcher
          );
        }

        public static IMatcher<string> FileOrDirectoryExists()
        {
            return Matchers.Function((string actualPath, IMatchDiagnostics diag) =>
            {
                if( File.Exists(actualPath) || Directory.Exists(actualPath))
                {
                    diag.Matched(Description.With()
                        .Value("expect", "file or directory to exist")
                        .Value("path", actualPath)
                        .Value("exists", true));
                    return true;
                }
                else
                {
                    diag.MisMatched(Description.With()
                        .Value("expected", "file or directory to exist")
                        .Value("path", actualPath)
                        .Value("exists", false));
                    return false;
                }
            },
            "FileOrDirectoryExists exists"
          );
        }

        public static IMatcher<string> FileExists()
        {
            return Matchers.Function((string actualPath, IMatchDiagnostics diag) =>
            {
                if (File.Exists(actualPath))
                {
                    diag.Matched(Description.With()
                        .Value("expect", "file to exist")
                        .Value("path", actualPath)
                        .Value("exists", true));
                    return true;
                }
                else
                {
                    diag.MisMatched(Description.With()
                        .Value("expect", "file to exist")
                        .Value("path", actualPath)
                        .Value("exists", false));
                    return false;
                }
            },
            "FileExists exists"
          );
        }

        public static IMatcher<string> CanReadFile()
        {
            return Matchers.Function((string actualPath, IMatchDiagnostics diag) =>
            {
                if (File.Exists(actualPath))
                {
                    diag.Matched(Description.With().Value("FileExists", true));

                    try
                    {
                        using (var fs = File.OpenRead(actualPath))
                        {
                            if (fs.CanRead)
                            {
                                diag.Matched(Description.With().Value("CanReadFile", true).Value("path", actualPath).Value("exists", true));
                                return true;
                            }
                            else
                            {
                                diag.MisMatched(Description.With().Value("CanReadFile", false).Value("path", actualPath).Value("exists", true));
                                return false;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        diag.MisMatched(Description.With().Value("CanReadFile", false).Value("path", actualPath).Value("error", e.Message));
                        return false;
                    }
                }
                diag.MisMatched(Description.With().Value("FileExists", false).Value("path", actualPath).Value("CanRead", false));
                return false;
            },
            "CanReadFile"
          );
        } 
    }
}
