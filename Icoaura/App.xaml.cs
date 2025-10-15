using Microsoft.Extensions.DependencyInjection;
using Model.DTO;
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


            FileService fs = ConfigureServices.Provider.GetRequiredService<FileService>();
            string base64 = fs.GetBase64("C:\\Users\\yagiz\\Pictures\\Screenshots\\Ekran Görüntüsü (1).png").Data;
            



            PackService packService = ConfigureServices.Provider.GetRequiredService<PackService>();
 
     


        }
    }

}
