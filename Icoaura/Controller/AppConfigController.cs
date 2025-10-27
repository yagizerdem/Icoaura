using Icoaura.Context;
using Icoaura.Enum;
using Icoaura.Exception;
using Icoaura.Model;
using Icoaura.Util;
using System.IO;

namespace Icoaura.Controller
{
    public class AppConfigController : BaseController
    {
        private readonly FileController _fileController;

        public AppConfigController(
            FileController fileController) 
        {
            _fileController = fileController;
        }

        
        public ApiResponse<AppConfig> ReadAppConfig()
        {
            return ExecuteSafe(() =>
            {
                string configPath = Path.Combine(ApplicationPathContext.AppConfigFilePath);
                var response = _fileController.ReadFileContentAsText(configPath);
                EnsureSuccess(response);
                string jsonContent = response.Data;
                AppConfig config = System.Text.Json.JsonSerializer.Deserialize<AppConfig>(jsonContent)
                ?? throw AppException.Operational(
                    userMessage: this._l10nService.GetLocalizedMessage("Errors.AppConfigCorrupted"),
                    logMessage: $"Failed to deserialize AppConfig from '{configPath}'. The JSON structure may be malformed or contain invalid values.",
                    level: LogLevel.Fatal,
                    sourceName: nameof(ReadAppConfig));

                return config;
            });
        }

        public ApiResponse<AppConfig> WriteAppConfig(AppConfig config)
        {
            return ExecuteSafe(() =>
            {
                string configPath = Path.Combine(ApplicationPathContext.AppConfigFilePath);
                var serialized = JsonUtil.Serialize(config);
                File.WriteAllText(configPath, serialized);
                
                GlobalContext.AppConfig = config; // sync with global context

                return config;
            });
        }

    }
}
