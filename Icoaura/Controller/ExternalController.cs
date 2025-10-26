using Icoaura.Context;
using Icoaura.Enum;
using Icoaura.Exception;
using Icoaura.Model;
using Icoaura.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Icoaura.Controller
{
    public class ExternalController : BaseController
    {
        private readonly FileController _fileController;

        public ExternalController(FileController fileController)
        {
            _fileController = fileController;
        }

        public ApiResponse<string> GetExeIconAsPngBase64(string exePath)
        {
            return ExecuteSafe(() =>
            {
                string tempFilePath = string.Empty;
                try
                {
                    FileUtil.EnsureFileExist(exePath, LogLevel.Error);
                    FileUtil.EnsureFileHasExtension(exePath, [".exe", "exe"]);

                    string extractIconExecutablePath = Path.Combine(
                        AppContext.BaseDirectory, "tools", "extracticon.exe");

                    tempFilePath = Path.Combine(
                        ApplicationPathContext.AppTempFolderPath,
                        Guid.NewGuid().ToString());

                    using (Process process = new Process())
                    {
                        process.StartInfo.FileName = extractIconExecutablePath;
                        process.StartInfo.Arguments = $"\"{exePath}\" \"{tempFilePath}\"";
                        process.StartInfo.CreateNoWindow = true;
                        process.StartInfo.UseShellExecute = false;
                        process.Start();
                        process.WaitForExit();
                        
                        this.EnsureProcessSafelyTerminated(
                         exitCode: process.ExitCode,
                         userMessage: "Failed to extract icon from executable. Please make sure the file is a valid Windows application.",
                         logMessage: $"extracticon.exe failed to extract icon from '{exePath}' (ExitCode: {process.ExitCode})",
                         logLevel: LogLevel.Error,
                         isOperational: true
                     );
                    }

                    FileUtil.EnsureFileExist(tempFilePath, LogLevel.Error);
                    string base64 = _fileController.GetBase64(tempFilePath).Data;
                    _fileController.DeleteFile(tempFilePath);

                    return base64;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(tempFilePath))
                        _fileController.DeleteFile(tempFilePath);
                }

            });

        }

        public ApiResponse<string> IcoToPngBase64(string icoPath, int width = 256, int height = 256)
        {
            return ExecuteSafe(() =>
            {
                string tempFilePath = string.Empty;

                try
                {
                    // --- Validation ---
                    FileUtil.EnsureFileExist(icoPath, LogLevel.Error);
                    FileUtil.EnsureFileHasExtension(icoPath, new[] { ".ico", ".cur" });

                    string magickExePath = Path.Combine(
                        AppContext.BaseDirectory,
                        "tools",
                        "ImageMagick",
                        "magick.exe");

                    FileUtil.EnsureFileExist(magickExePath, LogLevel.Error);

                    tempFilePath = Path.Combine(
                        ApplicationPathContext.AppTempFolderPath,
                        $"{Guid.NewGuid():N}.png");

                    // --- Execute ImageMagick process ---
                    using (var process = new Process())
                    {
                        process.StartInfo.FileName = magickExePath;
                        process.StartInfo.Arguments =
                            $"\"{icoPath}\" -thumbnail {width}x{height} -alpha on -background none -flatten \"{tempFilePath}\"";
                        process.StartInfo.CreateNoWindow = true;
                        process.StartInfo.UseShellExecute = false;
                        process.Start();
                        process.WaitForExit();

                        this.EnsureProcessSafelyTerminated(
                                   exitCode: process.ExitCode,
                                   userMessage: "ImageMagick failed to process the image.",
                                   logMessage: "Magick.exe returned non-zero exit code during ICO->PNG conversion.",
                                   logLevel: LogLevel.Warning
                               );
                    }

                    // --- Verify output file ---
                    FileUtil.EnsureFileExist(tempFilePath, LogLevel.Error);

                    // --- Convert to Base64 ---
                    byte[] bytes = File.ReadAllBytes(tempFilePath);
                    string base64 = Convert.ToBase64String(bytes);
                    string mimeType = FileUtil.GetMimeTypeFromAbsolutePath(tempFilePath);
                    return Base64Util.AddHeadersToBase64(base64, mimeType);
                }
                finally
                {
                    // Cleanup temporary file
                    try
                    {
                        if (!string.IsNullOrEmpty(tempFilePath) && File.Exists(tempFilePath))
                            File.Delete(tempFilePath);
                    }
                    catch
                    {
                        // ignore cleanup errors silently
                    }
                }
            });
        }

        public ApiResponse<string> PngToIcoBase64(string pngPath, int size = 256)
        {
            return ExecuteSafe(() =>
            {
                string tempFilePath = string.Empty;

                try
                {
                    // --- Validation ---
                    FileUtil.EnsureFileExist(pngPath, LogLevel.Error);
                    FileUtil.EnsureFileHasExtension(pngPath, new[] { ".png" });

                    string magickExePath = Path.Combine(
                        AppContext.BaseDirectory,
                        "tools",
                        "ImageMagick",
                        "magick.exe");

                    FileUtil.EnsureFileExist(magickExePath, LogLevel.Error);

                    tempFilePath = Path.Combine(
                        ApplicationPathContext.AppTempFolderPath,
                        $"{Guid.NewGuid():N}.ico");

                    // --- Execute ImageMagick process ---
                    using (var process = new Process())
                    {
                        process.StartInfo.FileName = magickExePath;
                        process.StartInfo.Arguments =
                            $"\"{pngPath}\" -resize {size}x{size} -alpha on -background none -flatten \"{tempFilePath}\"";
                        process.StartInfo.CreateNoWindow = true;
                        process.StartInfo.UseShellExecute = false;
                        process.Start();
                        process.WaitForExit();

                        this.EnsureProcessSafelyTerminated(
                            exitCode: process.ExitCode,
                            userMessage: "ImageMagick failed to process the image.",
                            logMessage: "Magick.exe returned non-zero exit code during PNG->ICO conversion.",
                            logLevel: LogLevel.Warning
                        );
                    }

                    // --- Verify output file ---
                    FileUtil.EnsureFileExist(tempFilePath, LogLevel.Warning);

                    // --- Convert to Base64 ---
                    byte[] bytes = File.ReadAllBytes(tempFilePath);
                    string base64 = Convert.ToBase64String(bytes);
                    string mimeType = FileUtil.GetMimeTypeFromAbsolutePath(tempFilePath);
                    return Base64Util.AddHeadersToBase64(base64, mimeType);
                }
                finally
                {
                    // Cleanup temporary file
                    try
                    {
                        if (!string.IsNullOrEmpty(tempFilePath) && File.Exists(tempFilePath))
                            File.Delete(tempFilePath);
                    }
                    catch
                    {
                        // ignore cleanup errors silently
                    }
                }
            });
        }

        public ApiResponse<string> ResizePng(string pngPath, int width, int height)
        {
            return ExecuteSafe(() =>
            {
                string tempFilePath = string.Empty;

                try
                {
                    // --- Validation ---
                    FileUtil.EnsureFileExist(pngPath, LogLevel.Error);
                    FileUtil.EnsureFileHasExtension(pngPath, new[] { ".png" });

                    string magickExePath = Path.Combine(
                        AppContext.BaseDirectory,
                        "tools",
                        "ImageMagick",
                        "magick.exe");

                    FileUtil.EnsureFileExist(magickExePath, LogLevel.Error);

                    tempFilePath = Path.Combine(
                        ApplicationPathContext.AppTempFolderPath,
                        $"{Guid.NewGuid():N}.png");

                    // --- Build process info ---
                    string arguments = $"\"{pngPath}\" -resize {width}x{height}! \"{tempFilePath}\"";

                    var startInfo = new ProcessStartInfo
                    {
                        FileName = magickExePath,
                        Arguments = arguments,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true
                    };

                    // --- Execute process ---
                    using (var process = new Process { StartInfo = startInfo })
                    {
                        process.Start();
                        string errorOutput = process.StandardError.ReadToEnd();
                        process.WaitForExit();

                        this.EnsureProcessSafelyTerminated(process.ExitCode,
                            "Failed to resize PNG image. Please check the file format or dimensions.",
                            $"ImageMagick process failed with exit code {process.ExitCode}: {errorOutput}",
                             LogLevel.Warning,
                             true);
                    }

                    // --- Verify output file ---
                    FileUtil.EnsureFileExist(tempFilePath, LogLevel.Warning);

                    // --- Convert resized PNG to Base64 ---
                    byte[] bytes = File.ReadAllBytes(tempFilePath);
                    string base64 = Convert.ToBase64String(bytes);
                    string mimeType = FileUtil.GetMimeTypeFromAbsolutePath(tempFilePath);
                    return Base64Util.AddHeadersToBase64(base64, mimeType);
                }
                finally
                {
                    // --- Cleanup temp file ---
                    try
                    {
                        if (!string.IsNullOrEmpty(tempFilePath) && File.Exists(tempFilePath))
                            File.Delete(tempFilePath);
                    }
                    catch
                    {
                        // ignore cleanup errors silently
                    }
                }
            });
        }

        public ApiResponse<(int Width, int Height)> GetPngDimensions(string pngPath)
        {
            return ExecuteSafe(() =>
            {
                string magickExePath = Path.Combine(
                    AppContext.BaseDirectory,
                    "tools",
                    "ImageMagick",
                    "magick.exe");

                // --- Validation ---
                FileUtil.EnsureFileExist(pngPath, LogLevel.Error);
                FileUtil.EnsureFileHasExtension(pngPath, new[] { ".png" });
                FileUtil.EnsureFileExist(magickExePath, LogLevel.Error);

                string output;

                using (var process = new Process())
                {
                    process.StartInfo.FileName = magickExePath;
                    process.StartInfo.Arguments = $"identify -ping -format \"%w %h\" \"{pngPath}\"";
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;

                    process.Start();

                    output = process.StandardOutput.ReadToEnd();
                    string errorOutput = process.StandardError.ReadToEnd();

                    process.WaitForExit();

                    this.EnsureProcessSafelyTerminated(
                        exitCode: process.ExitCode,
                        userMessage: "Failed to read PNG image dimensions. Please check the file format.",
                        logMessage: $"ImageMagick 'identify' command failed for '{pngPath}' (ExitCode: {process.ExitCode}, Error: {errorOutput})",
                        logLevel: LogLevel.Error
                    );
                }

                // --- Parse output safely ---
                var parts = output.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 2 ||
                    !int.TryParse(parts[0], out int width) ||
                    !int.TryParse(parts[1], out int height))
                {

                    throw AppException.Operational(
                        userMessage: "Failed to parse image dimensions from output.",
                        logMessage: $"Invalid ImageMagick output for '{pngPath}': '{output}'",
                        level: LogLevel.Error
                    );
                }

                return (width, height);
            });
        }


        public ApiResponse<string> ApplyOpacity(string pngPath, float opacityAmount)
        {
            return ExecuteSafe(() =>
            {
                string tempFilePath = string.Empty;

                try
                {
                    // --- Validation ---
                    FileUtil.EnsureFileExist(pngPath, LogLevel.Error);
                    FileUtil.EnsureFileHasExtension(pngPath, new[] { ".png" });

                    opacityAmount = Math.Clamp(opacityAmount, 0f, 1f);

                    string magickExePath = Path.Combine(
                        AppContext.BaseDirectory,
                        "tools",
                        "ImageMagick",
                        "magick.exe");

                    FileUtil.EnsureFileExist(magickExePath, LogLevel.Error);

                    tempFilePath = Path.Combine(
                        ApplicationPathContext.AppTempFolderPath,
                        $"{Guid.NewGuid():N}.png");

                    string opacityInvariant = opacityAmount.ToString(System.Globalization.CultureInfo.InvariantCulture);

                    // --- Build process ---
                    using (var process = new Process())
                    {
                        process.StartInfo.FileName = magickExePath;
                        process.StartInfo.Arguments =
                            $"\"{pngPath}\" -alpha set -channel A -evaluate Multiply {opacityInvariant} +channel \"{tempFilePath}\"";
                        process.StartInfo.CreateNoWindow = true;
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.RedirectStandardError = true;

                        process.Start();

                        string errorOutput = process.StandardError.ReadToEnd();
                        process.WaitForExit();

                        this.EnsureProcessSafelyTerminated(
                            exitCode: process.ExitCode,
                            userMessage: "Failed to apply opacity to PNG image.",
                            logMessage: $"ImageMagick failed while adjusting opacity on '{pngPath}'. Error: {errorOutput}",
                            logLevel: LogLevel.Error
                        );
                    }

                    // --- Verify output and convert to base64 ---
                    FileUtil.EnsureFileExist(tempFilePath, LogLevel.Warning);
                    string base64 = _fileController.GetBase64(tempFilePath).Data;
                    return base64;
                }
                finally
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(tempFilePath))
                            _fileController.DeleteFile(tempFilePath);
                    }
                    catch { /* ignore cleanup errors */ }
                }
            });
        }


        public ApiResponse<string> ApplyCornerRadius(string pngPath, float radiusAmount)
        {
            return ExecuteSafe(() =>
            {
                string tempFilePath = string.Empty;

                try
                {
                    // --- Validation ---
                    FileUtil.EnsureFileExist(pngPath, LogLevel.Error);
                    FileUtil.EnsureFileHasExtension(pngPath, new[] { ".png" });
                    radiusAmount = Math.Clamp(radiusAmount, 0f, 0.5f);

                    string magickExePath = Path.Combine(
                        AppContext.BaseDirectory,
                        "tools",
                        "ImageMagick",
                        "magick.exe");

                    FileUtil.EnsureFileExist(magickExePath, LogLevel.Error);

                    tempFilePath = Path.Combine(
                        ApplicationPathContext.AppTempFolderPath,
                        $"{Guid.NewGuid():N}.png");

                    // --- Get PNG dimensions ---
                    var dimResponse = this.GetPngDimensions(pngPath);
                    EnsureSuccess<(int w, int h)>(dimResponse);

                    (int w, int h) = dimResponse.Data;
                    int radiusInPixels = (int)(Math.Min(w, h) * radiusAmount);

                    // --- Build ImageMagick arguments ---
                    string args =
                        $"\"{pngPath}\" " +
                        "( +clone -alpha extract " +
                        $"-draw \"fill black polygon 0,0 0,{radiusInPixels} {radiusInPixels},0 fill white circle {radiusInPixels},{radiusInPixels} {radiusInPixels},0\" " +
                        "( +clone -flip ) -compose Multiply -composite " +
                        "( +clone -flop ) -compose Multiply -composite ) " +
                        "-alpha off -compose CopyOpacity -composite " +
                        $"\"{tempFilePath}\"";

                    // --- Execute ImageMagick ---
                    using (var process = new Process())
                    {
                        process.StartInfo.FileName = magickExePath;
                        process.StartInfo.Arguments = args;
                        process.StartInfo.CreateNoWindow = true;
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.RedirectStandardError = true;

                        process.Start();

                        string errorOutput = process.StandardError.ReadToEnd();
                        process.WaitForExit();

                        this.EnsureProcessSafelyTerminated(
                            exitCode: process.ExitCode,
                            userMessage: "Failed to apply corner radius to PNG image.",
                            logMessage: $"ImageMagick failed while rounding corners for '{pngPath}'. Error: {errorOutput}",
                            logLevel: LogLevel.Error
                        );
                    }

                    // --- Verify output and convert to base64 ---
                    FileUtil.EnsureFileExist(tempFilePath, LogLevel.Warning);
                    string base64 = _fileController.GetBase64(tempFilePath).Data;
                    return base64;
                }
                finally
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(tempFilePath))
                            _fileController.DeleteFile(tempFilePath);
                    }
                    catch { /* ignore cleanup errors */ }
                }
            });
        }


        public ApiResponse<string> IcoBase64ToPngBase64(string icoBase64)
        {
            return ExecuteSafe(() =>
            {
                string tempIcoPath = string.Empty;

                try
                {
                    // --- Validation ---
                    if (string.IsNullOrWhiteSpace(icoBase64))
                        throw AppException.Operational(
                            userMessage: "Base64 data cannot be empty.",
                            logMessage: "IcoBase64ToPngBase64 called with empty base64 string.",
                            level: LogLevel.Error
                        );

                    tempIcoPath = Path.Combine(
                        ApplicationPathContext.AppTempFolderPath,
                        $"{Guid.NewGuid():N}.ico");

                    // --- Write temporary ICO ---
                    var writeResponse = _fileController.WriteBase64(tempIcoPath, icoBase64);
                    EnsureSuccess<object>(writeResponse);

                    // --- Convert to PNG base64 ---
                    var pngResponse = this.IcoToPngBase64(tempIcoPath);
                    EnsureSuccess<string>(pngResponse);

                    return pngResponse.Data!;
                }
                finally
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(tempIcoPath))
                            _fileController.DeleteFile(tempIcoPath);
                    }
                    catch { /* ignore cleanup errors */ }
                }
            });
        }

        public ApiResponse<string> PngBase64ToIcoBase64(string pngBase64)
        {
            return ExecuteSafe(() =>
            {
                string tempPngPath = string.Empty;

                try
                {
                    if (string.IsNullOrWhiteSpace(pngBase64))
                        throw AppException.Operational(
                            userMessage: "Base64 data cannot be empty.",
                            logMessage: "PngBase64ToIcoBase64 called with empty base64 string.",
                            level: LogLevel.Error
                        );


                    tempPngPath = Path.Combine(
                        ApplicationPathContext.AppTempFolderPath,
                        $"{Guid.NewGuid():N}.png");

                    var writeResponse = _fileController.WriteBase64(tempPngPath, pngBase64);
                    EnsureSuccess<object>(writeResponse);

                    var icoResponse = this.PngToIcoBase64(tempPngPath);
                    EnsureSuccess<string>(icoResponse);

                    return icoResponse.Data!;
                }
                finally
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(tempPngPath))
                            _fileController.DeleteFile(tempPngPath);
                    }
                    catch { }
                }
            });
        }


        public ApiResponse<string> ResizePngBase64(string pngBase64, int width, int height)
        {
            return ExecuteSafe(() =>
            {
                string tempPngPath = string.Empty;

                try
                {
                    if (string.IsNullOrWhiteSpace(pngBase64))
                        throw AppException.Operational(
                            userMessage: "Base64 data cannot be empty.",
                            logMessage: "ResizePngBase64 called with empty base64 string.",
                            level: LogLevel.Error
                        );

                    tempPngPath = Path.Combine(
                        ApplicationPathContext.AppTempFolderPath,
                        $"{Guid.NewGuid():N}.png");

                    var writeResponse = _fileController.WriteBase64(tempPngPath, pngBase64);
                    EnsureSuccess<object>(writeResponse);

                    var resizeResponse = this.ResizePng(tempPngPath, width, height);
                    EnsureSuccess<string>(resizeResponse);

                    return resizeResponse.Data!;
                }
                finally
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(tempPngPath))
                            _fileController.DeleteFile(tempPngPath);
                    }
                    catch { }
                }
            });
        }


        public ApiResponse<string> ApplyOpacityOnPngBase64(string pngBase64, float opacityAmount)
        {
            return ExecuteSafe(() =>
            {
                string tempPngPath = string.Empty;

                try
                {
                    if (string.IsNullOrWhiteSpace(pngBase64))
                        throw AppException.Operational(
                            userMessage: "Base64 data cannot be empty.",
                            logMessage: "ApplyOpacityOnPngBase64 called with empty base64 string.",
                            level: LogLevel.Error
                        );

                    tempPngPath = Path.Combine(
                        ApplicationPathContext.AppTempFolderPath,
                        $"{Guid.NewGuid():N}.png");

                    var writeResponse = _fileController.WriteBase64(tempPngPath, pngBase64);
                    EnsureSuccess<object>(writeResponse);

                    var opacityResponse = this.ApplyOpacity(tempPngPath, opacityAmount);
                    EnsureSuccess<string>(opacityResponse);

                    return opacityResponse.Data!;
                }
                finally
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(tempPngPath))
                            _fileController.DeleteFile(tempPngPath);
                    }
                    catch { }
                }
            });
        }


        public ApiResponse<string> ApplyCornerRadiusOnPngBase64(string pngBase64, float cornerRadius)
        {
            return ExecuteSafe(() =>
            {
                string tempPngPath = string.Empty;

                try
                {
                    if (string.IsNullOrWhiteSpace(pngBase64))
                        throw AppException.Operational(
                            userMessage: "Base64 data cannot be empty.",
                            logMessage: "ApplyCornerRadiusOnPngBase64 called with empty base64 string.",
                            level: LogLevel.Error
                        );

                    tempPngPath = Path.Combine(
                        ApplicationPathContext.AppTempFolderPath,
                        $"{Guid.NewGuid():N}.png");

                    var writeResponse = _fileController.WriteBase64(tempPngPath, pngBase64);
                    EnsureSuccess<object>(writeResponse);

                    var radiusResponse = this.ApplyCornerRadius(tempPngPath, cornerRadius);
                    EnsureSuccess<string>(radiusResponse);

                    return radiusResponse.Data!;
                }
                finally
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(tempPngPath))
                            _fileController.DeleteFile(tempPngPath);
                    }
                    catch { }
                }
            });
        }

        public void EnsureProcessSafelyTerminated(int exitCode, 
            string userMessage = "The external process terminated unexpectedly. Please try again or check file permissions.",
             string logMessage = "External process exited abnormally."
            , LogLevel logLevel = LogLevel.Error,
            bool isOperational = true)
        {
            if (exitCode != 0)
            {
                throw AppException.Operational(
                    userMessage: userMessage,
                    logMessage: logMessage,
                    level: logLevel
                );
            }
        }


    }
}
