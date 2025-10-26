
namespace Icoaura.Util
{
    public class PathUtil
    {

        // %APPDATA%/test/util.exe -> C:/Users/username/AppData/Roaming/test/util.exe
        public static string Resolve(string path)
        {
            List<string> specialFolders = EnvUtil.SpecialFolders;
            List<string> resolvedSpecialFolders = specialFolders
                .Select(path_ => Environment.ExpandEnvironmentVariables(path_))
                .ToList();


            foreach (string speialFolder in specialFolders)
            {
                if (path.StartsWith(speialFolder))
                {
                    int indexOf = specialFolders.IndexOf(speialFolder);
                    string absoluteDirPath = resolvedSpecialFolders[indexOf];
                    path = absoluteDirPath + path.Substring(speialFolder.Length);
                    return path;
                }
            }


            return path;
        }


        // C:/Users/username/AppData/Roaming/test/util.exe -> %APPDATA%/test/util.exe
        public static string UnResolve(string path)
        {
            List<string> specialFolders = EnvUtil.SpecialFolders;
            List<string> resolvedSpecialFolders = specialFolders
                .Select(path_ => Environment.ExpandEnvironmentVariables(path_))
                .ToList();

            string longestPrefix = string.Empty;
            foreach (string resolvedSpecialFolder in resolvedSpecialFolders)
            {
                if (path.StartsWith(resolvedSpecialFolder) && resolvedSpecialFolder.Length > longestPrefix.Length)
                {
                    longestPrefix = resolvedSpecialFolder;
                }
            }

            if (!string.IsNullOrEmpty(longestPrefix))
            {
                int indexOf = resolvedSpecialFolders.IndexOf(longestPrefix);
                string specialFolder = specialFolders[indexOf];
                path = specialFolder + path.Substring(longestPrefix.Length);

                return path;
            }

            return path;
        }


        public static string ConvertToRelativePath(string path)
        {
            List<string> specialFolders = EnvUtil.SpecialFolders;
            List<string> resolvedSpecialFolders = specialFolders
                .Select(path_ => Environment.ExpandEnvironmentVariables(path_))
                .ToList();

            string longestPrefix = string.Empty;
            foreach (string resolvedSpecialFolder in resolvedSpecialFolders)
            {
                if (path.StartsWith(resolvedSpecialFolder) && resolvedSpecialFolder.Length > longestPrefix.Length)
                {
                    longestPrefix = resolvedSpecialFolder;
                }
            }

            if (!string.IsNullOrEmpty(longestPrefix))
            {
                int indexOf = resolvedSpecialFolders.IndexOf(longestPrefix);
                string specialFolder = specialFolders[indexOf];
                path = specialFolder + path.Substring(longestPrefix.Length);

                // add wildcard * after special path
                path = string.Join("\\", path.Split("\\").Select((item, index) =>
                {
                    if (index == 0 || index == path.Split("\\").Length - 1) return item;
                    return "*";
                }).ToList());

                return path;
            }


            return path;
        }

    }

}
