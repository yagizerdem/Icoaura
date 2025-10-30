using Icoaura.Context;
using Icoaura.Enum;
using Icoaura.Exception;
using Icoaura.Model;
using Icoaura.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Icoaura.Controller
{
    public class ImportExportController : BaseController
    {
        public ImportExportController()
        {
        }

        public ApiResponse<object> ExportPack(string packId)
        {
            return ExecuteSafe(() =>
            {
                this._logger.Log("Entering ExportPack()", TraceContext.TraceId, LogLevel.Trace);
                this._logger.Log($"Parameters: PackId={packId}", TraceContext.TraceId, LogLevel.Debug);

                string packDir = Path.Combine(ApplicationPathContext.AppPacksFolderPath, packId);
                this._logger.Log($"Resolved pack directory: {packDir}", TraceContext.TraceId, LogLevel.Debug);

                if (!Directory.Exists(packDir))
                {
                    this._logger.Log($"Pack directory not found: {packDir}", TraceContext.TraceId, LogLevel.Error);
                    throw AppException.Operational(_l10nService.GetLocalizedMessage("Errors.DirectoryNotFound"));
                }

                string zipPath = Path.Combine(GetDownloadFolderPath(), $"{Guid.NewGuid().ToString()}.icr");
                this._logger.Log($"Preparing to export pack to zip: {zipPath}", TraceContext.TraceId, LogLevel.Info);

                try
                {
                    ZipFile.CreateFromDirectory(
                        sourceDirectoryName: packDir,
                        destinationArchiveFileName: zipPath,
                        compressionLevel: CompressionLevel.Optimal,
                        includeBaseDirectory: false
                    );
                    this._logger.Log($"Pack successfully exported to: {zipPath}", TraceContext.TraceId, LogLevel.Info);
                }
                catch (System.Exception ex)
                {
                    this._logger.Log($"ExportPack failed during compression: {ex.Message}", TraceContext.TraceId, LogLevel.Error);
                    throw AppException.Operational("Failed to export pack archive.", ex.Message, LogLevel.Error);
                }

                return new object();
            });
        }

        public ApiResponse<object> ImportPack(string packPath)
        {
            return ExecuteSafe(() =>
            {
                this._logger.Log("Entering ImportPack()", TraceContext.TraceId, LogLevel.Trace);
                this._logger.Log($"Parameters: PackPath={packPath}", TraceContext.TraceId, LogLevel.Debug);

                if (!File.Exists(packPath))
                {
                    this._logger.Log($"Pack file not found: {packPath}", TraceContext.TraceId, LogLevel.Error);
                    throw AppException.Operational(_l10nService.GetLocalizedMessage("Errors.FileNotFound"));
                }

                string tempPath = Path.Combine(ApplicationPathContext.AppTempFolderPath, Guid.NewGuid().ToString());
                this._logger.Log($"Extracting pack to temporary path: {tempPath}", TraceContext.TraceId, LogLevel.Debug);

                try
                {
                    ZipFile.ExtractToDirectory(packPath, tempPath);
                    this._logger.Log($"Extraction completed for: {packPath}", TraceContext.TraceId, LogLevel.Info);
                }
                catch (System.Exception ex)
                {
                    this._logger.Log($"ImportPack failed during extraction: {ex.Message}", TraceContext.TraceId, LogLevel.Error);
                    throw AppException.Operational("Failed to extract pack archive.", ex.Message, LogLevel.Error);
                }

                string configPath = Path.Combine(tempPath, ApplicationPathContext.PACK_CONFIG_FILE_NAME);
                this._logger.Log($"Looking for config file: {configPath}", TraceContext.TraceId, LogLevel.Debug);

                if (!File.Exists(configPath))
                {
                    this._logger.Log("Pack configuration file not found.", TraceContext.TraceId, LogLevel.Error);
                    throw AppException.Operational(_l10nService.GetLocalizedMessage("Errors.FileNotFound"));
                }

                string serializedConfig = File.ReadAllText(configPath);
                this._logger.Log($"Config file read successfully ({serializedConfig.Length} bytes)", TraceContext.TraceId, LogLevel.Debug);

                PackConfig? packConfig = JsonUtil.Deserialize<PackConfig>(serializedConfig);
                if (packConfig == null)
                {
                    this._logger.Log("Deserialization failed — PackConfig is null.", TraceContext.TraceId, LogLevel.Error);
                    throw AppException.Operational(_l10nService.GetLocalizedMessage("Errors.PackConfigCorrupted"));
                }

                this._logger.Log($"PackConfig deserialized. Uid={packConfig.Uid}, Name={packConfig.PackName}", TraceContext.TraceId, LogLevel.Info);

                string destPackDir = Path.Combine(ApplicationPathContext.AppPacksFolderPath, packConfig.Uid);
                this._logger.Log($"Target pack directory resolved: {destPackDir}", TraceContext.TraceId, LogLevel.Debug);

                try
                {
                    if (Directory.Exists(destPackDir))
                    {
                        this._logger.Log($"Existing pack directory found — deleting: {destPackDir}", TraceContext.TraceId, LogLevel.Warning);
                        Directory.Delete(destPackDir, true);
                    }

                    Directory.Move(tempPath, destPackDir);
                    this._logger.Log($"Pack imported successfully into: {destPackDir}", TraceContext.TraceId, LogLevel.Info);
                }
                catch (System.Exception ex)
                {
                    this._logger.Log($"ImportPack failed during directory move: {ex.Message}", TraceContext.TraceId, LogLevel.Error);
                    throw AppException.Operational("Failed to finalize pack import.", ex.Message, LogLevel.Error);
                }

                return new object();
            });
        }

        private string GetHomePath()
        {
            this._logger.Log("Resolving home path.", TraceContext.TraceId, LogLevel.Trace);

            if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
            {
                string home = System.Environment.GetEnvironmentVariable("HOME");
                this._logger.Log($"Unix HOME path resolved: {home}", TraceContext.TraceId, LogLevel.Debug);
                return home;
            }

            string winHome = System.Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            this._logger.Log($"Windows home path resolved: {winHome}", TraceContext.TraceId, LogLevel.Debug);
            return winHome;
        }

        private string GetDownloadFolderPath()
        {
            this._logger.Log("Resolving download folder path.", TraceContext.TraceId, LogLevel.Trace);

            if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
            {
                string pathDownload = System.IO.Path.Combine(GetHomePath(), "Downloads");
                this._logger.Log($"Unix downloads folder path resolved: {pathDownload}", TraceContext.TraceId, LogLevel.Debug);
                return pathDownload;
            }

            string? winDownload = System.Convert.ToString(
                Microsoft.Win32.Registry.GetValue(
                     @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders",
                     "{374DE290-123F-4565-9164-39C4925E467B}",
                     String.Empty
                )
            );

            this._logger.Log($"Windows downloads folder path resolved: {winDownload}", TraceContext.TraceId, LogLevel.Debug);
            return winDownload;
        }
    }
}
