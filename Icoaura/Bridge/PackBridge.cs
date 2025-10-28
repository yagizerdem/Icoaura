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

        public string DeletePack(string packId, bool deleteIcons)
        {
            ApiResponse<object> response =  _packController.DeletePack(packId, deleteIcons);
            string serialized = JsonUtil.Serialize(response);
            return serialized;
        }

        public string WritePackConfig(string serializedPackConfig)
        {
            PackConfig packConfig = JsonUtil.Deserialize<PackConfig>(serializedPackConfig) ?? new PackConfig();
            ApiResponse<PackConfig> response = _packController.WritePackConfig(packConfig);
            string serialized = JsonUtil.Serialize(response);
            return serialized;
        }

        public string GetPackItems(string packId)
        {
            ApiResponse<List<PackItem>> response = _packController.GetPackItems(packId);
            string serialized = JsonUtil.Serialize(response);
            return serialized;
        }

        public string GetPackItemIconBase64(string packId, string packItemId)
        {
            ApiResponse<string> response = _packController.GetPackItemIconBase64(packId, packItemId);
            string serialized = JsonUtil.Serialize(response);
            return serialized;
        }


        public string AddDesktopIcons(string packId)
        {
            ApiResponse<List<PackItem>> response =  _packController.AddDesktopIcons(packId);
            string serialized = JsonUtil.Serialize(response);
            return serialized;
        }

        public string AppendPackItemFromPath(string packId, string path)
        {
            ApiResponse<PackItem> response = _packController.AppendPackItemFromPath(packId, path);
            string serialized = JsonUtil.Serialize(response);
            return serialized;
        }

    }
}
