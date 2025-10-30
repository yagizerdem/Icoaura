using Icoaura.Context;
using Icoaura.Enum;
using Icoaura.Exception;
using Icoaura.Model;
using Icoaura.Util;
using Model.DTO;
using System.IO;

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
                this._logger.Log("Entering CreatePack()", TraceContext.TraceId, LogLevel.Trace);

                string packUid = Guid.NewGuid().ToString();
                config.Uid = packUid;
                config.CreatedAt = DateTime.UtcNow;
                config.Opacity = GlobalContext.AppConfig.PackOpacity;
                config.CornerRadius = GlobalContext.AppConfig.PackCornerRadius;

                this._logger.Log($"Initialized PackConfig: Uid={packUid}, Opacity={config.Opacity}, CornerRadius={config.CornerRadius}", TraceContext.TraceId, LogLevel.Debug);

                string packPath = Path.Combine(ApplicationPathContext.AppPacksFolderPath, packUid);
                Directory.CreateDirectory(packPath);
                this._logger.Log($"Pack directory created at: {packPath}", TraceContext.TraceId, LogLevel.Info);

                this.ValidatePackConfig(config);
                this._logger.Log("PackConfig validation completed.", TraceContext.TraceId, LogLevel.Debug);

                EnsureSuccess(WritePackConfig(config));
                this._logger.Log("Pack configuration file successfully written.", TraceContext.TraceId, LogLevel.Info);

                return config;
            });
        }

        public ApiResponse<PackConfig> GetPackConfig(string packId)
        {
            return ExecuteSafe(() =>
            {
                this._logger.Log("Entering GetPackConfig()", TraceContext.TraceId, LogLevel.Trace);
                this._logger.Log($"Parameters: PackId={packId}", TraceContext.TraceId, LogLevel.Debug);

                string configPath = Path.Combine(ApplicationPathContext.AppPacksFolderPath, packId, ApplicationPathContext.PACK_CONFIG_FILE_NAME);
                FileUtil.EnsureFileExist(configPath);

                string serializedConfig = File.ReadAllText(configPath);
                this._logger.Log($"Read PackConfig file ({serializedConfig.Length} bytes) from: {configPath}", TraceContext.TraceId, LogLevel.Debug);

                PackConfig config = JsonUtil.Deserialize<PackConfig>(serializedConfig) ??
                    throw AppException.Operational(
                        userMessage: this._l10nService.GetLocalizedMessage("Errors.PackConfigCorrupted"),
                        logMessage: $"Deserialization failed for pack '{packId}' at '{configPath}'",
                        level: LogLevel.Error
                    );

                this._logger.Log($"PackConfig successfully loaded for pack '{packId}'", TraceContext.TraceId, LogLevel.Info);
                return config;
            });
        }

        public ApiResponse<PackConfig> WritePackConfig(PackConfig config)
        {
            return ExecuteSafe(() =>
            {
                this._logger.Log("Entering WritePackConfig()", TraceContext.TraceId, LogLevel.Trace);

                if (string.IsNullOrEmpty(config.Uid))
                {
                    this._logger.Log("WritePackConfig failed — Config.Uid is null or empty.", TraceContext.TraceId, LogLevel.Error);
                    throw AppException.Operational(
                        userMessage: this._l10nService.GetLocalizedMessage("Errors.PackConfigCorrupted"),
                        logMessage: "WritePackConfig failed: PackConfig.Uid is missing.",
                        level: LogLevel.Error
                    );
                }

                string packPath = Path.Combine(ApplicationPathContext.AppPacksFolderPath, config.Uid);
                FileUtil.EnsureDirectoryExist(packPath);

                string configPath = Path.Combine(packPath, ApplicationPathContext.PACK_CONFIG_FILE_NAME);
                string serializedConfig = JsonUtil.Serialize(config);
                File.WriteAllText(configPath, serializedConfig);

                this._logger.Log($"PackConfig written successfully to: {configPath}", TraceContext.TraceId, LogLevel.Info);
                return config;
            });
        }

        public ApiResponse<List<PackItem>> GetPackItems(string packId)
        {
            return ExecuteSafe(() =>
            {
                this._logger.Log("Entering GetPackItems()", TraceContext.TraceId, LogLevel.Trace);
                this._logger.Log($"Parameters: PackId={packId}", TraceContext.TraceId, LogLevel.Debug);

                string packItemsPath = Path.Combine(ApplicationPathContext.AppPacksFolderPath, packId, ApplicationPathContext.PACK_ITEMS_FILE_NAME);

                if (!File.Exists(packItemsPath))
                {
                    this._logger.Log($"Pack items file not found: {packItemsPath}", TraceContext.TraceId, LogLevel.Warning);
                    return new();
                }

                string serializedItems = File.ReadAllText(packItemsPath);
                this._logger.Log($"Pack items file read ({serializedItems.Length} bytes).", TraceContext.TraceId, LogLevel.Debug);

                List<PackItem> packItems = JsonUtil.Deserialize<List<PackItem>>(serializedItems) ?? new List<PackItem>();
                this._logger.Log($"Loaded {packItems.Count} pack items for pack '{packId}'.", TraceContext.TraceId, LogLevel.Info);

                return packItems;
            });
        }

        public ApiResponse<string> GetPackItemIconBase64(string packId, string packItemId)
        {
            return ExecuteSafe(() =>
            {
                this._logger.Log("Entering GetPackItemIconBase64()", TraceContext.TraceId, LogLevel.Trace);
                this._logger.Log($"Parameters: PackId={packId}, PackItemId={packItemId}", TraceContext.TraceId, LogLevel.Debug);

                string packDir = Path.Combine(ApplicationPathContext.AppPacksFolderPath, packId);
                FileUtil.EnsureDirectoryExist(packDir);

                string iconPath = Path.Combine(packDir, ApplicationPathContext.PACK_ICONS_FOLDER_NAME, $"{packItemId}.png");
                FileUtil.EnsureFileExist(iconPath);

                var response = _fileController.GetBase64(iconPath);
                EnsureSuccess(response);

                this._logger.Log($"Retrieved Base64 icon for item '{packItemId}' from '{iconPath}'", TraceContext.TraceId, LogLevel.Info);
                return response.Data ?? string.Empty;
            });
        }

        public ApiResponse<PackItem> AppendPackItemFromPath(string packId, string itemPath)
        {
            return ExecuteSafe(() =>
            {
                this._logger.Log("Entering AppendPackItemFromPath()", TraceContext.TraceId, LogLevel.Trace);
                this._logger.Log($"Parameters: PackId={packId}, ItemPath={itemPath}", TraceContext.TraceId, LogLevel.Debug);

                PackItem packItem = new()
                {
                    Uid = Guid.NewGuid().ToString(),
                    TargetPath = PathUtil.ConvertToRelativePath(itemPath),
                    Name = Path.GetFileNameWithoutExtension(itemPath)
                };

                this._logger.Log($"Creating PackItem: Uid={packItem.Uid}, Name={packItem.Name}, TargetPath={packItem.TargetPath}", TraceContext.TraceId, LogLevel.Debug);

                string iconDir = Path.Combine(ApplicationPathContext.AppPacksFolderPath, packId, ApplicationPathContext.PACK_ICONS_FOLDER_NAME);
                if (!Directory.Exists(iconDir))
                {
                    Directory.CreateDirectory(iconDir);
                    this._logger.Log($"Created icon directory: {iconDir}", TraceContext.TraceId, LogLevel.Debug);
                }

                string icoBase64 = string.Empty;

                if (packItem.IsLnkItem)
                {
                    this._logger.Log("Detected LNK item type.", TraceContext.TraceId, LogLevel.Debug);
                    var response = _fileController.GetLnkMetaData(itemPath);
                    EnsureSuccess(response);

                    LnkMetaData meta = response.Data ?? throw AppException.Operational(
                        userMessage: _l10nService.GetLocalizedMessage("Errors.ShortcutLoadFailed"),
                        logMessage: $"LnkMetaData is null for path {itemPath}.",
                        level: LogLevel.Error
                    );

                    packItem.Description = meta.Description;
                    packItem.TargetExePath = PathUtil.ConvertToRelativePath(meta.TargetPath);

                    icoBase64 = _fileController.GetBase64(meta.IconPath).Data;
                    if (string.IsNullOrEmpty(icoBase64))
                    {
                        this._logger.Log("LNK icon missing, attempting to extract from EXE.", TraceContext.TraceId, LogLevel.Warning);
                        var exeIcoResponse = _externalController.GetExeIconAsPngBase64(meta.TargetPath);
                        icoBase64 = exeIcoResponse.Data ?? string.Empty;
                    }
                    else
                    {
                        icoBase64 = _externalController.IcoBase64ToPngBase64(icoBase64).Data ?? string.Empty;
                    }

                }
                else if (packItem.IsUrlItem)
                {
                    this._logger.Log("Detected URL item type.", TraceContext.TraceId, LogLevel.Debug);
                    var response = _fileController.GetUrlMetaData(itemPath);
                    EnsureSuccess(response);

                    UrlMetaData meta = response.Data ?? throw AppException.Operational(
                        userMessage: _l10nService.GetLocalizedMessage("UrlFileLoadFailed"),
                        logMessage: $"UrlMetaData is null for path {itemPath}.",
                        level: LogLevel.Error
                    );

                    packItem.TargetUrl = PathUtil.ConvertToRelativePath(meta.Url);
                    icoBase64 = _fileController.GetBase64(meta.IconPath).Data;

                    if(!string.IsNullOrEmpty(icoBase64))
                    {
                        icoBase64 = _externalController.IcoBase64ToPngBase64(icoBase64).Data ?? string.Empty;
                    }
                }
                else if (packItem.IsDirItem)
                {
                    this._logger.Log("Detected Directory item type.", TraceContext.TraceId, LogLevel.Debug);
                    var response = _fileController.GetDirMetaData(itemPath);
                    EnsureSuccess(response);

                    DirMetaData meta = response.Data ?? throw AppException.Operational(
                        userMessage: _l10nService.GetLocalizedMessage("Errors.DirectoryMetadataLoadFailed"),
                        logMessage: $"DirMetaData is null for path {itemPath}.",
                        level: LogLevel.Error
                    );

                    icoBase64 = _fileController.GetBase64(meta.IconPath).Data;
                    if (!string.IsNullOrEmpty(icoBase64))
                    {
                        icoBase64 = _externalController.IcoBase64ToPngBase64(icoBase64).Data ?? string.Empty;
                    }
                }

                if (!string.IsNullOrEmpty(icoBase64))
                {
                    string iconFilePath = Path.Combine(iconDir, $"{packItem.Uid}.png");
                    _fileController.WriteBase64(iconFilePath, icoBase64, overwrite: true);
                    this._logger.Log($"Saved icon for PackItem '{packItem.Uid}' at: {iconFilePath}", TraceContext.TraceId, LogLevel.Info);
                }

                var itemsResponse = GetPackItems(packId);
                EnsureSuccess(itemsResponse);

                List<PackItem> packItems = itemsResponse.Data ?? new List<PackItem>();
                packItems.Add(packItem);

                string packItemsPath = Path.Combine(ApplicationPathContext.AppPacksFolderPath, packId, ApplicationPathContext.PACK_ITEMS_FILE_NAME);
                string serializedItems = JsonUtil.Serialize(packItems);
                File.WriteAllText(packItemsPath, serializedItems);

                this._logger.Log($"PackItem '{packItem.Name}' appended successfully to pack '{packId}'.", TraceContext.TraceId, LogLevel.Info);
                return packItem;
            });
        }



        public ApiResponse<List<PackItem>> WritePackItems(
    string packId,
    List<PackItem> packItems,
    Dictionary<string, string> packItemId_base64Map)
        {
            return ExecuteSafe(() =>
            {
                this._logger.Log("Entering WritePackItems()", TraceContext.TraceId, LogLevel.Trace);
                this._logger.Log($"Parameters: PackId={packId}, ItemsCount={packItems?.Count ?? 0}, IconMapCount={packItemId_base64Map?.Count ?? 0}", TraceContext.TraceId, LogLevel.Debug);

                string packPath = Path.Combine(ApplicationPathContext.AppPacksFolderPath, packId);
                FileUtil.EnsureDirectoryExist(packPath);

                string packItemsPath = Path.Combine(packPath, ApplicationPathContext.PACK_ITEMS_FILE_NAME);

                if (File.Exists(packItemsPath))
                {
                    this._logger.Log($"Existing pack items file found. Deleting: {packItemsPath}", TraceContext.TraceId, LogLevel.Warning);
                    File.Delete(packItemsPath);
                }

                string iconsPath = Path.Combine(packPath, ApplicationPathContext.PACK_ICONS_FOLDER_NAME);
                if (!Directory.Exists(iconsPath))
                {
                    Directory.CreateDirectory(iconsPath);
                    this._logger.Log($"Created icons folder: {iconsPath}", TraceContext.TraceId, LogLevel.Debug);
                }
                else
                {
                    this._logger.Log($"Clearing existing icons folder: {iconsPath}", TraceContext.TraceId, LogLevel.Warning);
                    DirectoryInfo di = new DirectoryInfo(iconsPath);
                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                        this._logger.Log($"Deleted old icon file: {file.FullName}", TraceContext.TraceId, LogLevel.Trace);
                    }
                }

                if (packItemId_base64Map != null)
                {
                    foreach (var kvp in packItemId_base64Map)
                    {
                        string packItemId = kvp.Key;
                        string base64 = kvp.Value;
                        string iconFilePath = Path.Combine(iconsPath, $"{packItemId}.png");

                        _fileController.WriteBase64(iconFilePath, base64, overwrite: true);
                        this._logger.Log($"Wrote icon for PackItem {packItemId} → {iconFilePath}", TraceContext.TraceId, LogLevel.Debug);
                    }
                }

                string serializedItems = JsonUtil.Serialize(packItems);
                File.WriteAllText(packItemsPath, serializedItems);
                this._logger.Log($"Pack items file successfully written: {packItemsPath}", TraceContext.TraceId, LogLevel.Info);

                return packItems;
            });
        }

        public ApiResponse<object> DeletePack(string packId, bool deleteIcons)
        {
            return ExecuteSafe(() =>
            {
                this._logger.Log("Entering DeletePack()", TraceContext.TraceId, LogLevel.Trace);
                this._logger.Log($"Parameters: PackId={packId}, DeleteIcons={deleteIcons}", TraceContext.TraceId, LogLevel.Debug);

                string packPath = Path.Combine(ApplicationPathContext.AppPacksFolderPath, packId);
                if (Directory.Exists(packPath))
                {
                    Directory.Delete(packPath, true);
                    this._logger.Log($"Deleted pack directory: {packPath}", TraceContext.TraceId, LogLevel.Info);
                }

                if (deleteIcons)
                {
                    string iconsPath = Path.Combine(ApplicationPathContext.AppIconsFolderPath, packId);
                    if (Directory.Exists(iconsPath))
                    {
                        Directory.Delete(iconsPath, true);
                        this._logger.Log($"Deleted associated icons directory: {iconsPath}", TraceContext.TraceId, LogLevel.Info);
                    }
                }

                return new object();
            });
        }

        public ApiResponse<List<PackItem>> AddDesktopIcons(string packId)
        {
            return ExecuteSafe(() =>
            {
                this._logger.Log("Entering AddDesktopIcons()", TraceContext.TraceId, LogLevel.Trace);
                this._logger.Log($"Parameters: PackId={packId}", TraceContext.TraceId, LogLevel.Debug);

                List<string> paths = new();

                var response = _fileController.GetFilesUnderPath(
                    PathUtil.Resolve("%DESKTOP%"),
                    ["lnk", ".lnk", "url", ".url"],
                    0);
                if (response.Data != null)
                    paths.AddRange(response.Data);

                response = _fileController.GetFilesUnderPath(
                    PathUtil.Resolve("%DESKTOP_COMMON%"),
                    ["lnk", ".lnk", "url", ".url"],
                    0);
                if (response.Data != null)
                    paths.AddRange(response.Data);

                response = _fileController.GetFoldersUnderPath(PathUtil.Resolve("%DESKTOP%"), 0);
                if (response.Data != null)
                    paths.AddRange(response.Data);

                response = _fileController.GetFoldersUnderPath(PathUtil.Resolve("%DESKTOP_COMMON%"), 0);
                if (response.Data != null)
                    paths.AddRange(response.Data);

                this._logger.Log($"Collected {paths.Count} desktop items to append.", TraceContext.TraceId, LogLevel.Info);

                foreach (var path in paths)
                {
                    this._logger.Log($"Appending desktop item: {path}", TraceContext.TraceId, LogLevel.Trace);
                    AppendPackItemFromPath(packId, path);
                }

                var result = GetPackItems(packId).Data ?? new List<PackItem>();
                this._logger.Log($"Added desktop icons successfully. Total PackItems: {result.Count}", TraceContext.TraceId, LogLevel.Info);

                return result;
            });
        }

        public ApiResponse<object> ApplyPackOperations(string packId)
        {
            return ExecuteSafe(() =>
            {
                this._logger.Log("Entering ApplyPackOperations()", TraceContext.TraceId, LogLevel.Trace);
                this._logger.Log($"Parameters: PackId={packId}", TraceContext.TraceId, LogLevel.Debug);

                PackConfig config = GetPackConfig(packId).Data;
                AppConfig appConfig = GlobalContext.AppConfig;
                List<PackItem> packItems = GetPackItems(packId).Data ?? new();

                List<string> possibleLnkFiles = new();
                List<string> possibleUrlFiles = new();

                this._logger.Log("Scanning special folders for .lnk and .url files...", TraceContext.TraceId, LogLevel.Debug);

                foreach (string specialPath in EnvUtil.SpecialFolders)
                {
                    var result = _fileController.GetFilesUnderPath(
                        PathUtil.Resolve(specialPath),
                        new[] { "lnk", ".lnk", "url", ".url" },
                        GlobalContext.AppConfig.RecursiveScanningDepth
                    ).Data ?? new();

                    possibleLnkFiles.AddRange(result.Where(p => p.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase)));
                    possibleUrlFiles.AddRange(result.Where(p => p.EndsWith(".url", StringComparison.OrdinalIgnoreCase)));
                }

                this._logger.Log($"Collected {possibleLnkFiles.Count} .lnk and {possibleUrlFiles.Count} .url files.", TraceContext.TraceId, LogLevel.Debug);

                foreach (PackItem packItem in packItems)
                {
                    string processedDir = Path.Combine(ApplicationPathContext.AppIconsFolderPath, packId);
                    if (!Directory.Exists(processedDir))
                        Directory.CreateDirectory(processedDir);

                    string originalPng = Path.Combine(ApplicationPathContext.AppPacksFolderPath, packId, ApplicationPathContext.PACK_ICONS_FOLDER_NAME, $"{packItem.Uid}.png");

                    string processedBase64 = ProcessPngImage(originalPng, config.CornerRadius, config.Opacity);
                    if (string.IsNullOrEmpty(processedBase64))
                    {
                        this._logger.Log($"Skipping PackItem '{packItem.Name}' — ProcessPngImage returned empty.", TraceContext.TraceId, LogLevel.Warning);
                        continue;
                    }

                    string processedIco = Path.Combine(processedDir, $"{Guid.NewGuid()}.ico");
                    var writeResp = _fileController.WriteBase64(processedIco, processedBase64, true);
                    if (!writeResp.Success)
                    {
                        this._logger.Log($"Failed to write processed icon for '{packItem.Name}'.", TraceContext.TraceId, LogLevel.Warning);
                        continue;
                    }

                    this._logger.Log($"Processing icon applied for PackItem '{packItem.Name}'", TraceContext.TraceId, LogLevel.Debug);

                    if (packItem.IsLnkItem)
                        HandleLnkPackItem(packItem, processedIco, possibleLnkFiles, config, appConfig, packId);
                    else if (packItem.IsUrlItem)
                        HandleUrlPackItem(packItem, processedIco, possibleUrlFiles, config, appConfig, packId);
                    else if (packItem.IsDirItem)
                        HandleDirPackItems(packItem, processedIco, config, appConfig, packId);
                }

                this._logger.Log("Refreshing Windows shell after icon application.", TraceContext.TraceId, LogLevel.Info);
                _packOperationController.RefreshWindowsShell();

                if (appConfig.ForceExplorerRefreshAfterIcoChange)
                {
                    this._logger.Log("Aggressively refreshing Windows Explorer icons — may cause lag on low-end hardware.", TraceContext.TraceId, LogLevel.Warning);
                    _packOperationController.ForceExplorerIconRefresh();
                }

                this._logger.Log("ApplyPackOperations() completed successfully.", TraceContext.TraceId, LogLevel.Info);
                return new object();
            });
        }

        public ApiResponse<List<PackConfig>> GetAllPackConfigs()
        {
            return ExecuteSafe(() =>
            {
                this._logger.Log("Entering GetAllPackConfigs()", TraceContext.TraceId, LogLevel.Trace);

                List<PackConfig> packConfigs = new();
                string packsFolder = ApplicationPathContext.AppPacksFolderPath;

                if (!Directory.Exists(packsFolder))
                {
                    this._logger.Log("No packs directory found.", TraceContext.TraceId, LogLevel.Warning);
                    return packConfigs;
                }

                string[] packDirs = Directory.GetDirectories(packsFolder);
                this._logger.Log($"Discovered {packDirs.Length} pack directories.", TraceContext.TraceId, LogLevel.Debug);

                foreach (string packDir in packDirs)
                {
                    string packId = Path.GetFileName(packDir);
                    try
                    {
                        PackConfig cfg = GetPackConfig(packId).Data;
                        if (cfg != null)
                        {
                            packConfigs.Add(cfg);
                            this._logger.Log($"Loaded PackConfig: {cfg.PackName} ({packId})", TraceContext.TraceId, LogLevel.Trace);
                        }
                    }
                    catch (AppException ex)
                    {
                        this._logger.Log($"Skipping corrupted pack '{packId}': {ex.Message}", TraceContext.TraceId, LogLevel.Warning);
                    }
                }

                this._logger.Log($"GetAllPackConfigs() completed — Total valid configs: {packConfigs.Count}", TraceContext.TraceId, LogLevel.Info);
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
            this._logger.Log($"Entering HandleLnkPackItem() for PackItem '{packItem.Name}'", TraceContext.TraceId, LogLevel.Trace);

            string? firstPart = packItem.TargetPath.Split("\\").FirstOrDefault();
            string remainingPart = packItem.TargetPath.Substring(firstPart?.Length ?? 0).TrimStart('\\');
            this._logger.Log($"Resolved TargetPath parts: FirstPart='{firstPart}', Remaining='{remainingPart}'", TraceContext.TraceId, LogLevel.Debug);

            List<string> matches = new();

            if (!string.IsNullOrEmpty(firstPart))
            {
                matches = _fileController.GetMatchingFileSystemEntries(
                    PathUtil.Resolve(firstPart),
                    remainingPart
                ).Data ?? new();
                this._logger.Log($"Initial LNK matches found: {matches.Count}", TraceContext.TraceId, LogLevel.Debug);
            }

            if (matches.Count == 0 && appConfig.MatchLnkByTargetExe)
            {
                this._logger.Log("No direct matches found. Falling back to MatchLnkByTargetExe mode.", TraceContext.TraceId, LogLevel.Warning);

                foreach (string lnkFile in possibleLnkFiles)
                {
                    LnkMetaData metaData = _fileController.GetLnkMetaData(lnkFile).Data;
                    if (metaData == null) continue;

                    if (_fileController.MatchesGlobPattern(metaData.TargetPath, PathUtil.Resolve(packItem.TargetExePath)))
                        matches.Add(lnkFile);
                }

                this._logger.Log($"Fallback LNK matches found by TargetExe: {matches.Count}", TraceContext.TraceId, LogLevel.Debug);
            }

            ApplyLnkOperations(matches, processedIconPath, packItem, appConfig);
            this._logger.Log($"HandleLnkPackItem completed for '{packItem.Name}'.", TraceContext.TraceId, LogLevel.Info);
        }

        private void HandleUrlPackItem(
            PackItem packItem,
            string processedIconPath,
            List<string> possibleUrlFiles,
            PackConfig config,
            AppConfig appConfig,
            string packId)
        {
            this._logger.Log($"Entering HandleUrlPackItem() for PackItem '{packItem.Name}'", TraceContext.TraceId, LogLevel.Trace);

            string? firstPart = packItem.TargetPath.Split("\\").FirstOrDefault();
            string remainingPart = packItem.TargetPath.Substring(firstPart?.Length ?? 0).TrimStart('\\');
            this._logger.Log($"Resolved TargetPath parts: FirstPart='{firstPart}', Remaining='{remainingPart}'", TraceContext.TraceId, LogLevel.Debug);

            List<string> matches = new();

            if (!string.IsNullOrEmpty(firstPart))
            {
                matches = _fileController.GetMatchingFileSystemEntries(
                    PathUtil.Resolve(firstPart),
                    remainingPart
                ).Data ?? new();
                this._logger.Log($"Initial URL matches found: {matches.Count}", TraceContext.TraceId, LogLevel.Debug);
            }

            if (matches.Count == 0 && appConfig.MatchUrlByTargetUrl)
            {
                this._logger.Log("No direct matches found. Falling back to MatchUrlByTargetUrl mode.", TraceContext.TraceId, LogLevel.Warning);

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

                this._logger.Log($"Fallback URL matches found by TargetUrl: {matches.Count}", TraceContext.TraceId, LogLevel.Debug);
            }

            ApplyUrlOperations(matches, processedIconPath, packItem, appConfig);
            this._logger.Log($"HandleUrlPackItem completed for '{packItem.Name}'.", TraceContext.TraceId, LogLevel.Info);
        }

        private void HandleDirPackItems(
            PackItem packItem,
            string processedIconPath,
            PackConfig config,
            AppConfig appConfig,
            string packId)
        {
            this._logger.Log($"Entering HandleDirPackItems() for PackItem '{packItem.Name}'", TraceContext.TraceId, LogLevel.Trace);

            string? firstPart = packItem.TargetPath.Split("\\").FirstOrDefault();
            string remainingPart = packItem.TargetPath.Substring(firstPart?.Length ?? 0).TrimStart('\\');
            this._logger.Log($"Resolved TargetPath parts: FirstPart='{firstPart}', Remaining='{remainingPart}'", TraceContext.TraceId, LogLevel.Debug);

            List<string> matches = new();

            if (!string.IsNullOrEmpty(firstPart))
            {
                matches = _fileController.GetMatchingFileSystemEntries(
                    PathUtil.Resolve(firstPart),
                    remainingPart
                ).Data ?? new();
                this._logger.Log($"Directory matches found: {matches.Count}", TraceContext.TraceId, LogLevel.Debug);
            }

            ApplyDirOperations(matches, processedIconPath, packItem, appConfig);
            this._logger.Log($"HandleDirPackItems completed for '{packItem.Name}'.", TraceContext.TraceId, LogLevel.Info);
        }

        private void ApplyLnkOperations(List<string> matches, string processedIconPath, PackItem packItem, AppConfig appConfig)
        {
            this._logger.Log($"Applying LNK operations. Matches={matches.Count}, Icon={processedIconPath}", TraceContext.TraceId, LogLevel.Trace);
            if (matches.Count == 0)
            {
                this._logger.Log("No matching LNK files found. Skipping operation.", TraceContext.TraceId, LogLevel.Warning);
                return;
            }

            foreach (string match in matches)
            {
                _packOperationController.ChangeLnkIconPath(match, processedIconPath);
                this._logger.Log($"Changed LNK icon path: {match}", TraceContext.TraceId, LogLevel.Debug);

                if (appConfig.ChangeDescriptionOfMatchedLnkFiles)
                {
                    _packOperationController.ChangeLnkDescription(match, packItem.Description);
                    this._logger.Log($"Updated LNK description for: {match}", TraceContext.TraceId, LogLevel.Trace);
                }
            }
            this._logger.Log("ApplyLnkOperations completed successfully.", TraceContext.TraceId, LogLevel.Info);
        }

        private void ApplyUrlOperations(List<string> matches, string processedIconPath, PackItem packItem, AppConfig appConfig)
        {
            this._logger.Log($"Applying URL operations. Matches={matches.Count}, Icon={processedIconPath}", TraceContext.TraceId, LogLevel.Trace);
            if (matches.Count == 0)
            {
                this._logger.Log("No matching URL files found. Skipping operation.", TraceContext.TraceId, LogLevel.Warning);
                return;
            }

            foreach (string match in matches)
            {
                _packOperationController.ChangeIconPathOfUrlFile(match, processedIconPath);
                this._logger.Log($"Changed URL icon path: {match}", TraceContext.TraceId, LogLevel.Debug);
            }

            this._logger.Log("ApplyUrlOperations completed successfully.", TraceContext.TraceId, LogLevel.Info);
        }

        private void ApplyDirOperations(List<string> matches, string processedIconPath, PackItem packItem, AppConfig appConfig)
        {
            this._logger.Log($"Applying directory operations. Matches={matches.Count}, Icon={processedIconPath}", TraceContext.TraceId, LogLevel.Trace);
            if (matches.Count == 0)
            {
                this._logger.Log("No matching directories found. Skipping operation.", TraceContext.TraceId, LogLevel.Warning);
                return;
            }

            foreach (string match in matches)
            {
                _packOperationController.ChangeDirectoryIcon(match, processedIconPath);
                this._logger.Log($"Applied directory icon to: {match}", TraceContext.TraceId, LogLevel.Debug);
            }

            this._logger.Log("ApplyDirOperations completed successfully.", TraceContext.TraceId, LogLevel.Info);
        }

        public string ProcessPngImage(string pngPath, float cornerRadius, float opacityAmount)
        {
            this._logger.Log($"Entering ProcessPngImage() for '{pngPath}'", TraceContext.TraceId, LogLevel.Trace);
            this._logger.Log($"CornerRadius={cornerRadius}, Opacity={opacityAmount}", TraceContext.TraceId, LogLevel.Debug);

            string base64 = _fileController.GetBase64(pngPath).Data;
            if (string.IsNullOrEmpty(base64))
            {
                this._logger.Log("Failed to get base64 from PNG.", TraceContext.TraceId, LogLevel.Error);
                return string.Empty;
            }

            base64 = _externalController.ApplyCornerRadiusOnPngBase64(base64, cornerRadius).Data;
            if (string.IsNullOrEmpty(base64))
            {
                this._logger.Log("Corner radius operation failed.", TraceContext.TraceId, LogLevel.Error);
                return string.Empty;
            }

            base64 = _externalController.ApplyOpacityOnPngBase64(base64, opacityAmount).Data;
            if (string.IsNullOrEmpty(base64))
            {
                this._logger.Log("Opacity operation failed.", TraceContext.TraceId, LogLevel.Error);
                return string.Empty;
            }

            base64 = _externalController.PngBase64ToIcoBase64(base64).Data;
            if (string.IsNullOrEmpty(base64))
            {
                this._logger.Log("PNG to ICO conversion failed.", TraceContext.TraceId, LogLevel.Error);
                return string.Empty;
            }

            this._logger.Log("ProcessPngImage completed successfully.", TraceContext.TraceId, LogLevel.Info);
            return base64;
        }

        public void ValidatePackConfig(PackConfig config)
        {
            this._logger.Log("Entering ValidatePackConfig()", TraceContext.TraceId, LogLevel.Trace);
            this._logger.Log($"PackName='{config.PackName}', Version='{config.Version}'", TraceContext.TraceId, LogLevel.Debug);

            if (string.IsNullOrEmpty(config.PackName))
            {
                this._logger.Log("PackName is empty — throwing AppException.", TraceContext.TraceId, LogLevel.Error);
                throw AppException.Operational(
                    userMessage: _l10nService.GetLocalizedMessage("Errors.PackNameEmpty"),
                    logMessage: "PackConfig validation failed: PackName is null or empty.",
                    level: LogLevel.Error
                );
            }

            string normalizedVersion = config.Version.StartsWith("v")
                ? config.Version.Substring(1)
                : config.Version;
            normalizedVersion = normalizedVersion.Trim();

            string versionPattern = @"^\d+\.\d+\.\d+$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(config.Version, versionPattern))
            {
                this._logger.Log("Version format invalid — expected x.y.z", TraceContext.TraceId, LogLevel.Error);
                throw AppException.Operational(
                    userMessage: _l10nService.GetLocalizedMessage("Errors.VersionFormatInvalid"),
                    logMessage: "PackConfig validation failed: Version format is invalid.",
                    level: LogLevel.Error
                );
            }

            this._logger.Log("ValidatePackConfig() passed successfully.", TraceContext.TraceId, LogLevel.Info);
        }

    }
}
