using Icoaura.Context;
using System.IO;
using System.Windows;

namespace Icoaura
{
    partial class App: Application
    {


        public void InitializeRootPath()
        {
            if (!Directory.Exists(ApplicationPathContext.AppRootPath))
            {
                GlobalContext.IsFirstRun = true;
                Directory.CreateDirectory(ApplicationPathContext.AppRootPath);
            }
        }

        private void InitializeTempFolder()
        {
            if (!Directory.Exists(ApplicationPathContext.AppTempFolderPath))
            {
                Directory.CreateDirectory(ApplicationPathContext.AppTempFolderPath);
            }
        }

        private void InitializePacksFolder()
        {
            if (!Directory.Exists(ApplicationPathContext.AppPacksFolderPath))
            {
                Directory.CreateDirectory(ApplicationPathContext.AppPacksFolderPath);
            }

        }

        private void InitializeIconsFolder()
        {
            if (!Directory.Exists(ApplicationPathContext.AppIconsFolderPath))
            {
                Directory.CreateDirectory(ApplicationPathContext.AppIconsFolderPath);
            }

        }

        public void InitializePaths()
        {
            InitializeRootPath();
            InitializeTempFolder();
            InitializePacksFolder();
            InitializeIconsFolder();
        }

    }
}
