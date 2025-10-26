
using Icoaura.Enum;
using Icoaura.Exception;
using Icoaura.Model;
using Icoaura.Util;
using IWshRuntimeLibrary;
using Model.DTO;
using System.IO;

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
                // --- Validation ---
                FileUtil.EnsureDirectoryExist(path, LogLevel.Error);

                if (allowedExtensions == null || allowedExtensions.Length == 0)
                {
                    return new();
                }

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
                        // collect files
                        foreach (var file in Directory.GetFiles(currentDir))
                        {
                            string ext = Path.GetExtension(file).ToLowerInvariant();
                            if (allowedExtensions.Any(e =>
                                string.Equals(e.TrimStart('.'), ext.TrimStart('.'), StringComparison.OrdinalIgnoreCase)))
                            {
                                result.Add(file);
                            }
                        }

                        // dive deeper
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


    }
}
