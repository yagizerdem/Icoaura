using Icoaura.Controller;
using Icoaura.Model;
using Icoaura.Util;
using Microsoft.Extensions.DependencyInjection;

namespace Icoaura.Bridge
{

    [System.Runtime.InteropServices.ClassInterface(System.Runtime.InteropServices.ClassInterfaceType.AutoDual)]
    [System.Runtime.InteropServices.ComVisible(true)]
    public class PackBridge
    {
        private readonly PackController _packController;
        public PackBridge()
        {
            _packController = DIProvider.Provider.GetRequiredService<PackController>();
        }

        public string GetAllPackConfigs()
        {
            ApiResponse<List<PackConfig>> response = _packController.GetAllPackConfigs();
            string serialized = JsonUtil.Serialize(response);
            return serialized;
        }
         
        public string CreatePack(string serializedPackConfig)
        {
            PackConfig packConfig = JsonUtil.Deserialize<PackConfig>(serializedPackConfig) ?? new PackConfig();
            ApiResponse<PackConfig> response = _packController.CreatePack(packConfig);
            string serialized = JsonUtil.Serialize(response);
            return serialized;
        }

    }
}
