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

        public AppConfigController(FileController fileController)
        {
            _fileController = fileController;
        }

        public ApiResponse<AppConfig> ReadAppConfig()
        {
            return ExecuteSafe(() =>
            {
                this._logger.Log("Entering ReadAppConfig()", TraceContext.TraceId, LogLevel.Trace);

                string configPath = Path.Combine(ApplicationPathContext.AppConfigFilePath);
                this._logger.Log($"Resolved config path: {configPath}", TraceContext.TraceId, LogLevel.Debug);

                if (!File.Exists(configPath))
                {
                    this._logger.Log($"AppConfig file not found at {configPath}", TraceContext.TraceId, LogLevel.Warning);
                }

                var response = _fileController.ReadFileContentAsText(configPath);
                EnsureSuccess(response);

                string jsonContent = response.Data;
                this._logger.Log($"AppConfig JSON content read ({jsonContent.Length} characters)", TraceContext.TraceId, LogLevel.Trace);

                AppConfig config = System.Text.Json.JsonSerializer.Deserialize<AppConfig>(jsonContent)
                    ?? throw AppException.Operational(
                        userMessage: this._l10nService.GetLocalizedMessage("Errors.AppConfigCorrupted"),
                        logMessage: $"Failed to deserialize AppConfig from '{configPath}'.",
                        level: LogLevel.Fatal,
                        sourceName: nameof(ReadAppConfig));

                this._logger.Log("AppConfig deserialized successfully", TraceContext.TraceId, LogLevel.Info);
                return config;
            });
        }

        public ApiResponse<AppConfig> WriteAppConfig(AppConfig config)
        {
            return ExecuteSafe(() =>
            {
                this._logger.Log("Entering WriteAppConfig()", TraceContext.TraceId, LogLevel.Trace);

                string configPath = Path.Combine(ApplicationPathContext.AppConfigFilePath);
                this._logger.Log($"Resolved config path: {configPath}", TraceContext.TraceId, LogLevel.Debug);

                var serialized = JsonUtil.Serialize(config);
                this._logger.Log("AppConfig serialized successfully", TraceContext.TraceId, LogLevel.Debug);

                File.WriteAllText(configPath, serialized);
                this._logger.Log($"AppConfig written to disk at '{configPath}'", TraceContext.TraceId, LogLevel.Info);

                GlobalContext.AppConfig = config;
                this._logger.Log("GlobalContext.AppConfig updated", TraceContext.TraceId, LogLevel.Trace);

                return config;
            });
        }

        public ApiResponse<object> ExportAppConfig()
        {
            return ExecuteSafe(() =>
            {
                this._logger.Log("Starting ExportAppConfig()", TraceContext.TraceId, LogLevel.Trace);

                var response = ReadAppConfig();
                EnsureSuccess(response);

                string downloadFolderPath = GetDownloadFolderPath();
                string exportFilePath = Path.Combine(downloadFolderPath, $"{Guid.NewGuid()}.json");

                this._logger.Log($"Export file target: {exportFilePath}", TraceContext.TraceId, LogLevel.Debug);

                AppConfig config = response.Data;
                string serialized = JsonUtil.Serialize(config);
                File.WriteAllText(exportFilePath, serialized);

                this._logger.Log($"AppConfig exported successfully to '{exportFilePath}'", TraceContext.TraceId, LogLevel.Info);

                return new object();
            });
        }

        public ApiResponse<AppConfig> ImportAppConfig(string path)
        {
            return ExecuteSafe(() =>
            {
                this._logger.Log($"Starting ImportAppConfig(path={path})", TraceContext.TraceId, LogLevel.Trace);

                if (!File.Exists(path))
                {
                    this._logger.Log($"Import path not found: {path}", TraceContext.TraceId, LogLevel.Warning);
                }

                string serialized = File.ReadAllText(path);
                this._logger.Log($"Read imported JSON ({serialized.Length} chars)", TraceContext.TraceId, LogLevel.Debug);

                AppConfig config = JsonUtil.Deserialize<AppConfig>(serialized)
                    ?? throw AppException.Operational(
                        userMessage: this._l10nService.GetLocalizedMessage("Errors.AppConfigCorrupted"),
                        logMessage: $"Failed to deserialize AppConfig from '{path}'.",
                        level: LogLevel.Fatal,
                        sourceName: nameof(ImportAppConfig));

                this._logger.Log("AppConfig deserialized successfully", TraceContext.TraceId, LogLevel.Info);

                var response = WriteAppConfig(config);
                EnsureSuccess(response);
                this._logger.Log("AppConfig imported and written successfully", TraceContext.TraceId, LogLevel.Info);

                return config;
            });
        }

        private string GetHomePath()
        {
            this._logger.Log("Resolving home path", TraceContext.TraceId, LogLevel.Trace);

            if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
            {
                string home = System.Environment.GetEnvironmentVariable("HOME");
                this._logger.Log($"Unix HOME path resolved: {home}", TraceContext.TraceId, LogLevel.Debug);
                return home;
            }

            string windowsHome = System.Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            this._logger.Log($"Windows HOME path resolved: {windowsHome}", TraceContext.TraceId, LogLevel.Debug);
            return windowsHome;
        }

        private string GetDownloadFolderPath()
        {
            this._logger.Log("Resolving download folder path", TraceContext.TraceId, LogLevel.Trace);

            if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
            {
                string pathDownload = Path.Combine(GetHomePath(), "Downloads");
                this._logger.Log($"Unix download folder resolved: {pathDownload}", TraceContext.TraceId, LogLevel.Debug);
                return pathDownload;
            }

            string winPath = Convert.ToString(
                Microsoft.Win32.Registry.GetValue(
                    @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders",
                    "{374DE290-123F-4565-9164-39C4925E467B}",
                    string.Empty
                )
            );

            if (string.IsNullOrWhiteSpace(winPath))
            {
                this._logger.Log("Failed to resolve Windows download folder. Fallback to default.", TraceContext.TraceId, LogLevel.Warning);
            }

            this._logger.Log($"Windows download folder resolved: {winPath}", TraceContext.TraceId, LogLevel.Debug);
            return winPath;
        }
    }
}
