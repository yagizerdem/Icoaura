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
                this._logger.Log("Entering ReadAppConfig()", TraceContext.TraceId, LogLevel.Trace);

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


        public ApiResponse<object> ExportAppConfig()
        {
            return ExecuteSafe(() =>
            {
                var response = ReadAppConfig();
                EnsureSuccess(response);

                string downloadFolderPath = GetDownloadFolderPath();
                string exportFilePath = Path.Combine(downloadFolderPath, $"{Guid.NewGuid().ToString()}.json");

                AppConfig config = response.Data;
                string serlialized = JsonUtil.Serialize(config);
                File.WriteAllText(exportFilePath, serlialized);

                return new object();
            }); 
        }

        public ApiResponse<AppConfig> ImportAppConfig(string path)
        {
            return ExecuteSafe(() =>
            {
                string serialized = File.ReadAllText(path);
                AppConfig config = JsonUtil.Deserialize<AppConfig>(serialized)
                    ?? throw AppException.Operational(
                        userMessage: this._l10nService.GetLocalizedMessage("Errors.AppConfigCorrupted"),
                        logMessage: $"Failed to deserialize AppConfig from '{path}'. The JSON structure may be malformed or contain invalid values.",
                        level: LogLevel.Fatal,
                        sourceName: nameof(ImportAppConfig));

                var response = WriteAppConfig(config);
                EnsureSuccess(response);
                return config;
            });
        }


        private string GetHomePath()
        {
            if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
                return System.Environment.GetEnvironmentVariable("HOME");

            return System.Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
        }

        private string GetDownloadFolderPath()
        {
            if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
            {
                string pathDownload = System.IO.Path.Combine(GetHomePath(), "Downloads");
                return pathDownload;
            }

            return System.Convert.ToString(
                Microsoft.Win32.Registry.GetValue(
                     @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders"
                    , "{374DE290-123F-4565-9164-39C4925E467B}"
                    , String.Empty
                )
            );
        }
    }
}
