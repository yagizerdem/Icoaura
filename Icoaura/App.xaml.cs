using Icoaura.Context;
using Icoaura.Util;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Icoaura
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Initialize();
        }

        public void Initialize()
        {
            InitializePaths(); // set up app data folder strucure
            DIProvider.Initialize(); // set up di container
            EnvUtil.Resolve(); // add special folder paths in windows as magic string to env vars

            CreateLogFile(); // create log folder if not exists
        }

        private void CreateLogFile()
        {
            string[] files = Directory.GetFiles(ApplicationPathContext.AppLogsFolderPath);

            if (files.Length >= GlobalContext.AppConfig.MaxLogCount)
            {
                var oldestFile = files
                    .Select(f => new FileInfo(f))
                    .OrderBy(f => f.CreationTimeUtc)
                    .FirstOrDefault();

                if (oldestFile != null)
                {
                    try
                    {
                        oldestFile.Delete();
                    }
                    catch (System.Exception ex)
                    {
                        // sliently fail
                    }
                }
            }


            string logFilePath = Path.Combine(ApplicationPathContext.AppLogsFolderPath, $"log_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
            File.Create(logFilePath).Dispose();

            GlobalContext.ActiveLogFilePath = logFilePath;
        }

    }

}
