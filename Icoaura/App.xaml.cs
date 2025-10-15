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
     
         
            ImageProcessorService imgServcie = ConfigureServices.Provider.GetRequiredService<ImageProcessorService>();
            var a =  imgServcie.ResizePng("C:\\Users\\yagiz\\Pictures\\Screenshots\\Ekran görüntüsü 2024-06-25 174053 - Kopya.png", 255, 255);
            
        }
    }

}
