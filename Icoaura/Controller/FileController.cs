
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
                    throw new AppException("File path cannot be empty.", true, LogLevel.Warning);

                if (string.IsNullOrWhiteSpace(base64))
                    throw new AppException("Base64 data cannot be empty.", true, LogLevel.Warning);

                // --- File existence logic ---
                if (System.IO.File.Exists(path))
                {
                    if (!overwrite)
                    {
                        throw new AppException(
                            userMessage: $"File already exists: {Path.GetFileName(path)}",
                            logMessage: $"Attempted to overwrite file '{path}' but overwrite=false",
                            isOperational: true,
                            logLevel: LogLevel.Warning
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
                    throw new AppException(
                        userMessage: "Invalid Base64 data format.",
                        logMessage: ex.Message,
                        isOperational: true,
                        logLevel: LogLevel.Error
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
            return ExecuteSafe<UrlMetaData>(() => {
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

    }
}
