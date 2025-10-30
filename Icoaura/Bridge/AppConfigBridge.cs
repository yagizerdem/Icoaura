using Icoaura.Controller;
using Icoaura.Model;
using Icoaura.Util;
using Microsoft.Extensions.DependencyInjection;

namespace Icoaura.Bridge
{
    [System.Runtime.InteropServices.ClassInterface(System.Runtime.InteropServices.ClassInterfaceType.AutoDual)]
    [System.Runtime.InteropServices.ComVisible(true)]
    public class AppConfigBridge
    {
        private readonly AppConfigController _appConfigController;
        public AppConfigBridge()
        {
            _appConfigController = DIProvider.Provider.GetRequiredService<AppConfigController>();
        }
    
        public string GetAppConfig()
        {
            ApiResponse<AppConfig> response = _appConfigController.ReadAppConfig();
            string serialized = JsonUtil.Serialize(response);
            return serialized;
        }

        public string WriteAppConfig(string configSerialized)
        {
            AppConfig config = JsonUtil.Deserialize<AppConfig>(configSerialized) ?? AppConfig.GetDefault();
            ApiResponse<AppConfig> response = _appConfigController.WriteAppConfig(config);
            string serialized = JsonUtil.Serialize(response);
            return serialized;
        }

        public string ExportAppConfig()
        {
            ApiResponse<object> response = _appConfigController.ExportAppConfig();
            string serialized = JsonUtil.Serialize(response);
            return serialized;
        }

        public string ImportAppConfig(string path)
        {
            ApiResponse<AppConfig> response = _appConfigController.ImportAppConfig(path);
            string serialized = JsonUtil.Serialize(response);
            return serialized;
        }

    }
}
