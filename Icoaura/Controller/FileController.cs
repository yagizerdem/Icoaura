
using Icoaura.Enum;
using Icoaura.Exception;
using Icoaura.Model;
using Icoaura.Util;
using IWshRuntimeLibrary;
using Model.DTO;
using System.IO;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using System.Windows.Media.Animation;
using Icoaura.Context;

namespace Icoaura.Controller
{
    public class FileController : BaseController
    {
        public FileController()
        {


        }

        public ApiResponse<string> ReadFileContentAsText(string path)
        {
            return ExecuteSafe(() =>
            {
                this._logger.Log("Entering ReadFileContentAsText()", TraceContext.TraceId, LogLevel.Trace);

                FileUtil.EnsureFileExist(path, Enum.LogLevel.Error);
                this._logger.Log($"File exists, proceeding to read content: {path}", TraceContext.TraceId, LogLevel.Debug);

                string serialized = System.IO.File.ReadAllText(path);
                this._logger.Log($"Successfully read text content from file: {path}", TraceContext.TraceId, LogLevel.Info);

                return serialized;
            });
        }

        public ApiResponse<string> GetBase64(string path)
        {
            return ExecuteSafe(() =>
            {
                this._logger.Log("Entering GetBase64()", TraceContext.TraceId, LogLevel.Trace);

                FileUtil.EnsureFileExist(path, Enum.LogLevel.Error);
                this._logger.Log($"File exists, reading bytes for Base64 conversion: {path}", TraceContext.TraceId, LogLevel.Debug);

                byte[] buffer = System.IO.File.ReadAllBytes(path);
                string base64 = Convert.ToBase64String(buffer);
                string mimeType = FileUtil.GetMimeTypeFromAbsolutePath(path);
                string base64WithHeaders = Base64Util.AddHeadersToBase64(base64, mimeType);

                this._logger.Log($"Base64 encoding successful. MIME type: {mimeType}", TraceContext.TraceId, LogLevel.Info);

                return base64WithHeaders;
            });
        }
        public ApiResponse<object> DeleteFile(string path)
        {
            return ExecuteSafe(() =>
            {
                this._logger.Log("Entering DeleteFile()", TraceContext.TraceId, LogLevel.Trace);

                if (System.IO.File.Exists(path))
                {
                    this._logger.Log($"Deleting file: {path}", TraceContext.TraceId, LogLevel.Debug);
                    System.IO.File.Delete(path);
                    this._logger.Log($"File deleted successfully: {path}", TraceContext.TraceId, LogLevel.Info);
                }
                else
                {
                    this._logger.Log($"File not found, nothing to delete: {path}", TraceContext.TraceId, LogLevel.Warning);
                }

                return new object();
            });
        }


        public ApiResponse<object> WriteBase64(string path, string base64, bool overwrite = true)
        {
            return ExecuteSafe(() =>
            {
                this._logger.Log("Entering WriteBase64()", TraceContext.TraceId, LogLevel.Trace);

                // --- Validation ---
                if (string.IsNullOrWhiteSpace(path))
                {
                    this._logger.Log("Empty file path provided for WriteBase64.", TraceContext.TraceId, LogLevel.Warning);
                    throw AppException.Operational(
                        userMessage: this._l10nService.GetLocalizedMessage("Errors.FilePathEmpty"),
                        logMessage: "Attempted to write Base64 data to an empty file path.",
                        level: LogLevel.Error
                    );
                }

                if (string.IsNullOrWhiteSpace(base64))
                {
                    this._logger.Log("Empty Base64 data provided for WriteBase64.", TraceContext.TraceId, LogLevel.Warning);
                    throw AppException.Operational(
                        userMessage: this._l10nService.GetLocalizedMessage("Errors.Base64DataEmpty"),
                        logMessage: "Attempted to write empty Base64 data to file.",
                        level: LogLevel.Error
                    );
                }

                // --- File existence logic ---
                if (System.IO.File.Exists(path))
                {
                    if (!overwrite)
                    {
                        this._logger.Log($"File already exists and overwrite=false: {path}", TraceContext.TraceId, LogLevel.Warning);
                        throw AppException.Operational(
                            userMessage: $"{this._l10nService.GetLocalizedMessage("Errors.FileAlreadyExists")} : ${Path.GetFileName(path)}",
                            logMessage: $"Attempted to overwrite file '{path}' but overwrite=false",
                            level: LogLevel.Error
                        );
                    }

                    this._logger.Log($"Overwriting existing file: {path}", TraceContext.TraceId, LogLevel.Debug);
                    System.IO.File.Delete(path);
                }

                // --- Base64 conversion ---
                this._logger.Log("Removing Base64 headers and decoding data...", TraceContext.TraceId, LogLevel.Debug);
                string normalized = Base64Util.RemoveHeadersFromBase64(base64);

                byte[] buffer;
                try
                {
                    buffer = Convert.FromBase64String(normalized);
                    this._logger.Log("Base64 decoded successfully.", TraceContext.TraceId, LogLevel.Info);
                }
                catch (FormatException ex)
                {
                    this._logger.Log($"Base64 decoding failed: {ex.Message}", TraceContext.TraceId, LogLevel.Error);
                    throw AppException.Operational(
                        userMessage: this._l10nService.GetLocalizedMessage("Error.Base64DataInvalid"),
                        logMessage: ex.Message,
                        level: LogLevel.Error
                    );
                }

                this._logger.Log($"Writing decoded bytes to file: {path}", TraceContext.TraceId, LogLevel.Debug);
                System.IO.File.WriteAllBytes(path, buffer);
                this._logger.Log($"File write operation successful: {path}", TraceContext.TraceId, LogLevel.Info);

                return new object();
            });
        }


        public ApiResponse<LnkMetaData> GetLnkMetaData(string path)
        {
            return ExecuteSafe<LnkMetaData>(() =>
            {
                this._logger.Log("Entering GetLnkMetaData()", TraceContext.TraceId, LogLevel.Trace);

                FileUtil.EnsureFileExist(path, Enum.LogLevel.Error);
                FileUtil.EnsureFileHasExtension(path, [".lnk", "lnk"], Enum.LogLevel.Error);
                this._logger.Log($"Valid .lnk file verified: {path}", TraceContext.TraceId, LogLevel.Debug);

                var shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(path);
                this._logger.Log($"Shortcut loaded successfully from path: {path}", TraceContext.TraceId, LogLevel.Info);

                // --- File info for timestamps ---
                var fi = new FileInfo(path);
                this._logger.Log($"FileInfo collected. Created: {fi.CreationTimeUtc}, Modified: {fi.LastWriteTimeUtc}", TraceContext.TraceId, LogLevel.Debug);

                // --- Parse icon info ---
                string iconLocation = shortcut.IconLocation ?? string.Empty;
                string iconPath = iconLocation;
                int iconIndex = 0;

                if (iconLocation.Contains(","))
                {
                    var parts = iconLocation.Split(',', StringSplitOptions.TrimEntries);
                    iconPath = parts[0];
                    if (parts.Length > 1 && int.TryParse(parts[1], out int parsed))
                        iconIndex = parsed;
                }

                // --- Target info ---
                string targetPath = shortcut.TargetPath ?? string.Empty;
                bool targetExists = System.IO.File.Exists(targetPath) || Directory.Exists(targetPath);
                string targetMimeType = string.Empty;
                string targetExtension = string.Empty;

                if (targetExists)
                {
                    targetMimeType = FileUtil.GetMimeTypeFromAbsolutePath(targetPath);
                    targetExtension = Path.GetExtension(targetPath).ToLowerInvariant();
                    this._logger.Log($"Target found: {targetPath} | MIME: {targetMimeType}", TraceContext.TraceId, LogLevel.Debug);
                }
                else
                {
                    this._logger.Log($"Target path not found or invalid: {targetPath}", TraceContext.TraceId, LogLevel.Warning);
                }

                var meta = new LnkMetaData
                {
                    ShortcutPath = path,
                    TargetPath = targetPath,
                    WorkingDirectory = shortcut.WorkingDirectory ?? string.Empty,
                    Arguments = shortcut.Arguments ?? string.Empty,
                    Description = shortcut.Description ?? string.Empty,
                    IconPath = iconPath,
                    IconIndex = iconIndex,
                    CreationTime = fi.CreationTimeUtc,
                    LastModifiedTime = fi.LastWriteTimeUtc,
                    LastAccessedTime = fi.LastAccessTimeUtc,
                    IsTargetValid = targetExists,
                    TargetMimeType = targetMimeType,
                    TargetExtension = targetExtension
                };

                this._logger.Log($"Returning LnkMetaData for: {path}", TraceContext.TraceId, LogLevel.Info);
                return meta;
            });
        }



        public ApiResponse<UrlMetaData> GetUrlMetaData(string path)
        {
            return ExecuteSafe<UrlMetaData>(() =>
            {
                this._logger.Log("Entering GetUrlMetaData()", TraceContext.TraceId, LogLevel.Trace);

                FileUtil.EnsureFileExist(path, Enum.LogLevel.Error);
                FileUtil.EnsureFileHasExtension(path, [".url", "url"], Enum.LogLevel.Error);
                this._logger.Log($"Validated .url file: {path}", TraceContext.TraceId, LogLevel.Debug);

                var lines = System.IO.File.ReadAllLines(path);
                string? targetUrl = null;
                string? iconPath = null;

                foreach (string line in lines)
                {
                    if (line.StartsWith("URL=", StringComparison.OrdinalIgnoreCase))
                        targetUrl = line.Substring(4).Trim();

                    if (line.StartsWith("IconFile=", StringComparison.OrdinalIgnoreCase))
                        iconPath = line.Substring(9).Trim();
                }

                var metaData = new UrlMetaData
                {
                    ShortcutPath = path,
                    IconPath = iconPath ?? string.Empty,
                    Url = targetUrl ?? string.Empty
                };

                this._logger.Log($"Extracted URL shortcut. TargetUrl: {metaData.Url}, Icon: {metaData.IconPath}", TraceContext.TraceId, LogLevel.Info);
                return metaData;
            });
        }


        public ApiResponse<DirMetaData> GetDirMetaData(string path)
        {
            return ExecuteSafe(() =>
            {
                this._logger.Log("Entering GetDirMetaData()", TraceContext.TraceId, LogLevel.Trace);

                FileUtil.EnsureDirectoryExist(path);
                this._logger.Log($"Verified directory exists: {path}", TraceContext.TraceId, LogLevel.Debug);

                var meta = new DirMetaData
                {
                    DirectoryPath = path,
                    IconPath = string.Empty
                };

                string desktopIniPath = Path.Combine(path, "desktop.ini");
                if (System.IO.File.Exists(desktopIniPath))
                {
                    this._logger.Log($"Found desktop.ini in directory: {desktopIniPath}", TraceContext.TraceId, LogLevel.Debug);

                    string? iconResourceLine = System.IO.File.ReadAllLines(desktopIniPath)
                        .FirstOrDefault(line => line.StartsWith("IconResource=", StringComparison.OrdinalIgnoreCase));

                    if (!string.IsNullOrWhiteSpace(iconResourceLine))
                    {
                        string iconPath = iconResourceLine
                            .Substring("IconResource=".Length)
                            .Split(',')[0]
                            .Trim();

                        if (System.IO.File.Exists(iconPath))
                        {
                            meta.IconPath = iconPath;
                            this._logger.Log($"Custom icon found for directory: {iconPath}", TraceContext.TraceId, LogLevel.Info);
                        }
                        else
                        {
                            this._logger.Log($"IconResource file path invalid: {iconPath}", TraceContext.TraceId, LogLevel.Warning);
                        }
                    }
                    else
                    {
                        this._logger.Log("desktop.ini found but no IconResource entry detected.", TraceContext.TraceId, LogLevel.Warning);
                    }
                }
                else
                {
                    this._logger.Log("No desktop.ini found — using default folder icon.", TraceContext.TraceId, LogLevel.Info);
                }

                return meta;
            });
        }


        public ApiResponse<List<string>> GetFilesUnderPath(
            string path,
            string[] allowedExtensions,
            int depth)
        {
            return ExecuteSafe(() =>
            {
                this._logger.Log("Entering GetFilesUnderPath()", TraceContext.TraceId, LogLevel.Trace);
                this._logger.Log($"Parameters: Path={path}, Depth={depth}, AllowedExtensions=[{string.Join(',', allowedExtensions ?? Array.Empty<string>())}]", TraceContext.TraceId, LogLevel.Debug);

                FileUtil.EnsureDirectoryExist(path, LogLevel.Error);
                this._logger.Log($"Verified directory exists: {path}", TraceContext.TraceId, LogLevel.Debug);

                if (allowedExtensions == null || allowedExtensions.Length == 0)
                {
                    this._logger.Log("No allowed extensions provided — returning empty list.", TraceContext.TraceId, LogLevel.Warning);
                    return new();
                }

                if (depth < 0)
                {
                    this._logger.Log($"Invalid depth value detected: {depth}", TraceContext.TraceId, LogLevel.Error);
                    throw AppException.Operational(
                        userMessage: this._l10nService.GetLocalizedMessage("Errors.DepthNegative"),
                        logMessage: $"Invalid depth value: {depth}",
                        level: LogLevel.Error
                    );
                }

                var result = new List<string>();
                var stack = new Stack<(string dir, int level)>();
                stack.Push((path, 0));

                bool matchAll = allowedExtensions.Any(e => e == "*" || e == ".*");
                this._logger.Log($"File extension matching mode: {(matchAll ? "MatchAll" : "Filtered")}", TraceContext.TraceId, LogLevel.Debug);

                while (stack.Count > 0)
                {
                    var (currentDir, currentLevel) = stack.Pop();
                    this._logger.Log($"Scanning directory: {currentDir} (Level {currentLevel})", TraceContext.TraceId, LogLevel.Trace);

                    if (currentLevel > depth)
                        continue;

                    try
                    {
                        foreach (var file in Directory.GetFiles(currentDir))
                        {
                            string ext = Path.GetExtension(file).ToLowerInvariant();

                            if (matchAll || allowedExtensions.Any(e =>
                                string.Equals(e.TrimStart('.'), ext.TrimStart('.'), StringComparison.OrdinalIgnoreCase)))
                            {
                                result.Add(file);
                                this._logger.Log($"File matched: {file}", TraceContext.TraceId, LogLevel.Debug);
                            }
                        }

                        if (currentLevel < depth)
                        {
                            foreach (var dir in Directory.GetDirectories(currentDir))
                            {
                                stack.Push((dir, currentLevel + 1));
                                this._logger.Log($"Queued subdirectory for scan: {dir}", TraceContext.TraceId, LogLevel.Trace);
                            }
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        this._logger.Log($"Access denied to directory: {currentDir}", TraceContext.TraceId, LogLevel.Warning);
                        continue;
                    }
                    catch (IOException ex)
                    {
                        this._logger.Log($"IOException while accessing {currentDir}: {ex.Message}", TraceContext.TraceId, LogLevel.Error);
                        throw AppException.Operational(
                            userMessage: this._l10nService.GetLocalizedMessage("Errors.IOException"),
                            logMessage: ex.Message,
                            level: LogLevel.Error
                        );
                    }
                }

                this._logger.Log($"GetFilesUnderPath completed — Total matched files: {result.Count}", TraceContext.TraceId, LogLevel.Info);
                return result;
            });
        }

        public ApiResponse<List<string>> GetFoldersUnderPath(string path, int depth)
        {
            return ExecuteSafe(() =>
            {
                this._logger.Log("Entering GetFoldersUnderPath()", TraceContext.TraceId, LogLevel.Trace);
                this._logger.Log($"Parameters: Path={path}, Depth={depth}", TraceContext.TraceId, LogLevel.Debug);

                // --- Validation ---
                FileUtil.EnsureDirectoryExist(path, LogLevel.Error);

                if (depth < 0)
                {
                    this._logger.Log($"Invalid depth value: {depth}", TraceContext.TraceId, LogLevel.Error);
                    throw AppException.Operational(
                        userMessage: "Depth cannot be negative.",
                        logMessage: $"Invalid depth value: {depth}",
                        level: LogLevel.Error
                    );
                }

                var result = new List<string>();
                var stack = new Stack<(string dir, int level)>();
                stack.Push((path, 0));

                while (stack.Count > 0)
                {
                    var (currentDir, currentLevel) = stack.Pop();

                    if (currentLevel > depth)
                        continue;

                    try
                    {
                        foreach (var dir in Directory.GetDirectories(currentDir))
                        {
                            result.Add(dir);
                            this._logger.Log($"Found subdirectory: {dir}", TraceContext.TraceId, LogLevel.Debug);

                            if (currentLevel < depth)
                                stack.Push((dir, currentLevel + 1));
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        this._logger.Log($"Access denied to directory: {currentDir}", TraceContext.TraceId, LogLevel.Warning);
                        continue;
                    }
                    catch (IOException ex)
                    {
                        this._logger.Log($"I/O error in directory '{currentDir}': {ex.Message}", TraceContext.TraceId, LogLevel.Error);
                        throw AppException.Operational(
                            userMessage: "An I/O error occurred while scanning directories.",
                            logMessage: ex.Message,
                            level: LogLevel.Error
                        );
                    }
                }

                this._logger.Log($"Returning {result.Count} folders found under '{path}'", TraceContext.TraceId, LogLevel.Info);
                return result;
            });
        }


        public ApiResponse<List<string>> GetMatchingFileSystemEntries(string rootPath, string pattern)
        {
            return ExecuteSafe(() =>
            {
                this._logger.Log("Entering GetMatchingFileSystemEntries()", TraceContext.TraceId, LogLevel.Trace);
                this._logger.Log($"Parameters: RootPath={rootPath}, Pattern={pattern}", TraceContext.TraceId, LogLevel.Debug);

                if (!Directory.Exists(rootPath))
                {
                    this._logger.Log($"Root directory not found: {rootPath}", TraceContext.TraceId, LogLevel.Error);
                    throw AppException.Operational(
                        userMessage: this._l10nService.GetLocalizedMessage("Errors.DirectoryNotFound"),
                        logMessage: $"GetMatchingFileSystemEntries failed: root path not found ({rootPath})",
                        level: LogLevel.Error
                    );
                }

                var results = new List<string>();
                pattern = pattern.Replace('/', '\\');
                var searchOption = pattern.Contains("**") ? SearchOption.AllDirectories : SearchOption.AllDirectories;

                this._logger.Log($"Resolved pattern: {pattern} (SearchOption={searchOption})", TraceContext.TraceId, LogLevel.Debug);

                var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);
                matcher.AddInclude(pattern);

                var directoryInfo = new DirectoryInfoWrapper(new DirectoryInfo(rootPath));
                var matchResult = matcher.Execute(directoryInfo);

                foreach (var match in matchResult.Files)
                {
                    string fullPath = Path.GetFullPath(Path.Combine(rootPath, match.Path.Replace('/', '\\')));
                    results.Add(fullPath);
                    this._logger.Log($"Matched file: {fullPath}", TraceContext.TraceId, LogLevel.Debug);
                }

                bool isSingleSegment = !pattern.Contains("\\") && !pattern.Contains("/");
                if (isSingleSegment)
                {
                    foreach (var dir in Directory.EnumerateDirectories(rootPath, "*", SearchOption.TopDirectoryOnly))
                    {
                        string dirName = Path.GetFileName(dir);
                        if (MatchesSimplePattern(dirName, pattern))
                        {
                            results.Add(Path.GetFullPath(dir));
                            this._logger.Log($"Matched single-segment directory: {dir}", TraceContext.TraceId, LogLevel.Debug);
                        }
                    }
                }
                else
                {
                    foreach (var dir in Directory.EnumerateDirectories(rootPath, "*", SearchOption.AllDirectories))
                    {
                        string relativePath = Path.GetRelativePath(rootPath, dir);
                        if (MatchesGlobPattern(relativePath, pattern))
                        {
                            results.Add(Path.GetFullPath(dir));
                            this._logger.Log($"Matched directory: {dir}", TraceContext.TraceId, LogLevel.Debug);
                        }
                    }
                }

                var distinctResults = results
                    .Select(p => Path.GetFullPath(p.Replace('/', '\\')))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                this._logger.Log($"Returning {distinctResults.Count} total matches.", TraceContext.TraceId, LogLevel.Info);
                return distinctResults;
            });
        }


        public bool MatchesGlobPattern(string path, string pattern)
        {
            this._logger.Log($"Checking MatchesGlobPattern: Path={path}, Pattern={pattern}", TraceContext.TraceId, LogLevel.Trace);

            path = path.Replace('/', '\\').Trim('\\');
            pattern = pattern.Replace('/', '\\').Trim('\\');

            if (path.Equals(pattern, StringComparison.OrdinalIgnoreCase))
            {
                this._logger.Log("Exact match found.", TraceContext.TraceId, LogLevel.Debug);
                return true;
            }

            string[] pathParts = path.Split('\\', StringSplitOptions.RemoveEmptyEntries);
            string[] patternParts = pattern.Split('\\', StringSplitOptions.RemoveEmptyEntries);

            if (!pattern.Contains("*") && pathParts.Length != patternParts.Length)
            {
                this._logger.Log("Different segment count and no wildcard — not matched.", TraceContext.TraceId, LogLevel.Debug);
                return false;
            }

            bool result = MatchesPatternParts(pathParts, patternParts, 0, 0);
            this._logger.Log($"MatchesGlobPattern result: {result}", TraceContext.TraceId, LogLevel.Info);
            return result;
        }


        public bool MatchesPatternParts(string[] pathParts, string[] patternParts, int pathIndex, int patternIndex)
        {
            if (pathIndex >= pathParts.Length && patternIndex >= patternParts.Length)
                return true;

            if (patternIndex >= patternParts.Length)
                return false;

            if (pathIndex >= pathParts.Length)
            {
                for (int i = patternIndex; i < patternParts.Length; i++)
                {
                    if (patternParts[i] != "**")
                        return false;
                }
                return true;
            }

            string currentPattern = patternParts[patternIndex];
            string currentPath = pathParts[pathIndex];

            if (currentPattern == "**")
            {
                if (patternIndex == patternParts.Length - 1)
                    return true;

                if (MatchesPatternParts(pathParts, patternParts, pathIndex, patternIndex + 1))
                    return true;

                return MatchesPatternParts(pathParts, patternParts, pathIndex + 1, patternIndex);
            }

            if (currentPattern == "*" ||
                System.IO.Enumeration.FileSystemName.MatchesSimpleExpression(currentPattern, currentPath, ignoreCase: true))
            {
                return MatchesPatternParts(pathParts, patternParts, pathIndex + 1, patternIndex + 1);
            }

            if (currentPattern.Equals(currentPath, StringComparison.OrdinalIgnoreCase))
            {
                return MatchesPatternParts(pathParts, patternParts, pathIndex + 1, patternIndex + 1);
            }

            return false;
        }


        public bool MatchesSimplePattern(string name, string pattern)
        {
            this._logger.Log($"Checking MatchesSimplePattern: Name={name}, Pattern={pattern}", TraceContext.TraceId, LogLevel.Trace);

            if (name.Equals(pattern, StringComparison.OrdinalIgnoreCase))
            {
                this._logger.Log("Exact filename match found.", TraceContext.TraceId, LogLevel.Debug);
                return true;
            }

            if (pattern.Contains("*") || pattern.Contains("?"))
            {
                bool match = System.IO.Enumeration.FileSystemName.MatchesSimpleExpression(
                    pattern,
                    name,
                    ignoreCase: true
                );
                this._logger.Log($"Wildcard match result: {match}", TraceContext.TraceId, LogLevel.Info);
                return match;
            }

            this._logger.Log("No match found in MatchesSimplePattern.", TraceContext.TraceId, LogLevel.Debug);
            return false;
        }



    }
}
