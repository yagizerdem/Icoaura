using Icoaura.Context;
using Icoaura.Model;
using Icoaura.Util;
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

        private void InitializeAppConfig()
        {
            if (!File.Exists(ApplicationPathContext.AppConfigFilePath))
            {
                var defaultConfig = AppConfig.GetDefault();
                string serializedConfig = JsonUtil.Serialize(defaultConfig);
                File.WriteAllText(ApplicationPathContext.AppConfigFilePath, serializedConfig);
                
                GlobalContext.AppConfig = defaultConfig;
            }
            else
            {
                string serializedConfig = File.ReadAllText(ApplicationPathContext.AppConfigFilePath);
                var appConfig = JsonUtil.Deserialize<AppConfig>(serializedConfig);
                if(appConfig == null)
                {
                    // fix corrupted config
                    appConfig = AppConfig.GetDefault();
                    serializedConfig = JsonUtil.Serialize(appConfig);
                    File.WriteAllText(ApplicationPathContext.AppConfigFilePath, serializedConfig);
                }
                GlobalContext.AppConfig = appConfig;
            }
        }

        public void InitializePaths()
        {
            InitializeRootPath();
            InitializeTempFolder();
            InitializePacksFolder();
            InitializeIconsFolder();
            InitializeAppConfig();
        }

    }
}
