using Icoaura.Util;
using System.Configuration;
using System.Data;
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

        private void Initialize()
        {
            InitializePaths(); // set up app data folder strucure
            DIProvider.Initialize(); // set up di container
            EnvUtil.Resolve(); // add special folder paths in windows as magic string to env vars
        }



    }

}
