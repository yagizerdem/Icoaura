using Microsoft.Extensions.DependencyInjection;
using Service;
using System.Windows;

namespace Icoaura
{
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            this.InitializePaths();
            this.SetUpEnv(); 
         
            // add servcies to DI
            ConfigureServices.Configure();

            LnkService lnkService = ConfigureServices.Provider.GetRequiredService<LnkService>();
            var response = lnkService.GetLnkAbsolutePathsUnderDesktop();
     
            
        }
    }

}
