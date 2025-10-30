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
                this._logger.Log("Entering GetExeIconAsPngBase64()", TraceContext.TraceId, LogLevel.Trace);

                string tempFilePath = string.Empty;
                try
                {
                    this._logger.Log($"Validating exePath: {exePath}", TraceContext.TraceId, LogLevel.Debug);

                    FileUtil.EnsureFileExist(exePath, LogLevel.Error);
                    FileUtil.EnsureFileHasExtension(exePath, [".exe", "exe"]);

                    string extractIconExecutablePath = Path.Combine(
                        AppContext.BaseDirectory, "tools", "extracticon.exe");
                    this._logger.Log($"Resolved extracticon.exe path: {extractIconExecutablePath}", TraceContext.TraceId, LogLevel.Debug);

                    tempFilePath = Path.Combine(
                        ApplicationPathContext.AppTempFolderPath,
                        Guid.NewGuid().ToString());
                    this._logger.Log($"Temporary icon output path: {tempFilePath}", TraceContext.TraceId, LogLevel.Debug);

                    using (Process process = new Process())
                    {
                        this._logger.Log($"Starting extracticon.exe for '{exePath}'", TraceContext.TraceId, LogLevel.Trace);

                        process.StartInfo.FileName = extractIconExecutablePath;
                        process.StartInfo.Arguments = $"\"{exePath}\" \"{tempFilePath}\"";
                        process.StartInfo.CreateNoWindow = true;
                        process.StartInfo.UseShellExecute = false;
                        process.Start();
                        process.WaitForExit();

                        this._logger.Log($"extracticon.exe exited with code {process.ExitCode}", TraceContext.TraceId, LogLevel.Debug);

                        this.EnsureProcessSafelyTerminated(
                            exitCode: process.ExitCode,
                            userMessage: this._l10nService.GetLocalizedMessage("Errors.IconExtractionFailed"),
                            logMessage: $"extracticon.exe failed to extract icon from '{exePath}' (ExitCode: {process.ExitCode})",
                            logLevel: LogLevel.Error,
                            isOperational: true
                        );
                    }

                    FileUtil.EnsureFileExist(tempFilePath, LogLevel.Error);
                    this._logger.Log($"Icon extracted successfully to temp file: {tempFilePath}", TraceContext.TraceId, LogLevel.Info);

                    string base64 = _fileController.GetBase64(tempFilePath).Data;
                    this._logger.Log($"Converted extracted icon to Base64 (length: {base64.Length})", TraceContext.TraceId, LogLevel.Debug);

                    _fileController.DeleteFile(tempFilePath);
                    this._logger.Log("Temporary icon file deleted", TraceContext.TraceId, LogLevel.Trace);

                    return base64;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(tempFilePath))
                    {
                        try
                        {
                            _fileController.DeleteFile(tempFilePath);
                            this._logger.Log($"Cleaned up temp file: {tempFilePath}", TraceContext.TraceId, LogLevel.Trace);
                        }
                        catch
                        {
                            this._logger.Log($"Failed to delete temp file: {tempFilePath}", TraceContext.TraceId, LogLevel.Warning);
                        }
                    }
                }
            });
        }


        public ApiResponse<string> IcoToPngBase64(string icoPath, int width = 256, int height = 256)
        {
            return ExecuteSafe(() =>
            {
                this._logger.Log("Entering IcoToPngBase64()", TraceContext.TraceId, LogLevel.Trace);

                string tempFilePath = string.Empty;

                try
                {
                    this._logger.Log($"Validating ICO path: {icoPath}", TraceContext.TraceId, LogLevel.Debug);

                    // --- Validation ---
                    FileUtil.EnsureFileExist(icoPath, LogLevel.Error);
                    FileUtil.EnsureFileHasExtension(icoPath, new[] { ".ico", ".cur" });

                    string magickExePath = Path.Combine(
                        AppContext.BaseDirectory,
                        "tools",
                        "ImageMagick",
                        "magick.exe");

                    FileUtil.EnsureFileExist(magickExePath, LogLevel.Error);
                    this._logger.Log($"Resolved ImageMagick path: {magickExePath}", TraceContext.TraceId, LogLevel.Debug);

                    tempFilePath = Path.Combine(
                        ApplicationPathContext.AppTempFolderPath,
                        $"{Guid.NewGuid():N}.png");
                    this._logger.Log($"Temporary PNG output path: {tempFilePath}", TraceContext.TraceId, LogLevel.Debug);

                    // --- Execute ImageMagick process ---
                    using (var process = new Process())
                    {
                        process.StartInfo.FileName = magickExePath;
                        process.StartInfo.Arguments =
                            $"\"{icoPath}\" -thumbnail {width}x{height} -alpha on -background none -flatten \"{tempFilePath}\"";
                        process.StartInfo.CreateNoWindow = true;
                        process.StartInfo.UseShellExecute = false;

                        this._logger.Log($"Executing magick.exe: {process.StartInfo.Arguments}", TraceContext.TraceId, LogLevel.Trace);

                        process.Start();
                        process.WaitForExit();

                        this._logger.Log($"magick.exe exited with code {process.ExitCode}", TraceContext.TraceId, LogLevel.Debug);

                        this.EnsureProcessSafelyTerminated(
                            exitCode: process.ExitCode,
                            userMessage: this._l10nService.GetLocalizedMessage("Errors.ImageMagickProcessFailed"),
                            logMessage: "Magick.exe returned non-zero exit code during ICO->PNG conversion.",
                            logLevel: LogLevel.Warning
                        );
                    }

                    // --- Verify output file ---
                    FileUtil.EnsureFileExist(tempFilePath, LogLevel.Error);
                    this._logger.Log("ImageMagick successfully generated PNG file", TraceContext.TraceId, LogLevel.Info);

                    // --- Convert to Base64 ---
                    byte[] bytes = File.ReadAllBytes(tempFilePath);
                    string base64 = Convert.ToBase64String(bytes);
                    string mimeType = FileUtil.GetMimeTypeFromAbsolutePath(tempFilePath);
                    this._logger.Log($"Converted PNG to Base64 (length: {base64.Length})", TraceContext.TraceId, LogLevel.Debug);

                    return Base64Util.AddHeadersToBase64(base64, mimeType);
                }
                finally
                {
                    // Cleanup temporary file
                    try
                    {
                        if (!string.IsNullOrEmpty(tempFilePath) && File.Exists(tempFilePath))
                        {
                            File.Delete(tempFilePath);
                            this._logger.Log($"Deleted temporary PNG file: {tempFilePath}", TraceContext.TraceId, LogLevel.Trace);
                        }
                    }
                    catch
                    {
                        this._logger.Log($"Failed to delete temporary PNG file: {tempFilePath}", TraceContext.TraceId, LogLevel.Warning);
                    }
                }
            });
        }


        public ApiResponse<string> PngToIcoBase64(string pngPath, int size = 256)
        {
            return ExecuteSafe(() =>
            {
                this._logger.Log("Entering PngToIcoBase64()", TraceContext.TraceId, LogLevel.Trace);

                string tempFilePath = string.Empty;

                try
                {
                    this._logger.Log($"Validating PNG path: {pngPath}", TraceContext.TraceId, LogLevel.Debug);

                    // --- Validation ---
                    FileUtil.EnsureFileExist(pngPath, LogLevel.Error);
                    FileUtil.EnsureFileHasExtension(pngPath, new[] { ".png" });

                    string magickExePath = Path.Combine(
                        AppContext.BaseDirectory,
                        "tools",
                        "ImageMagick",
                        "magick.exe");

                    FileUtil.EnsureFileExist(magickExePath, LogLevel.Error);
                    this._logger.Log($"Resolved ImageMagick path: {magickExePath}", TraceContext.TraceId, LogLevel.Debug);

                    tempFilePath = Path.Combine(
                        ApplicationPathContext.AppTempFolderPath,
                        $"{Guid.NewGuid():N}.ico");
                    this._logger.Log($"Temporary ICO output path: {tempFilePath}", TraceContext.TraceId, LogLevel.Debug);

                    // --- Execute ImageMagick process ---
                    using (var process = new Process())
                    {
                        process.StartInfo.FileName = magickExePath;
                        process.StartInfo.Arguments =
                            $"\"{pngPath}\" -resize {size}x{size} -alpha on -background none -flatten \"{tempFilePath}\"";
                        process.StartInfo.CreateNoWindow = true;
                        process.StartInfo.UseShellExecute = false;

                        this._logger.Log($"Executing magick.exe with args: {process.StartInfo.Arguments}", TraceContext.TraceId, LogLevel.Trace);

                        process.Start();
                        process.WaitForExit();

                        this._logger.Log($"magick.exe exited with code {process.ExitCode}", TraceContext.TraceId, LogLevel.Debug);

                        this.EnsureProcessSafelyTerminated(
                            exitCode: process.ExitCode,
                            userMessage: this._l10nService.GetLocalizedMessage("Errors.ImageMagickProcessFailed"),
                            logMessage: "Magick.exe returned non-zero exit code during PNG->ICO conversion.",
                            logLevel: LogLevel.Warning
                        );
                    }

                    // --- Verify output file ---
                    FileUtil.EnsureFileExist(tempFilePath, LogLevel.Warning);
                    this._logger.Log("ImageMagick successfully generated ICO file", TraceContext.TraceId, LogLevel.Info);

                    // --- Convert to Base64 ---
                    byte[] bytes = File.ReadAllBytes(tempFilePath);
                    string base64 = Convert.ToBase64String(bytes);
                    string mimeType = FileUtil.GetMimeTypeFromAbsolutePath(tempFilePath);
                    this._logger.Log($"Converted ICO to Base64 (length: {base64.Length})", TraceContext.TraceId, LogLevel.Debug);

                    return Base64Util.AddHeadersToBase64(base64, mimeType);
                }
                finally
                {
                    // Cleanup temporary file
                    try
                    {
                        if (!string.IsNullOrEmpty(tempFilePath) && File.Exists(tempFilePath))
                        {
                            File.Delete(tempFilePath);
                            this._logger.Log($"Deleted temporary ICO file: {tempFilePath}", TraceContext.TraceId, LogLevel.Trace);
                        }
                    }
                    catch
                    {
                        this._logger.Log($"Failed to delete temporary ICO file: {tempFilePath}", TraceContext.TraceId, LogLevel.Warning);
                    }
                }
            });
        }


        public ApiResponse<string> ResizePng(string pngPath, int width, int height)
        {
            return ExecuteSafe(() =>
            {
                this._logger.Log("Entering ResizePng()", TraceContext.TraceId, LogLevel.Trace);

                string tempFilePath = string.Empty;

                try
                {
                    this._logger.Log($"Validating PNG path: {pngPath}", TraceContext.TraceId, LogLevel.Debug);

                    // --- Validation ---
                    FileUtil.EnsureFileExist(pngPath, LogLevel.Error);
                    FileUtil.EnsureFileHasExtension(pngPath, new[] { ".png" });

                    string magickExePath = Path.Combine(
                        AppContext.BaseDirectory,
                        "tools",
                        "ImageMagick",
                        "magick.exe");

                    FileUtil.EnsureFileExist(magickExePath, LogLevel.Error);
                    this._logger.Log($"Resolved ImageMagick path: {magickExePath}", TraceContext.TraceId, LogLevel.Debug);

                    tempFilePath = Path.Combine(
                        ApplicationPathContext.AppTempFolderPath,
                        $"{Guid.NewGuid():N}.png");
                    this._logger.Log($"Temporary output path for resized PNG: {tempFilePath}", TraceContext.TraceId, LogLevel.Debug);

                    // --- Build process info ---
                    string arguments = $"\"{pngPath}\" -resize {width}x{height}! \"{tempFilePath}\"";
                    this._logger.Log($"Constructed ImageMagick arguments: {arguments}", TraceContext.TraceId, LogLevel.Trace);

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
                        this._logger.Log("Starting ImageMagick resize process...", TraceContext.TraceId, LogLevel.Trace);

                        process.Start();
                        string errorOutput = process.StandardError.ReadToEnd();
                        process.WaitForExit();

                        this._logger.Log($"ImageMagick exited with code {process.ExitCode}", TraceContext.TraceId, LogLevel.Debug);

                        this.EnsureProcessSafelyTerminated(process.ExitCode,
                            "Failed to resize PNG image. Please check the file format or dimensions.",
                            $"ImageMagick process failed with exit code {process.ExitCode}: {errorOutput}",
                            LogLevel.Warning,
                            true);
                    }

                    // --- Verify output file ---
                    FileUtil.EnsureFileExist(tempFilePath, LogLevel.Warning);
                    this._logger.Log("Resized PNG file verified successfully", TraceContext.TraceId, LogLevel.Info);

                    // --- Convert resized PNG to Base64 ---
                    byte[] bytes = File.ReadAllBytes(tempFilePath);
                    string base64 = Convert.ToBase64String(bytes);
                    string mimeType = FileUtil.GetMimeTypeFromAbsolutePath(tempFilePath);
                    this._logger.Log($"Converted resized PNG to Base64 (length: {base64.Length})", TraceContext.TraceId, LogLevel.Debug);

                    return Base64Util.AddHeadersToBase64(base64, mimeType);
                }
                finally
                {
                    // --- Cleanup temp file ---
                    try
                    {
                        if (!string.IsNullOrEmpty(tempFilePath) && File.Exists(tempFilePath))
                        {
                            File.Delete(tempFilePath);
                            this._logger.Log($"Deleted temporary resized PNG file: {tempFilePath}", TraceContext.TraceId, LogLevel.Trace);
                        }
                    }
                    catch
                    {
                        this._logger.Log($"Failed to delete temporary resized PNG file: {tempFilePath}", TraceContext.TraceId, LogLevel.Warning);
                    }
                }
            });
        }


        public ApiResponse<(int Width, int Height)> GetPngDimensions(string pngPath)
        {
            return ExecuteSafe(() =>
            {
                this._logger.Log("Entering GetPngDimensions()", TraceContext.TraceId, LogLevel.Trace);

                string magickExePath = Path.Combine(
                    AppContext.BaseDirectory,
                    "tools",
                    "ImageMagick",
                    "magick.exe");

                this._logger.Log($"Preparing to read PNG dimensions for: {pngPath}", TraceContext.TraceId, LogLevel.Debug);

                // --- Validation ---
                FileUtil.EnsureFileExist(pngPath, LogLevel.Error);
                FileUtil.EnsureFileHasExtension(pngPath, new[] { ".png" });
                FileUtil.EnsureFileExist(magickExePath, LogLevel.Error);

                this._logger.Log($"Validated inputs. Using ImageMagick at: {magickExePath}", TraceContext.TraceId, LogLevel.Debug);

                string output;

                using (var process = new Process())
                {
                    process.StartInfo.FileName = magickExePath;
                    process.StartInfo.Arguments = $"identify -ping -format \"%w %h\" \"{pngPath}\"";
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;

                    this._logger.Log($"Executing ImageMagick identify command: {process.StartInfo.Arguments}", TraceContext.TraceId, LogLevel.Trace);

                    process.Start();

                    output = process.StandardOutput.ReadToEnd();
                    string errorOutput = process.StandardError.ReadToEnd();

                    process.WaitForExit();

                    this._logger.Log($"ImageMagick identify exited with code {process.ExitCode}", TraceContext.TraceId, LogLevel.Debug);

                    this.EnsureProcessSafelyTerminated(
                        exitCode: process.ExitCode,
                        userMessage: this._l10nService.GetLocalizedMessage("Errors.PngDimensionReadFailed"),
                        logMessage: $"ImageMagick 'identify' command failed for '{pngPath}' (ExitCode: {process.ExitCode}, Error: {errorOutput})",
                        logLevel: LogLevel.Error
                    );
                }

                this._logger.Log($"Raw identify output: '{output.Trim()}'", TraceContext.TraceId, LogLevel.Trace);

                // --- Parse output safely ---
                var parts = output.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 2 ||
                    !int.TryParse(parts[0], out int width) ||
                    !int.TryParse(parts[1], out int height))
                {
                    this._logger.Log($"Invalid ImageMagick output for '{pngPath}': '{output}'", TraceContext.TraceId, LogLevel.Warning);

                    throw AppException.Operational(
                        userMessage: this._l10nService.GetLocalizedMessage("Errors.ImageDimensionParseFailed"),
                        logMessage: $"Invalid ImageMagick output for '{pngPath}': '{output}'",
                        level: LogLevel.Error
                    );
                }

                this._logger.Log($"Parsed PNG dimensions successfully: Width={width}, Height={height}", TraceContext.TraceId, LogLevel.Info);

                return (width, height);
            });
        }


        public ApiResponse<string> ApplyOpacity(string pngPath, float opacityAmount)
        {
            return ExecuteSafe(() =>
            {
                this._logger.Log("Entering ApplyOpacity()", TraceContext.TraceId, LogLevel.Trace);

                string tempFilePath = string.Empty;

                try
                {
                    this._logger.Log($"Validating PNG path: {pngPath}", TraceContext.TraceId, LogLevel.Debug);

                    // --- Validation ---
                    FileUtil.EnsureFileExist(pngPath, LogLevel.Error);
                    FileUtil.EnsureFileHasExtension(pngPath, new[] { ".png" });

                    opacityAmount = Math.Clamp(opacityAmount, 0f, 1f);
                    this._logger.Log($"Clamped opacity amount to: {opacityAmount}", TraceContext.TraceId, LogLevel.Debug);

                    string magickExePath = Path.Combine(
                        AppContext.BaseDirectory,
                        "tools",
                        "ImageMagick",
                        "magick.exe");

                    FileUtil.EnsureFileExist(magickExePath, LogLevel.Error);
                    this._logger.Log($"Resolved ImageMagick path: {magickExePath}", TraceContext.TraceId, LogLevel.Debug);

                    tempFilePath = Path.Combine(
                        ApplicationPathContext.AppTempFolderPath,
                        $"{Guid.NewGuid():N}.png");
                    this._logger.Log($"Temporary output path: {tempFilePath}", TraceContext.TraceId, LogLevel.Debug);

                    string opacityInvariant = opacityAmount.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    this._logger.Log($"Building process for opacity adjustment with amount={opacityInvariant}", TraceContext.TraceId, LogLevel.Trace);

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

                        this._logger.Log($"Executing magick.exe with args: {process.StartInfo.Arguments}", TraceContext.TraceId, LogLevel.Trace);

                        process.Start();

                        string errorOutput = process.StandardError.ReadToEnd();
                        process.WaitForExit();

                        this._logger.Log($"magick.exe exited with code {process.ExitCode}", TraceContext.TraceId, LogLevel.Debug);

                        this.EnsureProcessSafelyTerminated(
                            exitCode: process.ExitCode,
                            userMessage: this._l10nService.GetLocalizedMessage("Errors.PngOpacityApplyFailed"),
                            logMessage: $"ImageMagick failed while adjusting opacity on '{pngPath}'. Error: {errorOutput}",
                            logLevel: LogLevel.Error
                        );
                    }

                    // --- Verify output and convert to base64 ---
                    FileUtil.EnsureFileExist(tempFilePath, LogLevel.Warning);
                    this._logger.Log("Opacity-adjusted PNG file successfully created", TraceContext.TraceId, LogLevel.Info);

                    string base64 = _fileController.GetBase64(tempFilePath).Data;
                    this._logger.Log($"Converted opacity-adjusted PNG to Base64 (length: {base64.Length})", TraceContext.TraceId, LogLevel.Debug);

                    return base64;
                }
                finally
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(tempFilePath))
                        {
                            _fileController.DeleteFile(tempFilePath);
                            this._logger.Log($"Deleted temporary file: {tempFilePath}", TraceContext.TraceId, LogLevel.Trace);
                        }
                    }
                    catch
                    {
                        this._logger.Log($"Failed to delete temporary file: {tempFilePath}", TraceContext.TraceId, LogLevel.Warning);
                    }
                }
            });
        }



        public ApiResponse<string> ApplyCornerRadius(string pngPath, float radiusAmount)
        {
            return ExecuteSafe(() =>
            {
                this._logger.Log("Entering ApplyCornerRadius()", TraceContext.TraceId, LogLevel.Trace);

                string tempFilePath = string.Empty;

                try
                {
                    this._logger.Log($"Validating PNG path: {pngPath}", TraceContext.TraceId, LogLevel.Debug);

                    // --- Validation ---
                    FileUtil.EnsureFileExist(pngPath, LogLevel.Error);
                    FileUtil.EnsureFileHasExtension(pngPath, new[] { ".png" });
                    radiusAmount = Math.Clamp(radiusAmount, 0f, 0.5f);
                    this._logger.Log($"Clamped radius amount to: {radiusAmount}", TraceContext.TraceId, LogLevel.Debug);

                    string magickExePath = Path.Combine(
                        AppContext.BaseDirectory,
                        "tools",
                        "ImageMagick",
                        "magick.exe");

                    FileUtil.EnsureFileExist(magickExePath, LogLevel.Error);
                    this._logger.Log($"Resolved ImageMagick path: {magickExePath}", TraceContext.TraceId, LogLevel.Debug);

                    tempFilePath = Path.Combine(
                        ApplicationPathContext.AppTempFolderPath,
                        $"{Guid.NewGuid():N}.png");
                    this._logger.Log($"Temporary output path for rounded PNG: {tempFilePath}", TraceContext.TraceId, LogLevel.Debug);

                    // --- Get PNG dimensions ---
                    this._logger.Log("Retrieving PNG dimensions before applying corner radius...", TraceContext.TraceId, LogLevel.Trace);
                    var dimResponse = this.GetPngDimensions(pngPath);
                    EnsureSuccess<(int w, int h)>(dimResponse);

                    (int w, int h) = dimResponse.Data;
                    int radiusInPixels = (int)(Math.Min(w, h) * radiusAmount);
                    this._logger.Log($"Calculated radius in pixels: {radiusInPixels} (Width={w}, Height={h})", TraceContext.TraceId, LogLevel.Debug);

                    // --- Build ImageMagick arguments ---
                    string args =
                        $"\"{pngPath}\" " +
                        "( +clone -alpha extract " +
                        $"-draw \"fill black polygon 0,0 0,{radiusInPixels} {radiusInPixels},0 fill white circle {radiusInPixels},{radiusInPixels} {radiusInPixels},0\" " +
                        "( +clone -flip ) -compose Multiply -composite " +
                        "( +clone -flop ) -compose Multiply -composite ) " +
                        "-alpha off -compose CopyOpacity -composite " +
                        $"\"{tempFilePath}\"";

                    this._logger.Log($"Built ImageMagick command args for corner rounding:\n{args}", TraceContext.TraceId, LogLevel.Trace);

                    // --- Execute ImageMagick ---
                    using (var process = new Process())
                    {
                        process.StartInfo.FileName = magickExePath;
                        process.StartInfo.Arguments = args;
                        process.StartInfo.CreateNoWindow = true;
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.RedirectStandardError = true;

                        this._logger.Log("Starting ImageMagick process for corner rounding...", TraceContext.TraceId, LogLevel.Trace);

                        process.Start();

                        string errorOutput = process.StandardError.ReadToEnd();
                        process.WaitForExit();

                        this._logger.Log($"magick.exe exited with code {process.ExitCode}", TraceContext.TraceId, LogLevel.Debug);

                        this.EnsureProcessSafelyTerminated(
                            exitCode: process.ExitCode,
                            userMessage: this._l10nService.GetLocalizedMessage("Errors.PngCornerRadiusApplyFailed"),
                            logMessage: $"ImageMagick failed while rounding corners for '{pngPath}'. Error: {errorOutput}",
                            logLevel: LogLevel.Error
                        );
                    }

                    // --- Verify output and convert to base64 ---
                    FileUtil.EnsureFileExist(tempFilePath, LogLevel.Warning);
                    this._logger.Log("Corner-radius applied PNG verified successfully", TraceContext.TraceId, LogLevel.Info);

                    string base64 = _fileController.GetBase64(tempFilePath).Data;
                    this._logger.Log($"Converted rounded PNG to Base64 (length: {base64.Length})", TraceContext.TraceId, LogLevel.Debug);

                    return base64;
                }
                finally
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(tempFilePath))
                        {
                            _fileController.DeleteFile(tempFilePath);
                            this._logger.Log($"Deleted temporary file: {tempFilePath}", TraceContext.TraceId, LogLevel.Trace);
                        }
                    }
                    catch
                    {
                        this._logger.Log($"Failed to delete temporary file: {tempFilePath}", TraceContext.TraceId, LogLevel.Warning);
                    }
                }
            });
        }



        public ApiResponse<string> IcoBase64ToPngBase64(string icoBase64)
        {
            return ExecuteSafe(() =>
            {
                this._logger.Log("Entering IcoBase64ToPngBase64()", TraceContext.TraceId, LogLevel.Trace);

                string tempIcoPath = string.Empty;

                try
                {
                    // --- Validation ---
                    if (string.IsNullOrWhiteSpace(icoBase64))
                    {
                        this._logger.Log("Received empty Base64 string for ICO conversion.", TraceContext.TraceId, LogLevel.Warning);

                        throw AppException.Operational(
                            userMessage: this._l10nService.GetLocalizedMessage("Errors.Base64DataEmpty"),
                            logMessage: "IcoBase64ToPngBase64 called with empty base64 string.",
                            level: LogLevel.Error
                        );
                    }

                    this._logger.Log("Validation passed. Preparing temporary ICO file for conversion...", TraceContext.TraceId, LogLevel.Debug);

                    tempIcoPath = Path.Combine(
                        ApplicationPathContext.AppTempFolderPath,
                        $"{Guid.NewGuid():N}.ico");

                    this._logger.Log($"Temporary ICO file path: {tempIcoPath}", TraceContext.TraceId, LogLevel.Trace);

                    // --- Write temporary ICO ---
                    this._logger.Log("Writing Base64 ICO data to temporary file...", TraceContext.TraceId, LogLevel.Trace);
                    var writeResponse = _fileController.WriteBase64(tempIcoPath, icoBase64);
                    EnsureSuccess<object>(writeResponse);
                    this._logger.Log("Temporary ICO file written successfully.", TraceContext.TraceId, LogLevel.Info);

                    // --- Convert to PNG base64 ---
                    this._logger.Log("Converting ICO to PNG Base64...", TraceContext.TraceId, LogLevel.Debug);
                    var pngResponse = this.IcoToPngBase64(tempIcoPath);
                    EnsureSuccess<string>(pngResponse);
                    this._logger.Log("ICO to PNG Base64 conversion successful.", TraceContext.TraceId, LogLevel.Info);

                    return pngResponse.Data!;
                }
                finally
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(tempIcoPath))
                        {
                            _fileController.DeleteFile(tempIcoPath);
                            this._logger.Log($"Deleted temporary ICO file: {tempIcoPath}", TraceContext.TraceId, LogLevel.Trace);
                        }
                    }
                    catch
                    {
                        this._logger.Log($"Failed to delete temporary ICO file: {tempIcoPath}", TraceContext.TraceId, LogLevel.Warning);
                    }
                }
            });
        }


        public ApiResponse<string> PngBase64ToIcoBase64(string pngBase64)
        {
            return ExecuteSafe(() =>
            {
                this._logger.Log("Entering PngBase64ToIcoBase64()", TraceContext.TraceId, LogLevel.Trace);

                string tempPngPath = string.Empty;

                try
                {
                    // --- Validation ---
                    if (string.IsNullOrWhiteSpace(pngBase64))
                    {
                        this._logger.Log("Received empty Base64 string for PNG conversion.", TraceContext.TraceId, LogLevel.Warning);

                        throw AppException.Operational(
                            userMessage: _l10nService.GetLocalizedMessage("Errors.Base64DataEmpty"),
                            logMessage: "PngBase64ToIcoBase64 called with empty base64 string.",
                            level: LogLevel.Error
                        );
                    }

                    this._logger.Log("Validation passed. Preparing to write temporary PNG file.", TraceContext.TraceId, LogLevel.Debug);

                    tempPngPath = Path.Combine(
                        ApplicationPathContext.AppTempFolderPath,
                        $"{Guid.NewGuid():N}.png");

                    this._logger.Log($"Temporary PNG file path: {tempPngPath}", TraceContext.TraceId, LogLevel.Trace);

                    // --- Write temporary PNG ---
                    this._logger.Log("Writing Base64 PNG data to temporary file...", TraceContext.TraceId, LogLevel.Trace);
                    var writeResponse = _fileController.WriteBase64(tempPngPath, pngBase64);
                    EnsureSuccess<object>(writeResponse);
                    this._logger.Log("Temporary PNG file written successfully.", TraceContext.TraceId, LogLevel.Info);

                    // --- Convert to ICO Base64 ---
                    this._logger.Log("Converting PNG to ICO Base64 using ImageMagick...", TraceContext.TraceId, LogLevel.Debug);
                    var icoResponse = this.PngToIcoBase64(tempPngPath);
                    EnsureSuccess<string>(icoResponse);
                    this._logger.Log("PNG to ICO Base64 conversion successful.", TraceContext.TraceId, LogLevel.Info);

                    return icoResponse.Data!;
                }
                finally
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(tempPngPath))
                        {
                            _fileController.DeleteFile(tempPngPath);
                            this._logger.Log($"Deleted temporary PNG file: {tempPngPath}", TraceContext.TraceId, LogLevel.Trace);
                        }
                    }
                    catch
                    {
                        this._logger.Log($"Failed to delete temporary PNG file: {tempPngPath}", TraceContext.TraceId, LogLevel.Warning);
                    }
                }
            });
        }



        public ApiResponse<string> ResizePngBase64(string pngBase64, int width, int height)
        {
            return ExecuteSafe(() =>
            {
                this._logger.Log("Entering ResizePngBase64()", TraceContext.TraceId, LogLevel.Trace);

                string tempPngPath = string.Empty;

                try
                {
                    if (string.IsNullOrWhiteSpace(pngBase64))
                    {
                        this._logger.Log("Received empty Base64 string for resize operation.", TraceContext.TraceId, LogLevel.Warning);

                        throw AppException.Operational(
                            userMessage: _l10nService.GetLocalizedMessage("Errors.Base64DataEmpty"),
                            logMessage: "ResizePngBase64 called with empty base64 string.",
                            level: LogLevel.Error
                        );
                    }

                    this._logger.Log($"Starting resize operation for Base64 PNG to {width}x{height}px.", TraceContext.TraceId, LogLevel.Debug);

                    tempPngPath = Path.Combine(
                        ApplicationPathContext.AppTempFolderPath,
                        $"{Guid.NewGuid():N}.png");

                    this._logger.Log($"Temporary PNG path for resizing: {tempPngPath}", TraceContext.TraceId, LogLevel.Trace);

                    var writeResponse = _fileController.WriteBase64(tempPngPath, pngBase64);
                    EnsureSuccess<object>(writeResponse);
                    this._logger.Log("Wrote Base64 PNG to temp file successfully.", TraceContext.TraceId, LogLevel.Info);

                    var resizeResponse = this.ResizePng(tempPngPath, width, height);
                    EnsureSuccess<string>(resizeResponse);
                    this._logger.Log("Resize operation completed successfully.", TraceContext.TraceId, LogLevel.Info);

                    return resizeResponse.Data!;
                }
                finally
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(tempPngPath))
                        {
                            _fileController.DeleteFile(tempPngPath);
                            this._logger.Log($"Deleted temporary PNG file after resize: {tempPngPath}", TraceContext.TraceId, LogLevel.Trace);
                        }
                    }
                    catch
                    {
                        this._logger.Log($"Failed to delete temporary PNG file: {tempPngPath}", TraceContext.TraceId, LogLevel.Warning);
                    }
                }
            });
        }

        public ApiResponse<string> ApplyOpacityOnPngBase64(string pngBase64, float opacityAmount)
        {
            return ExecuteSafe(() =>
            {
                this._logger.Log("Entering ApplyOpacityOnPngBase64()", TraceContext.TraceId, LogLevel.Trace);

                string tempPngPath = string.Empty;

                try
                {
                    if (string.IsNullOrWhiteSpace(pngBase64))
                    {
                        this._logger.Log("Received empty Base64 string for opacity adjustment.", TraceContext.TraceId, LogLevel.Warning);

                        throw AppException.Operational(
                            userMessage: _l10nService.GetLocalizedMessage("Errors.Base64DataEmpty"),
                            logMessage: "ApplyOpacityOnPngBase64 called with empty base64 string.",
                            level: LogLevel.Error
                        );
                    }

                    this._logger.Log($"Applying opacity: {opacityAmount}", TraceContext.TraceId, LogLevel.Debug);

                    tempPngPath = Path.Combine(
                        ApplicationPathContext.AppTempFolderPath,
                        $"{Guid.NewGuid():N}.png");

                    this._logger.Log($"Temporary PNG path for opacity adjustment: {tempPngPath}", TraceContext.TraceId, LogLevel.Trace);

                    var writeResponse = _fileController.WriteBase64(tempPngPath, pngBase64);
                    EnsureSuccess<object>(writeResponse);
                    this._logger.Log("Wrote PNG base64 to temp file successfully.", TraceContext.TraceId, LogLevel.Info);

                    var opacityResponse = this.ApplyOpacity(tempPngPath, opacityAmount);
                    EnsureSuccess<string>(opacityResponse);
                    this._logger.Log("Opacity applied successfully to PNG.", TraceContext.TraceId, LogLevel.Info);

                    return opacityResponse.Data!;
                }
                finally
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(tempPngPath))
                        {
                            _fileController.DeleteFile(tempPngPath);
                            this._logger.Log($"Deleted temporary PNG file after opacity adjustment: {tempPngPath}", TraceContext.TraceId, LogLevel.Trace);
                        }
                    }
                    catch
                    {
                        this._logger.Log($"Failed to delete temporary file: {tempPngPath}", TraceContext.TraceId, LogLevel.Warning);
                    }
                }
            });
        }

        public ApiResponse<string> ApplyCornerRadiusOnPngBase64(string pngBase64, float cornerRadius)
        {
            return ExecuteSafe(() =>
            {
                this._logger.Log("Entering ApplyCornerRadiusOnPngBase64()", TraceContext.TraceId, LogLevel.Trace);

                string tempPngPath = string.Empty;

                try
                {
                    if (string.IsNullOrWhiteSpace(pngBase64))
                    {
                        this._logger.Log("Received empty Base64 string for corner radius operation.", TraceContext.TraceId, LogLevel.Warning);

                        throw AppException.Operational(
                            userMessage: _l10nService.GetLocalizedMessage("Errors.Base64DataEmpty"),
                            logMessage: "ApplyCornerRadiusOnPngBase64 called with empty base64 string.",
                            level: LogLevel.Error
                        );
                    }

                    this._logger.Log($"Applying corner radius: {cornerRadius}", TraceContext.TraceId, LogLevel.Debug);

                    tempPngPath = Path.Combine(
                        ApplicationPathContext.AppTempFolderPath,
                        $"{Guid.NewGuid():N}.png");

                    this._logger.Log($"Temporary PNG path for corner radius operation: {tempPngPath}", TraceContext.TraceId, LogLevel.Trace);

                    var writeResponse = _fileController.WriteBase64(tempPngPath, pngBase64);
                    EnsureSuccess<object>(writeResponse);
                    this._logger.Log("Wrote PNG base64 to temp file successfully.", TraceContext.TraceId, LogLevel.Info);

                    var radiusResponse = this.ApplyCornerRadius(tempPngPath, cornerRadius);
                    EnsureSuccess<string>(radiusResponse);
                    this._logger.Log("Corner radius applied successfully.", TraceContext.TraceId, LogLevel.Info);

                    return radiusResponse.Data!;
                }
                finally
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(tempPngPath))
                        {
                            _fileController.DeleteFile(tempPngPath);
                            this._logger.Log($"Deleted temporary PNG file after corner radius operation: {tempPngPath}", TraceContext.TraceId, LogLevel.Trace);
                        }
                    }
                    catch
                    {
                        this._logger.Log($"Failed to delete temporary file: {tempPngPath}", TraceContext.TraceId, LogLevel.Warning);
                    }
                }
            });
        }

        public void EnsureProcessSafelyTerminated(
            int exitCode,
            string userMessage = "The external process terminated unexpectedly. Please try again or check file permissions.",
            string logMessage = "External process exited abnormally.",
            LogLevel logLevel = LogLevel.Error,
            bool isOperational = true)
        {
            this._logger.Log($"Validating external process exit code: {exitCode}", TraceContext.TraceId, LogLevel.Trace);

            if (exitCode != 0)
            {
                this._logger.Log($"Process exited with non-zero code: {exitCode} — raising AppException.", TraceContext.TraceId, LogLevel.Error);

                throw AppException.Operational(
                    userMessage: userMessage,
                    logMessage: logMessage,
                    level: logLevel
                );
            }

            this._logger.Log("External process terminated successfully (ExitCode=0).", TraceContext.TraceId, LogLevel.Info);
        }


    }
}
