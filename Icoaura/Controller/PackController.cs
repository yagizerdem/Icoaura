using Icoaura.Context;
using Icoaura.Enum;
using Icoaura.Exception;
using Icoaura.Model;
using Icoaura.Util;
using Model.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Icoaura.Controller
{
    public  class PackController : BaseController
    {
        private readonly PackOperationController _packOperationController;
        private readonly FileController _fileController;
        private readonly ExternalController _externalController;
        public PackController(PackOperationController packOperationController, 
            FileController fileController,
            ExternalController externalController)
        {
            _packOperationController = packOperationController;
            _fileController = fileController;
            _externalController = externalController;
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
                        userMessage: this._l10nService.GetLocalizedMessage("Errors.PackConfigCorrupted"),
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
                        userMessage: this._l10nService.GetLocalizedMessage("Errors.PackConfigCorrupted"),
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

        public ApiResponse<List<PackItem>> GetPackItems(string packId)
        {
            return ExecuteSafe(() =>
            {
                string packItemsPath = Path.Combine(ApplicationPathContext.AppPacksFolderPath, 
                    packId, 
                    ApplicationPathContext.PACK_ITEMS_FILE_NAME);

                if (!File.Exists(packItemsPath))
                    return new();

                var serializedItems = File.ReadAllText(packItemsPath);
                List<PackItem> packItems = JsonUtil.Deserialize<List<PackItem>>(serializedItems) ?? new List<PackItem>();
                return packItems;
            });
        }

        public ApiResponse<string> GetPackItemIconBase64(string packId, string packItemId)
        {
            return ExecuteSafe(() =>
            {
                string packDir = Path.Combine(ApplicationPathContext.AppPacksFolderPath,
                    packId);
                FileUtil.EnsureDirectoryExist(packDir);

                string iconPath = Path.Combine(packDir, ApplicationPathContext.PACK_ICONS_FOLDER_NAME, $"{packItemId}.png");
                FileUtil.EnsureFileExist(iconPath);

                ApiResponse<string> response = _fileController.GetBase64(iconPath);
                EnsureSuccess(response);
                return response.Data ?? string.Empty;
            });
        }

        public ApiResponse<PackItem> AppendPackItemFromPath(string packId, string itemPath)
        {
            return ExecuteSafe(() =>
            {
                PackItem packItem = new()
                {
                    Uid = Guid.NewGuid().ToString(),
                    TargetPath = PathUtil.ConvertToRelativePath(itemPath),
                    Name = Path.GetFileNameWithoutExtension(itemPath)
                };

                string iconPath = Path.Combine(ApplicationPathContext.AppPacksFolderPath, packId, ApplicationPathContext.PACK_ICONS_FOLDER_NAME);
                if (!Directory.Exists(iconPath))
                    Directory.CreateDirectory(iconPath);

                string icoBase64 = string.Empty;

                if (packItem.IsLnkItem)
                {
                    var response = _fileController.GetLnkMetaData(itemPath);
                    EnsureSuccess(response);
                    LnkMetaData metaData = response.Data ?? throw AppException.Operational(
                                userMessage: this._l10nService.GetLocalizedMessage("Errors.ShortcutLoadFailed"),
                                logMessage: $"LnkMetaData deserialization returned null for shortcut response (path: {itemPath}).",
                                level: LogLevel.Error);
                    packItem.Description = metaData.Description;
                    packItem.TargetExePath = PathUtil.ConvertToRelativePath(metaData.TargetPath);

                    icoBase64 = _fileController.GetBase64(metaData.IconPath).Data;

                    if(string.IsNullOrEmpty(icoBase64))
                    {
                        // extract  lnk icon from exe
                        var exeIcoResponse = _externalController.GetExeIconAsPngBase64(metaData.TargetPath);
                        icoBase64 = exeIcoResponse.Data ?? string.Empty;
                    }
                }
                else if (packItem.IsUrlItem)
                {
                    var response = _fileController.GetUrlMetaData(itemPath);
                    EnsureSuccess(response);
                    UrlMetaData metaData = response.Data ?? throw AppException.Operational(
                                userMessage: this._l10nService.GetLocalizedMessage("UrlFileLoadFailed"),
                                logMessage: $"UrlMetaData deserialization returned null for URL response (path: {itemPath}).",
                                level: LogLevel.Error);
                    packItem.TargetUrl = PathUtil.ConvertToRelativePath(metaData.Url);
                    icoBase64 = _fileController.GetBase64(metaData.IconPath).Data;
                }
                else if (packItem.IsDirItem)
                {
                    var resopnse = _fileController.GetDirMetaData(itemPath);
                    EnsureSuccess(resopnse);
                    DirMetaData metaData = resopnse.Data ?? throw AppException.Operational(
                                userMessage: this._l10nService.GetLocalizedMessage("Errors.DirectoryMetadataLoadFailed"),
                                logMessage: $"DirMetaData deserialization returned null for directory response (path: {itemPath}).",
                                level: LogLevel.Error);

                    icoBase64 = _fileController.GetBase64(metaData.IconPath).Data;
                }

                if (!string.IsNullOrEmpty(icoBase64))
                {
                    string iconFileName = $"{packItem.Uid}.png";
                    string iconFilePath = Path.Combine(iconPath, iconFileName);
                    _fileController.WriteBase64(iconFilePath, icoBase64, overwrite: true);
                }

                var writeresponse = GetPackItems(packId);
                EnsureSuccess(writeresponse);
                List<PackItem> packItems = writeresponse.Data ?? new List<PackItem>();
                packItems.Add(packItem);
                string packItemsPath = Path.Combine(ApplicationPathContext.AppPacksFolderPath,
                    packId,
                    ApplicationPathContext.PACK_ITEMS_FILE_NAME);
                string serializedItems = JsonUtil.Serialize(packItems);
                File.WriteAllText(packItemsPath, serializedItems);
                return packItem;

            });
        }

        public ApiResponse<List<PackItem>> WritePackItems(string packId, 
            List<PackItem> packItems,
            Dictionary<string, string> packItemId_base64Map)
        {
            return ExecuteSafe(() =>
            {
                string packPath = Path.Combine(ApplicationPathContext.AppPacksFolderPath, packId);
                FileUtil.EnsureDirectoryExist(packPath);

                string packItemsPath = Path.Combine(packPath,
                    ApplicationPathContext.PACK_ITEMS_FILE_NAME);

                if(File.Exists(packItemsPath))
                    File.Delete(packItemsPath);

                string iconsPath = Path.Combine(packPath, ApplicationPathContext.PACK_ICONS_FOLDER_NAME);
                if (!Directory.Exists(iconsPath))
                {
                    Directory.CreateDirectory(iconsPath);
                }
                else
                {
                    DirectoryInfo di = new DirectoryInfo(iconsPath);
                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                }

                    if (packItemId_base64Map != null)
                {
                    for (int i = 0; i < packItemId_base64Map.Count; i++)
                    {
                        string packItemId = packItemId_base64Map.ElementAt(i).Key;
                        string base64 = packItemId_base64Map.ElementAt(i).Value;
                        string iconFileName = $"{packItemId}.png";
                        string iconFilePath = Path.Combine(iconsPath, iconFileName);
                        _fileController.WriteBase64(iconFilePath, base64, overwrite: true);
                    }

                }



                string serializedItems = JsonUtil.Serialize(packItems);
                File.WriteAllText(packItemsPath, serializedItems);
                return packItems;
            });
        }

        public ApiResponse<object> DeletePack(string packId, bool deleteIcons)
        {
            return ExecuteSafe(() =>
            {
                string packPath = Path.Combine(ApplicationPathContext.AppPacksFolderPath, packId);
                if (Directory.Exists(packPath))
                {
                    Directory.Delete(packPath, true);
                }
                if (deleteIcons)
                {
                    string iconsPath = Path.Combine(ApplicationPathContext.AppIconsFolderPath, packId);
                    if (Directory.Exists(iconsPath))
                    {
                        Directory.Delete(iconsPath, true);
                    }
                }

                return new object();
            });
        }


        public ApiResponse<List<PackItem>> AddDesktopIcons(string packId)
        {
            return ExecuteSafe(() =>
            {
                List<string> paths = new List<string>();

                var response = _fileController.GetFilesUnderPath(
                    PathUtil.Resolve("%DESKTOP%"), 
                    ["lnk", ".lnk", "url", ".url"],
                    0);
                
                if(response.Data != null)
                    paths.AddRange(response.Data);

                response = _fileController.GetFilesUnderPath(
                    PathUtil.Resolve("%DESKTOP_COMMON%"),
                    ["lnk", ".lnk", "url", ".url"],
                    0);
                
                if(response.Data != null)
                    paths.AddRange(response.Data);

                response = _fileController.GetFoldersUnderPath(
                    PathUtil.Resolve("%DESKTOP%"),
                    0);
                
                if (response.Data != null)
                    paths.AddRange(response.Data);

                response = _fileController.GetFoldersUnderPath(
                    PathUtil.Resolve("%DESKTOP_COMMON%"),
                    0);

                if (response.Data != null)
                    paths.AddRange(response.Data);

                foreach (var path in paths)
                    AppendPackItemFromPath(packId, path);

                return GetPackItems(packId).Data ?? new List<PackItem>();
            });
        }
        public ApiResponse<object> ApplyPackOperations(string packId)
        {
            return ExecuteSafe(() =>
            {
                // --- Load configs ---
                PackConfig config = GetPackConfig(packId).Data;
                AppConfig appConfig = GlobalContext.AppConfig;
                List<PackItem> packItems = GetPackItems(packId).Data ?? new List<PackItem>();

                List<string> possibleLnkFiles = new();
                List<string> possibleUrlFiles = new();

                // --- Collect all .lnk and .url files from special folders ---
                foreach (string specialPath in EnvUtil.SpecialFolders)
                {
                    var result = _fileController.GetFilesUnderPath(
                        PathUtil.Resolve(specialPath),
                        new[] { "lnk", ".lnk", "url", ".url" },
                        2
                    ).Data ?? new();

                    possibleLnkFiles.AddRange(result.Where(p => p.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase)));
                    possibleUrlFiles.AddRange(result.Where(p => p.EndsWith(".url", StringComparison.OrdinalIgnoreCase)));
                }

                // --- Process each pack item ---
                foreach (PackItem packItem in packItems)
                {
                    string packProcessedIconsPath = Path.Combine(ApplicationPathContext.AppIconsFolderPath, packId);
                    if (!Directory.Exists(packProcessedIconsPath))
                        Directory.CreateDirectory(packProcessedIconsPath);

                    string originalPngPath = Path.Combine(
                        ApplicationPathContext.AppPacksFolderPath,
                        packId,
                        ApplicationPathContext.PACK_ICONS_FOLDER_NAME,
                        $"{packItem.Uid}.png"
                    );

                    string processedIcoBase64 = ProcessPngImage(
                        originalPngPath,
                        config.CornerRadius,
                        config.Opacity
                    );

                    if (string.IsNullOrEmpty(processedIcoBase64))
                        continue;

                    string processedIconPath = Path.Combine(packProcessedIconsPath, $"{Guid.NewGuid()}.ico");
                    var writeResponse = _fileController.WriteBase64(processedIconPath, processedIcoBase64, overwrite: true);

                    if (!writeResponse.Success)
                        continue;

                    if (packItem.IsLnkItem)
                    {
                        HandleLnkPackItem(packItem, processedIconPath, possibleLnkFiles, config, appConfig, packId);
                    }

                    else if (packItem.IsUrlItem)
                    {
                        HandleUrlPackItem(packItem, processedIconPath, possibleUrlFiles, config, appConfig, packId);
                    }
                    else if(packItem.IsDirItem)
                    {
                        HandleDirPackItems(packItem, processedIconPath, config, appConfig, packId);
                    }
                }

                // --- Apply changes immediately ---
                _packOperationController.RefreshWindowsShell();

                if(appConfig.ForceExplorerRefreshAfterIcoChange)
                {
                    _packOperationController.ForceExplorerIconRefresh();
                }


                return new object();
            });
        }

        public ApiResponse<List<PackConfig>> GetAllPackConfigs()
        {
            return ExecuteSafe(() =>
            {
                List<PackConfig> packConfigs = new List<PackConfig>();
                string packsFolderPath = ApplicationPathContext.AppPacksFolderPath;
                if (!Directory.Exists(packsFolderPath))
                    return packConfigs;
                string[] packDirectories = Directory.GetDirectories(packsFolderPath);
                foreach (string packDirectory in packDirectories)
                {
                    string packId = Path.GetFileName(packDirectory);
                    try
                    {
                        PackConfig config = GetPackConfig(packId).Data;
                        if(config != null) packConfigs.Add(config);
                    }
                    catch (AppException ex)
                    {
                        // Log and skip corrupted pack configs

                    }
                }
               
                return packConfigs;
            });
        }

        private void HandleLnkPackItem(
            PackItem packItem,
            string processedIconPath,
            List<string> possibleLnkFiles,
            PackConfig config,
            AppConfig appConfig,
            string packId)
        {
            string? firstPart = packItem.TargetPath.Split("\\").FirstOrDefault();
            string remainingPart = packItem.TargetPath.Substring(firstPart?.Length ?? 0).TrimStart('\\');

            List<string> matches = new();

            if (!string.IsNullOrEmpty(firstPart))
            {
                matches = _fileController.GetMatchingFileSystemEntries(
                    PathUtil.Resolve(firstPart),
                    remainingPart
                ).Data ?? new();
            }

            // --- fallback match by exe path ---
            if (matches.Count == 0 && appConfig.MatchLnkByTargetExe)
            {
                foreach (string lnkFile in possibleLnkFiles)
                {
                    LnkMetaData metaData = _fileController.GetLnkMetaData(lnkFile).Data;
                    if (metaData == null) continue;

                    if (_fileController.MatchesGlobPattern(metaData.TargetPath, PathUtil.Resolve(packItem.TargetExePath)))
                        matches.Add(lnkFile);
                }
            }

            // --- apply operations ---
            ApplyLnkOperations(matches, processedIconPath, packItem, appConfig);
        }

        private void HandleUrlPackItem(
            PackItem packItem,
            string processedIconPath,
            List<string> possibleUrlFiles,
            PackConfig config,
            AppConfig appConfig,
            string packId)
        {
            string? firstPart = packItem.TargetPath.Split("\\").FirstOrDefault();
            string remainingPart = packItem.TargetPath.Substring(firstPart?.Length ?? 0).TrimStart('\\');

            List<string> matches = new();

            if (!string.IsNullOrEmpty(firstPart))
            {
                matches = _fileController.GetMatchingFileSystemEntries(
                    PathUtil.Resolve(firstPart),
                    remainingPart
                ).Data ?? new();
            }

            // --- fallback match by TargetUrl ---
            if (matches.Count == 0 && appConfig.MatchUrlByTargetUrl)
            {
                foreach (string urlFile in possibleUrlFiles)
                {
                    UrlMetaData metaData = _fileController.GetUrlMetaData(urlFile).Data;
                    if (metaData == null) continue;

                    string? targetUrl = metaData.Url;
                    if (string.IsNullOrEmpty(targetUrl))
                        continue;

                    if (_fileController.MatchesGlobPattern(targetUrl, PathUtil.Resolve(packItem.TargetUrl)))
                        matches.Add(urlFile);
                }
            }

            // --- apply operations ---
            ApplyUrlOperations(matches, processedIconPath, packItem, appConfig);
        }

        private void HandleDirPackItems(
            PackItem packItem,
            string processedIconPath,
            PackConfig config,
            AppConfig appConfig,
            string packId)
        {
            string? firstPart = packItem.TargetPath.Split("\\").FirstOrDefault();
            string remainingPart = packItem.TargetPath.Substring(firstPart?.Length ?? 0).TrimStart('\\');

            List<string> matches = new();

            // --- try pattern match ---
            if (!string.IsNullOrEmpty(firstPart))
            {
                matches = _fileController.GetMatchingFileSystemEntries(
                    PathUtil.Resolve(firstPart),
                    remainingPart
                ).Data ?? new();
            }


            ApplyDirOperations(matches, processedIconPath, packItem, appConfig);


        }

        private void ApplyLnkOperations(List<string> matches, string processedIconPath, PackItem packItem, AppConfig appConfig)
        {
            if (matches.Count == 0) return;

            foreach (string match in matches)
            {
                _packOperationController.ChangeLnkIconPath(match, processedIconPath);

                if (appConfig.ChangeDescriptionOfMatchedLnkFiles)
                {
                    _packOperationController.ChangeLnkDescription(match, packItem.Description);
                }
            }
        }

        private void ApplyUrlOperations(List<string> matches, string processedIconPath, PackItem packItem, AppConfig appConfig)
        {
            if (matches.Count == 0) return;

            foreach (string match in matches)
            {
                _packOperationController.ChangeIconPathOfUrlFile(match, processedIconPath);
            }
        }
        
        private void ApplyDirOperations(List<string> matches, string processedIconPath, PackItem packItem, AppConfig appConfig)
        {
            if (matches.Count == 0) return;

            // --- apply icon to matched directories ---
            foreach (string match in matches)
            {
                _packOperationController.ChangeDirectoryIcon(match, processedIconPath);
            }
        }

        public string ProcessPngImage(string pngPath, float cornerRadius, float opacityAmount)
        {
            string bas64 =  _fileController.GetBase64(pngPath).Data;
            if (string.IsNullOrEmpty(bas64)) return string.Empty;
            bas64 = _externalController.ApplyCornerRadiusOnPngBase64(bas64, cornerRadius).Data;
            if (string.IsNullOrEmpty(bas64)) return string.Empty;
            bas64 = _externalController.ApplyOpacityOnPngBase64(bas64, opacityAmount).Data;
            if (string.IsNullOrEmpty(bas64)) return string.Empty;
            bas64 = _externalController.PngBase64ToIcoBase64(bas64).Data;
            if (string.IsNullOrEmpty(bas64)) return string.Empty;
            return bas64;
        }


        // auxilary
        public void ValidatePackConfig(PackConfig config)
        {
            if (string.IsNullOrEmpty(config.PackName))
            {
                throw AppException.Operational(
                    userMessage: this._l10nService.GetLocalizedMessage("Errors.PackNameEmpty"),
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
                    userMessage: this._l10nService.GetLocalizedMessage("Errors.VersionFormatInvalid"),
                    logMessage: "PackConfig validation failed: Version format is invalid.",
                    level: LogLevel.Error
                );
            }
        
        }
    }
}
