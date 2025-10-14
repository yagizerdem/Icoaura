using System.IO;
using System.Windows;
using Util;

namespace Icoaura
{
    public partial class App: Application
    {
        // this class must run before app start
        // creat app related folder and resolve absoltute path and save in static variables

        private void InitializeBaseDir()
        {
            string rootDirName = SD.BaseFolderName;
            string appData = Environment.ExpandEnvironmentVariables("%APPDATA%");
            string absoluteBasePath = Path.Combine(appData, rootDirName);

            if (!Directory.Exists(absoluteBasePath))
            {
                Directory.CreateDirectory(absoluteBasePath);
            }
            SD.BaseFolderAbsolutePath = absoluteBasePath;
        }

        private void InitializeIconsDir()
        {
            string iconsDirName = SD.IconsFolderName;
            string absoluteIconsPath = Path.Combine(SD.BaseFolderAbsolutePath, iconsDirName);
            if (!Directory.Exists(absoluteIconsPath))
            {
                Directory.CreateDirectory(absoluteIconsPath);
            }
            SD.IconsFolderAbsolutePath = absoluteIconsPath;
        }

        private void InitilizePackDir()
        {
            string packDirName = SD.PackFolderName;
            string absolutePackPath = Path.Combine(SD.BaseFolderAbsolutePath, packDirName);
            if (!Directory.Exists(absolutePackPath))
            {
                Directory.CreateDirectory(absolutePackPath);
            }
            SD.PackFolderAbsolutePath = absolutePackPath;
        }

        private void InitializeTempDir()
        {
            string tempDirName = SD.TempFolderName;
            string absoluteTempPath = Path.Combine(SD.BaseFolderAbsolutePath, tempDirName);
            if (!Directory.Exists(absoluteTempPath))
            {
                Directory.CreateDirectory(absoluteTempPath);
            }
            SD.TempFolderAbsolutePath = absoluteTempPath;
        }

        public void InitializePaths()
        {
            InitializeBaseDir(); // base dir should resolve first

            InitializeIconsDir();
            InitilizePackDir();
            InitializeTempDir();
        }

        public void SetUpEnv()
        {
            var specialFolders = new Dictionary<string, Environment.SpecialFolder>
            {
                ["DESKTOP"] = Environment.SpecialFolder.Desktop,
                ["DOCUMENTS"] = Environment.SpecialFolder.MyDocuments,
                ["DOWNLOADS"] = Environment.SpecialFolder.UserProfile, 
                ["PICTURES"] = Environment.SpecialFolder.MyPictures,
                ["MUSIC"] = Environment.SpecialFolder.MyMusic,
                ["VIDEOS"] = Environment.SpecialFolder.MyVideos,
                ["APPDATA"] = Environment.SpecialFolder.ApplicationData,
                ["LOCALAPPDATA"] = Environment.SpecialFolder.LocalApplicationData,
                ["PROGRAMDATA"] = Environment.SpecialFolder.CommonApplicationData,
                ["PROGRAMFILES"] = Environment.SpecialFolder.ProgramFiles,
                ["PROGRAMFILESX86"] = Environment.SpecialFolder.ProgramFilesX86,
                ["PUBLIC"] = Environment.SpecialFolder.CommonDocuments,
                ["USERPROFILE"] = Environment.SpecialFolder.UserProfile,
                ["TEMP"] = Environment.SpecialFolder.InternetCache, 
                ["STARTMENU"] = Environment.SpecialFolder.StartMenu,
                ["STARTUP"] = Environment.SpecialFolder.Startup,
                ["DESKTOP_COMMON"] = Environment.SpecialFolder.CommonDesktopDirectory,
            };

            foreach (var pair in specialFolders)
            {
                try
                {
                    string path = Environment.GetFolderPath(pair.Value);
                    if (!string.IsNullOrWhiteSpace(path))
                        Environment.SetEnvironmentVariable(pair.Key, path, EnvironmentVariableTarget.Process);
                }
                catch
                {
                    // some paths may not be available on all systems, ignore errors
                }
            }

            string downloadsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Downloads"
            );

            if (Directory.Exists(downloadsPath))
                Environment.SetEnvironmentVariable("DOWNLOADS", downloadsPath, EnvironmentVariableTarget.Process);
        }
    }
}
