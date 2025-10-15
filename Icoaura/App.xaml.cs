using Microsoft.Extensions.DependencyInjection;
using Model.Configs;
using Model.DTO;
using Model.ResponseTypes;
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
            string base64 = fs.GetBase64("C:\\Users\\yagiz\\Pictures\\Screenshots\\Ekran görüntüsü 2023-11-13 193940.png").Data;
            



            PackService packService = ConfigureServices.Provider.GetRequiredService<PackService>();

            //ServiceResponse<object> reponse =  packService.ApplyCoverImage("4b3d0712-00cf-4491-88e7-6780bdfddbd8", base64);



        }
    }

}
