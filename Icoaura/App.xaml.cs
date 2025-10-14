using System.Windows;

namespace Icoaura
{
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            this.InitializePaths();
         
            // add servcies to DI
            ConfigureServices.Configure();
        }
    }

}
