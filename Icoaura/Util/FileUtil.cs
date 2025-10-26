
using System.IO;

namespace Icoaura.Util
{
    class FileUtil
    {

        public static string GetMimeTypeFromAbsolutePath(string absolutePath)
        {
            string extension = Path.GetExtension(absolutePath).ToLower();

            return extension switch
            {
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".svg" => "image/svg+xml",

                ".ico" => "image/ico",
                ".tiff" => "image/tiff",
                ".webp" => "image/webp",

                ".txt" => "text/plain",
                ".csv" => "text/csv",
                ".html" => "text/html",
                ".css" => "text/css",
                ".js" => "application/javascript",
                ".json" => "application/json",
                ".xml" => "application/xml",

                ".pdf" => "application/pdf",
                ".zip" => "application/zip",
                ".rar" => "application/x-rar-compressed",
                ".gz" => "application/gzip",

                ".mp3" => "audio/mpeg",
                ".wav" => "audio/wav",
                ".ogg" => "audio/ogg",

                ".mp4" => "video/mp4",
                ".mov" => "video/quicktime",
                ".avi" => "video/x-msvideo",
                ".mkv" => "video/x-matroska",

                ".exe" => "application/octet-stream",
                ".dll" => "application/octet-stream",
                ".lnk" => "application/x-ms-shortcut",
                ".url" => "application/internet-shortcut",

                _ => "application/octet-stream"
            };
        }

        public static string GetExtensionFromAbsolutePath(string absolutePath)
        {
            if (string.IsNullOrWhiteSpace(absolutePath))
                return ".bin";

            string ext = Path.GetExtension(absolutePath).ToLowerInvariant();

            return ext switch
            {
                ".jpeg" => ".jpg",
                ".htm" => ".html",
                ".tiff" => ".tif",
                ".svgz" => ".svg",
                ".jfif" => ".jpg",
                ".jpe" => ".jpg",
                ".mpga" => ".mp3",
                ".mpe" => ".mpg",
                ".mpeg" => ".mpg",
                ".mpv" => ".mp4",
                ".qt" => ".mov",
                ".ico" => ".ico",
                ".icns" => ".ico",
                ".icon" => ".ico",
                ".url" => ".url",

                // accepted normalized ones
                ".png" or ".jpg" or ".gif" or ".bmp" or ".svg" or ".webp"
                    or ".txt" or ".csv" or ".html" or ".css" or ".js"
                    or ".json" or ".xml" or ".pdf" or ".zip" or ".rar"
                    or ".gz" or ".mp3" or ".wav" or ".ogg" or ".mp4"
                    or ".mov" or ".avi" or ".mkv" or ".lnk"
                    => ext,

                _ => ".bin"
            };
        }

    }
}
