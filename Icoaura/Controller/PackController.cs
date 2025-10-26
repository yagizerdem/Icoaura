using Icoaura.Context;
using Icoaura.Enum;
using Icoaura.Exception;
using Icoaura.Model;
using Icoaura.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Icoaura.Controller
{
    public  class PackController : BaseController
    {
        public PackController()
        {
            
        }

        public ApiResponse<PackConfig> CreatePack(PackConfig config)
        {
            return ExecuteSafe(() =>
            {
                string packUid = Guid.NewGuid().ToString();
                config.Uid = packUid;
                config.CreatedAt = DateTime.UtcNow;
                config.Opacity = GlobalContext.AppConfig.PackOpacity;
                config.CornerRadius = GlobalContext.AppConfig.PackCornerRadius;

                Directory.CreateDirectory(Path.Combine(ApplicationPathContext.AppPacksFolderPath, packUid));

                this.ValidatePackConfig(config);

                EnsureSuccess(WritePackConfig(config));

                return config;
            });
        }


        public ApiResponse<PackConfig> GetPackConfig(string packId)
        {
            return ExecuteSafe(() =>
            {
                string configPath = Path.Combine(ApplicationPathContext.AppPacksFolderPath, packId, ApplicationPathContext.PACK_CONFIG_FILE_NAME);
                FileUtil.EnsureFileExist(configPath);
                string serializedConfig = File.ReadAllText(configPath);
                PackConfig config = JsonUtil.Deserialize<PackConfig>(serializedConfig) ??
                    throw AppException.Operational(
                        userMessage: "Failed to load pack configuration. The file may be corrupted or invalid.",
                        logMessage: $"Deserialization of PackConfig failed for pack '{packId}'. File path: {configPath}",
                        level:LogLevel.Error );

                return config;
            });
        }

        public ApiResponse<PackConfig> WritePackConfig(PackConfig config)
        {
            return ExecuteSafe(() =>
            {
                if(string.IsNullOrEmpty(config.Uid))
                {
                    throw AppException.Operational(
                        userMessage: "Pack configuration cannot be null or empty.",
                        logMessage: "WritePackConfig failed: PackConfig is null or empty.",
                        level: LogLevel.Error
                    );
                }

                string packPath = Path.Combine(ApplicationPathContext.AppPacksFolderPath, config.Uid);
                FileUtil.EnsureDirectoryExist(packPath);
                string configPath = Path.Combine(packPath, ApplicationPathContext.PACK_CONFIG_FILE_NAME);
                string serializedConfig = JsonUtil.Serialize(config);
                File.WriteAllText(configPath, serializedConfig);

                return config;

            });
        }

        

        // auxilary
        public void ValidatePackConfig(PackConfig config)
        {
            if (string.IsNullOrEmpty(config.PackName))
            {
                throw AppException.Operational(
                    userMessage: "Pack name cannot be empty.",
                    logMessage: "PackConfig validation failed: PackName is null or empty.",
                    level:LogLevel.Error
                );
            }

            string normalizedVersion = config.Version.StartsWith("v") ? config.Version.Substring(1) : config.Version;
            normalizedVersion = normalizedVersion.Trim();

            string versionPattern = @"^\d+\.\d+\.\d+$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(config.Version, versionPattern))
            {
                throw AppException.Operational(
                    userMessage: "Version must be in the format 'major.minor.patch' (e.g., '1.0.0').",
                    logMessage: "PackConfig validation failed: Version format is invalid.",
                    level: LogLevel.Error
                );
            }
        
        }
    }
}
