using System.IO;

namespace Icoaura.Util
{
    class EnvUtil
    {
        public static List<string> SpecialFolders = new()
{
    "%DESKTOP%",
    "%DESKTOP_COMMON%",
    "%DOCUMENTS%",
    "%DOWNLOADS%",
    "%PICTURES%",
    "%MUSIC%",
    "%VIDEOS%",
    "%APPDATA%",
    "%LOCALAPPDATA%",
    "%PROGRAMDATA%",
    "%PROGRAMFILES%",
    "%PROGRAMFILESX86%",
    "%PUBLIC%",
    "%USERPROFILE%",
    "%STARTMENU%",
    "%STARTUP%"
};

        public static void Resolve()
        {
            var envVariables = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["%DESKTOP%"] = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                ["%DOCUMENTS%"] = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                ["%DOWNLOADS%"] = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"),
                ["%PICTURES%"] = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                ["%MUSIC%"] = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                ["%VIDEOS%"] = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),

                ["%APPDATA%"] = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                ["%LOCALAPPDATA%"] = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                ["%PROGRAMDATA%"] = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),

                ["%PROGRAMFILES%"] = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                ["%PROGRAMFILESX86%"] = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),

                ["%PUBLIC%"] = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments),
                ["%USERPROFILE%"] = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),

                ["%STARTMENU%"] = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
                ["%STARTUP%"] = Environment.GetFolderPath(Environment.SpecialFolder.Startup),

                ["%DESKTOP_COMMON%"] = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory),
            };

            if (string.IsNullOrEmpty(envVariables["%DESKTOP_COMMON%"]))
            {
                envVariables["%DESKTOP_COMMON%"] = @"C:\Users\Public\Desktop";
            }

            foreach (var kvp in envVariables)
            {
                if (!string.IsNullOrEmpty(kvp.Value))
                {
                    Environment.SetEnvironmentVariable(kvp.Key.Trim('%'), kvp.Value, EnvironmentVariableTarget.Process);
                }
            }
        }

    }
}
