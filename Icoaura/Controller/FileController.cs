
using Icoaura.Enum;
using Icoaura.Exception;
using Icoaura.Model;
using Icoaura.Util;
using IWshRuntimeLibrary;
using Model.DTO;
using System.IO;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

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
                FileUtil.EnsureFileExist(path, Enum.LogLevel.Error);
                string serialized = System.IO.File.ReadAllText(path);
                return serialized;
            });
        }

        public ApiResponse<string> GetBase64(string path)
        {
            return ExecuteSafe(() =>
            {
                FileUtil.EnsureFileExist(path, Enum.LogLevel.Error);
                byte[] buffer = System.IO.File.ReadAllBytes(path);
                string base64 = Convert.ToBase64String(buffer);
                string mimeType = FileUtil.GetMimeTypeFromAbsolutePath(path);
                string base64WitHeaders = Base64Util.AddHeadersToBase64(base64, mimeType);
                return base64WitHeaders;
            });
        }

        public ApiResponse<object> DeleteFile(string path)
        {
            return ExecuteSafe(() =>
            {
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                return new object();
            });
        }

        public ApiResponse<object> WriteBase64(string path, string base64, bool overwrite = true)
        {
            return ExecuteSafe(() =>
            {
                // --- Validation ---
                if (string.IsNullOrWhiteSpace(path))
                    throw AppException.Operational(
                        userMessage: "File path cannot be empty.",
                        logMessage: "Attempted to write Base64 data to an empty file path.",
                        level: LogLevel.Error
                    );

                if (string.IsNullOrWhiteSpace(base64))
                    throw AppException.Operational(
                        userMessage: "Base64 data cannot be empty.",
                        logMessage: "Attempted to write empty Base64 data to file.",
                        level: LogLevel.Error
                    );


                // --- File existence logic ---
                if (System.IO.File.Exists(path))
                {
                    if (!overwrite)
                    {
                        throw AppException.Operational(
                            userMessage: $"File already exists: {Path.GetFileName(path)}",
                            logMessage: $"Attempted to overwrite file '{path}' but overwrite=false",
                            level: LogLevel.Error
                        );

                    }

                    // Safe delete before write
                    System.IO.File.Delete(path);
                }

                // --- Base64 conversion ---
                string normalized = Base64Util.RemoveHeadersFromBase64(base64);

                byte[] buffer;
                try
                {
                    buffer = Convert.FromBase64String(normalized);
                }
                catch (FormatException ex)
                {
                    throw AppException.Operational(
                        userMessage: "The provided Base64 data is not in a valid format.",
                        logMessage: ex.Message,
                        level: LogLevel.Error
                    );

                }

                System.IO.File.WriteAllBytes(path, buffer);

                return new object();
            });
        }

        public ApiResponse<LnkMetaData> GetLnkMetaData(string path)
        {
            return ExecuteSafe<LnkMetaData>(() =>
            {
                FileUtil.EnsureFileExist(path, Enum.LogLevel.Error);
                FileUtil.EnsureFileHasExtension(path, [".lnk", "lnk"], Enum.LogLevel.Error);

                var shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(path);


                // --- File info for timestamps ---
                var fi = new FileInfo(path);

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

                return meta;

            });
        }


        public ApiResponse<UrlMetaData> GetUrlMetaData(string path)
        {
            return ExecuteSafe<UrlMetaData>(() =>
            {
                FileUtil.EnsureFileExist(path, Enum.LogLevel.Error);
                FileUtil.EnsureFileHasExtension(path, [".url", "url"], Enum.LogLevel.Error);

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

                return metaData;

            });
        }

        public ApiResponse<DirMetaData> GetDirMetaData(string path)
        {
            return ExecuteSafe(() =>
            {
                FileUtil.EnsureDirectoryExist(path);

                var meta = new DirMetaData
                {
                    DirectoryPath = path,
                    IconPath = string.Empty
                };

                string desktopIniPath = Path.Combine(path, "desktop.ini");

                if (System.IO.File.Exists(desktopIniPath))
                {
                    string? iconResourceLine = System.IO.File.ReadAllLines(desktopIniPath)
                        .FirstOrDefault(line => line.StartsWith("IconResource=", StringComparison.OrdinalIgnoreCase));

                    if (!string.IsNullOrWhiteSpace(iconResourceLine))
                    {
                        string iconPath = iconResourceLine
                            .Substring("IconResource=".Length)
                            .Split(',')[0]
                            .Trim();

                        if (System.IO.File.Exists(iconPath))
                            meta.IconPath = iconPath;

                    }
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
                FileUtil.EnsureDirectoryExist(path, LogLevel.Error);

                if (allowedExtensions == null || allowedExtensions.Length == 0)
                    return new();

                if (depth < 0)
                {
                    throw AppException.Operational(
                        userMessage: "Depth cannot be negative.",
                        logMessage: $"Invalid depth value: {depth}",
                        level: LogLevel.Error
                    );
                }

                var result = new List<string>();
                var stack = new Stack<(string dir, int level)>();
                stack.Push((path, 0));

                bool matchAll = allowedExtensions.Any(e => e == "*" || e == ".*");

                while (stack.Count > 0)
                {
                    var (currentDir, currentLevel) = stack.Pop();

                    if (currentLevel > depth)
                        continue;

                    try
                    {
                        foreach (var file in Directory.GetFiles(currentDir))
                        {
                            string ext = Path.GetExtension(file).ToLowerInvariant();

                            // '*' matches all files
                            if (matchAll || allowedExtensions.Any(e =>
                                string.Equals(e.TrimStart('.'), ext.TrimStart('.'), StringComparison.OrdinalIgnoreCase)))
                            {
                                result.Add(file);
                            }
                        }

                        if (currentLevel < depth)
                        {
                            foreach (var dir in Directory.GetDirectories(currentDir))
                            {
                                stack.Push((dir, currentLevel + 1));
                            }
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        continue;
                    }
                    catch (IOException ex)
                    {
                        throw AppException.Operational(
                            userMessage: "An I/O error occurred while scanning directories.",
                            logMessage: ex.Message,
                            level: LogLevel.Error
                        );
                    }
                }

                return result;
            });
        }

        public ApiResponse<List<string>> GetFoldersUnderPath(string path, int depth)
        {
            return ExecuteSafe(() =>
            {
                // --- Validation ---
                FileUtil.EnsureDirectoryExist(path, LogLevel.Error);

                if (depth < 0)
                {

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
                        // collect subdirectories
                        foreach (var dir in Directory.GetDirectories(currentDir))
                        {
                            result.Add(dir);

                            // recursive dive if allowed
                            if (currentLevel < depth)
                                stack.Push((dir, currentLevel + 1));
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // skip restricted folders silently
                        continue;
                    }
                    catch (IOException ex)
                    {
                        throw AppException.Operational(
                            userMessage: "An I/O error occurred while scanning directories.",
                            logMessage: ex.Message,
                            level: LogLevel.Error
                        );

                    }
                }

                return result;
            });
        }

        public ApiResponse<List<string>> GetMatchingFileSystemEntries(string rootPath, string pattern)
        {
            return ExecuteSafe(() =>
            {
                if (!Directory.Exists(rootPath))
                    throw AppException.Operational(
                        userMessage: "Root path does not exist.",
                        logMessage: $"GetMatchingFileSystemEntries failed: root path not found ({rootPath})",
                        level: LogLevel.Error
                    );

                var results = new List<string>();
                pattern = pattern.Replace('/', '\\');
                var searchOption = pattern.Contains("**") ? SearchOption.AllDirectories : SearchOption.AllDirectories;

                var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);
                matcher.AddInclude(pattern);

                var directoryInfo = new DirectoryInfoWrapper(new DirectoryInfo(rootPath));
                var matchResult = matcher.Execute(directoryInfo);

                foreach (var match in matchResult.Files)
                {
                    string fullPath = Path.GetFullPath(Path.Combine(rootPath, match.Path.Replace('/', '\\')));
                    results.Add(fullPath);
                }

                bool isSingleSegment = !pattern.Contains("\\") && !pattern.Contains("/");

                if (isSingleSegment)
                {
                    foreach (var dir in Directory.EnumerateDirectories(rootPath, "*", SearchOption.TopDirectoryOnly))
                    {
                        string dirName = Path.GetFileName(dir);
                        if (MatchesSimplePattern(dirName, pattern))
                            results.Add(Path.GetFullPath(dir));
                    }
                }
                else
                {
                    foreach (var dir in Directory.EnumerateDirectories(rootPath, "*", SearchOption.AllDirectories))
                    {
                        string relativePath = Path.GetRelativePath(rootPath, dir);
                        if (MatchesGlobPattern(relativePath, pattern))
                            results.Add(Path.GetFullPath(dir));
                    }
                }

                return results
                    .Select(p => Path.GetFullPath(p.Replace('/', '\\')))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
            });
        }

        public bool MatchesGlobPattern(string path, string pattern)
        {
            path = path.Replace('/', '\\').Trim('\\');
            pattern = pattern.Replace('/', '\\').Trim('\\');

            if (path.Equals(pattern, StringComparison.OrdinalIgnoreCase))
                return true;

            string[] pathParts = path.Split('\\', StringSplitOptions.RemoveEmptyEntries);
            string[] patternParts = pattern.Split('\\', StringSplitOptions.RemoveEmptyEntries);

            if (!pattern.Contains("*") && pathParts.Length != patternParts.Length)
                return false;

            return MatchesPatternParts(pathParts, patternParts, 0, 0);
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
            if (name.Equals(pattern, StringComparison.OrdinalIgnoreCase))
                return true;

            if (pattern.Contains("*") || pattern.Contains("?"))
            {
                return System.IO.Enumeration.FileSystemName.MatchesSimpleExpression(
                    pattern,
                    name,
                    ignoreCase: true
                );
            }

            return false;
        }



    }
}
