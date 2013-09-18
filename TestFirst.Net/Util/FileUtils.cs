using System;
using System.IO;
using System.Security.AccessControl;

namespace TestFirst.Net.Util
{
    /// <summary>
    /// Various useful test utility methods around files
    /// </summary>
    public static class FileUtils
    {
        public static string NewRandomDirectoryPath()
        {
            //bug In Path, where if you end with a '\\' it will just return '\\', ignoring the paths before it
            var dir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName(), "RandomDir\\");
            return Path.GetFullPath(dir);
        }
        
        public static string NewRandomDirectoryPathWithName(string dirName)
        {
            return Path.Combine(Path.GetTempPath(), Path.GetRandomFileName(), dirName + "\\");
        }

        public static String AddPermissionForPathAndCurrentIdentity(FileSystemRights right, string path)
        {
            return AddPermissionForPathAndIdentity(right, path, Environment.UserName);
        }

        public static String AddPermissionForPathAndIdentity(FileSystemRights right, string path, string identity)
        {
            var addRight = new FileSystemAccessRule(identity, right, AccessControlType.Allow);
            AddAccessRuleForPathAndIdentity(addRight, path, identity);
            return path;
        }

        public static String DenyPermissionForPathAndCurrentIdentity(FileSystemRights right, string path)
        {
            return DenyPermissionForPathAndIdentity(right, path, Environment.UserName);
        }

        public static String DenyPermissionForPathAndIdentity(FileSystemRights right, string path, string identity)
        {
            var removeRight = new FileSystemAccessRule(identity, right, AccessControlType.Deny);
            AddAccessRuleForPathAndIdentity(removeRight, path, identity);
            return path;
        }


        private static void AddAccessRuleForPathAndIdentity(FileSystemAccessRule rule, string path, string identity)
        {
            //be very careful before changing this! You could cause people to loose access to their important OS folders!
            IdiotCheckPath(path);
            try
            {
                FileSecurity security = File.GetAccessControl(path);
                security.AddAccessRule(rule);
                File.SetAccessControl(path, security);
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("Error adding a rule '{0}:{1}:{2}' on path '{3}'", rule.AccessControlType, rule.FileSystemRights, rule.IdentityReference.Value, path), e);
            }
        }

        /// <summary>
        /// Check the path to ensure we don't deny permission to something like c:, c:\windows etc, \\monaco etc..
        /// Ensure there are at least 3 levels deep, and that it is no any of the 'standard' folders
        /// </summary>
        /// <param name="path"></param>
        private static void IdiotCheckPath(string path)
        {
            if(String.IsNullOrWhiteSpace(path))
            {
                throw new Exception("Careful! Path is empty so not changing permissions on root drive");
            }

            if (!Directory.Exists(path) && !File.Exists(path))
            {
                throw new Exception(String.Format("Can't change permission on non existent file or directory '{0}'", path));
            }
            var cleanPath = Path.GetFullPath(path);
            cleanPath = cleanPath.Trim();
            cleanPath = cleanPath.Replace("\\","/");
            cleanPath = cleanPath.ToLower();

            var parts = cleanPath.Split('/');
            if ((cleanPath.StartsWith("//") && parts.Length < 4) || (parts.Length < 3))
            {
                throw new Exception(String.Format("Careful! Path seems rather short, changing perms on a top level folder could cause issues. Not changing permissions on '{0}'", path));
            }

            if(cleanPath.Contains("/windows/") || cleanPath.Contains("/program files"))
            {
                throw new Exception("Careful! Path seems to be an important OS directory, not changing permissions");
            }

            var homePath = GetHomeDirectory().Replace("\\", "/").ToLower();
            if (cleanPath.Equals(homePath))
            {
                throw new Exception("Careful! Path seems to be your home directory, not changing permissions");
            }
            
        }

        private static string GetHomeDirectory()
        {
            string homePath = 
                (Environment.OSVersion.Platform == PlatformID.Unix ||Environment.OSVersion.Platform == PlatformID.MacOSX)
                ? Environment.GetEnvironmentVariable("HOME")
                : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            return Path.GetFullPath(homePath);
        }
    }
}
