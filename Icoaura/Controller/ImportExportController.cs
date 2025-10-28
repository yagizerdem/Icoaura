using Icoaura.Context;
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
                string packDir = Path.Combine(ApplicationPathContext.AppPacksFolderPath, packId);
                if (!Directory.Exists(packDir))
                    throw AppException.Operational(_l10nService.GetLocalizedMessage("Errors.DirectoryNotFound"));

                string zipPath = Path.Combine(GetDownloadFolderPath(), $"{Guid.NewGuid().ToString()}.icr");


                ZipFile.CreateFromDirectory(
                    sourceDirectoryName: packDir,
                    destinationArchiveFileName: zipPath,
                    compressionLevel: CompressionLevel.Optimal,
                    includeBaseDirectory: false
                );

                return new object();
            });

        }

        public ApiResponse<object> ImportPack(string packPath)
        {
            return ExecuteSafe(() =>
            {
                if (!File.Exists(packPath))
                    throw AppException.Operational(_l10nService.GetLocalizedMessage("Errors.FileNotFound"));

                string tempPath = Path.Combine(ApplicationPathContext.AppTempFolderPath, Guid.NewGuid().ToString());
                ZipFile.ExtractToDirectory(
                    sourceArchiveFileName: packPath,
                    destinationDirectoryName: tempPath
                );

                string configPath = Path.Combine(tempPath, ApplicationPathContext.PACK_CONFIG_FILE_NAME);
                if(!File.Exists(configPath))
                    throw AppException.Operational(_l10nService.GetLocalizedMessage("Errors.FileNotFound"));

                string serializedConfig = File.ReadAllText(configPath);
                PackConfig? packConfig = JsonUtil.Deserialize<PackConfig>(serializedConfig);
                if(packConfig == null)
                    throw AppException.Operational(_l10nService.GetLocalizedMessage("Errors.PackConfigCorrupted"));

                string destPackDir = Path.Combine(ApplicationPathContext.AppPacksFolderPath, packConfig.Uid);

                if (Directory.Exists(destPackDir))
                    Directory.Delete(destPackDir, true);
                
                Directory.Move(tempPath, destPackDir);

                return new object();
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
