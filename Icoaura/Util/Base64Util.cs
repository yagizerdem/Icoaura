using System.IO;

namespace Icoaura.Util
{
    class Base64Util
    {
        public static string AddHeadersToBase64(string base64, string mimeType)
        {
            return $"data:{mimeType};base64,{base64}";
        }

        public static string RemoveHeadersFromBase64(string base64)
        {
            int commaIndex = base64.IndexOf(',');
            if (commaIndex >= 0)
            {
                return base64.Substring(commaIndex + 1);
            }
            return base64; // No header found, return original
        }

        public static string GetHeadersBase64(string base64)
        {
            int commaIndex = base64.IndexOf(',');
            if (commaIndex >= 0)
            {
                return base64.Substring(0, commaIndex);
            }
            return string.Empty; // No header found
        }

        public static string GetMimeTypeOfBase64(string base64)
        {
            // Try to extract MIME type from header if present
            string headers = GetHeadersBase64(base64);
            if (!string.IsNullOrWhiteSpace(headers))
            {
                int colonIndex = headers.IndexOf(':');
                int semicolonIndex = headers.IndexOf(';');
                if (colonIndex >= 0 && semicolonIndex > colonIndex)
                    return headers.Substring(colonIndex + 1, semicolonIndex - colonIndex - 1);
            }

            // Remove header if exists, to analyze raw bytes
            base64 = RemoveHeadersFromBase64(base64);

            if (string.IsNullOrWhiteSpace(base64))
                return "application/octet-stream";

            // Decode a small chunk of the base64 to inspect magic bytes
            byte[] buffer;
            try
            {
                buffer = Convert.FromBase64String(base64[..Math.Min(64, base64.Length)]);
            }
            catch
            {
                return "application/octet-stream";
            }

            // Check file signatures (magic numbers)
            if (buffer.Length > 4)
            {
                // PNG
                if (buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47)
                    return "image/png";

                // JPG
                if (buffer[0] == 0xFF && buffer[1] == 0xD8)
                    return "image/jpeg";

                // GIF
                if (buffer[0] == 0x47 && buffer[1] == 0x49 && buffer[2] == 0x46)
                    return "image/gif";

                // BMP
                if (buffer[0] == 0x42 && buffer[1] == 0x4D)
                    return "image/bmp";

                // PDF
                if (buffer[0] == 0x25 && buffer[1] == 0x50 && buffer[2] == 0x44 && buffer[3] == 0x46)
                    return "application/pdf";

                // ZIP (also DOCX, XLSX, PPTX, APK, etc.)
                if (buffer[0] == 0x50 && buffer[1] == 0x4B)
                    return "application/zip";

                // ICO
                if (buffer[0] == 0x00 && buffer[1] == 0x00 && buffer[2] == 0x01 && buffer[3] == 0x00)
                    return "image/ico";

                // MP4
                if (buffer[4] == 0x66 && buffer[5] == 0x74 && buffer[6] == 0x79 && buffer[7] == 0x70)
                    return "video/mp4";

                // WEBP
                if (buffer.Length >= 12 &&
                    buffer[0] == 0x52 && buffer[1] == 0x49 && buffer[2] == 0x46 && buffer[3] == 0x46 && // "RIFF"
                    buffer[8] == 0x57 && buffer[9] == 0x45 && buffer[10] == 0x42 && buffer[11] == 0x50)  // "WEBP"
                    return "image/webp";
            }

            // Default fallback
            return "application/octet-stream";
        }

        public static string GetExtensionOfBase64(string base64)
        {
            string mimeType = GetMimeTypeOfBase64(base64);

            // normalize mimeType -> extension
            return mimeType.ToLower() switch
            {
                "image/jpeg" => ".jpg",
                "image/jpg" => ".jpg",
                "image/png" => ".png",
                "image/gif" => ".gif",
                "image/bmp" => ".bmp",
                "image/svg+xml" => ".svg",
                "image/webp" => ".webp",
                "image/ico" => ".ico",

                "text/plain" => ".txt",
                "text/csv" => ".csv",
                "text/html" => ".html",
                "text/css" => ".css",
                "application/javascript" => ".js",
                "application/json" => ".json",
                "application/xml" => ".xml",

                "application/pdf" => ".pdf",
                "application/zip" => ".zip",
                "application/x-rar-compressed" => ".rar",
                "application/gzip" => ".gz",

                "audio/mpeg" => ".mp3",
                "audio/wav" => ".wav",
                "audio/ogg" => ".ogg",

                "video/mp4" => ".mp4",
                "video/quicktime" => ".mov",
                "video/x-msvideo" => ".avi",
                "video/x-matroska" => ".mkv",

                "application/x-ms-shortcut" => ".lnk",
                "application/internet-shortcut" => ".url",

                _ => ".bin"
            };
        }

    }
}
