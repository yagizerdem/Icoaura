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

    }
}
