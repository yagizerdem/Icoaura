using Model.ResponseTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;

namespace Service
{
    public class ImageProcessorService
    {
        private readonly FileService _fileService;
        public ImageProcessorService(FileService fileService)
        {
            _fileService = fileService;
        }

        // returns base64 string of the image
        public ServiceResponse<string> ResizePng(string pngPath, int width, int height)
        {
            string tempFileAbsolutePath = null!;

            try
            {
                if (!File.Exists(pngPath))
                {
                    return ServiceResponse<string>.Fail("File does not exist");
                }

                ServiceResponse<FileInfo> resopnse = _fileService.GetFileInfo(pngPath);
                if (!resopnse.Success)
                {
                    return ServiceResponse<string>.Fail(resopnse.Message ?? "Unknown error getting file info");
                }

                FileInfo fileInfo = resopnse.Data!;
                if (fileInfo.Extension != ".png")
                {
                    return ServiceResponse<string>.Fail("File is not a PNG image");
                }

                string tempFolderPath = SD.TempFolderAbsolutePath;


                string workingDir = Directory.GetCurrentDirectory();
                string rootPath = Path.Combine(workingDir, "tools");
                string exeAbsolutepath = Path.Combine(rootPath, "ImageMagic", "magick.exe");
                string tempFileNamme = $"{Guid.NewGuid().ToString()}.png";
                tempFileAbsolutePath = Path.Combine(tempFolderPath, tempFileNamme);


                string arguments = $"\"{pngPath}\" -resize {width}x{height}! \"{tempFileAbsolutePath}\"";


                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = false;
                startInfo.UseShellExecute = false;
                startInfo.FileName = exeAbsolutepath;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.Arguments = arguments;
                startInfo.RedirectStandardError = true;
                startInfo.RedirectStandardOutput = true;

                try
                {
                    using (Process exeProcess = Process.Start(startInfo))
                    {
                        string error = exeProcess.StandardError.ReadToEnd();
                        exeProcess.WaitForExit();

                        if (exeProcess.ExitCode != 0)
                            return ServiceResponse<string>.Fail($"ImageMagick failed: {error}");
                    }

                    ServiceResponse<string> response = _fileService.GetBase64(tempFileAbsolutePath);
                    if (!resopnse.Success)
                    {
                        return ServiceResponse<string>.Fail(response.Message ?? "Unknown error converting file to base64");
                    }

                    string base64 = response.Data!;
                    return ServiceResponse<string>.Ok(base64);
                }
                catch (Exception ex)
                {
                    return ServiceResponse<string>.Fail($"Error applying opacity: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                return ServiceResponse<string>.Fail($"Error applying opacity: {ex.Message}");
            }
            finally
            {
                if (tempFileAbsolutePath != null)
                {
                    _fileService.DeleteFile(tempFileAbsolutePath);
                }
            }
        }

        // get width and height of the png via system.drawing
        public ServiceResponse<(int, int)> GetDimentions(string pngPath)
        {
            if (string.IsNullOrEmpty(pngPath))
            {
                return ServiceResponse<(int, int)>.Fail("Path is null or empty", true);
            }

            if (!File.Exists(pngPath))
            {
                return ServiceResponse<(int, int)>.Fail("File does not exist", true);
            }

            ServiceResponse<FileInfo> resopnse = _fileService.GetFileInfo(pngPath);
            if (!resopnse.Success)
            {
                return ServiceResponse<(int, int)>.Fail(resopnse.Message ?? 
                    "Unknown error getting file info", 
                    resopnse.IsOperational);
            }

            FileInfo fileInfo = resopnse.Data!;
            if (fileInfo.Extension != ".png")
            {
                return ServiceResponse<(int, int)>.Fail("File is not a PNG image", true);
            }

            try
            {
                using (var image = System.Drawing.Image.FromFile(pngPath))
                {
                    return ServiceResponse<(int, int)>.Ok((image.Width, image.Height));
                }
            }
            catch (Exception ex)
            {
                return ServiceResponse<(int, int)>.Fail($"Error getting image dimensions: {ex.Message}");
            }
        }

        // returns base64 string of the image
        public ServiceResponse<string> ApplyOpacity(string pngPath, float opacityAmount)
        {
            string tempFileAbsolutePath = null!;

            try
            {
                if (!File.Exists(pngPath))
                {
                    return ServiceResponse<string>.Fail("File does not exist");
                }

                ServiceResponse<FileInfo> resopnse = _fileService.GetFileInfo(pngPath);
                if (!resopnse.Success)
                {
                    return ServiceResponse<string>.Fail(resopnse.Message ?? "Unknown error getting file info");
                }

                FileInfo fileInfo = resopnse.Data!;
                if (fileInfo.Extension != ".png")
                {
                    return ServiceResponse<string>.Fail("File is not a PNG image");
                }

                string tempFolderPath = SD.TempFolderAbsolutePath;


                string workingDir = Directory.GetCurrentDirectory();
                string rootPath = Path.Combine(workingDir, "tools");
                string exeAbsolutepath = Path.Combine(rootPath, "ImageMagic", "magick.exe");
                string tempFileNamme = $"{Guid.NewGuid().ToString()}.png";
                tempFileAbsolutePath = Path.Combine(tempFolderPath, tempFileNamme);

                string arguments = $"\"{pngPath}\" -alpha set -channel A -evaluate Multiply {opacityAmount.ToString(System.Globalization.CultureInfo.InvariantCulture)} +channel \"{tempFileAbsolutePath}\"";


                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = false;
                startInfo.UseShellExecute = false;
                startInfo.FileName = exeAbsolutepath;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.Arguments = arguments;
                startInfo.RedirectStandardError = true;
                startInfo.RedirectStandardOutput = true;

                try
                {
                    using (Process exeProcess = Process.Start(startInfo))
                    {
                        string error = exeProcess.StandardError.ReadToEnd();
                        exeProcess.WaitForExit();

                        if (exeProcess.ExitCode != 0)
                            return ServiceResponse<string>.Fail($"ImageMagick failed: {error}");
                    }

                    ServiceResponse<string> response = _fileService.GetBase64(tempFileAbsolutePath);
                    if (!resopnse.Success)
                    {
                        return ServiceResponse<string>.Fail(response.Message ?? "Unknown error converting file to base64");
                    }

                    string base64 = response.Data!;
                    return ServiceResponse<string>.Ok(base64);
                }
                catch (Exception ex)
                {
                    return ServiceResponse<string>.Fail($"Error applying opacity: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                return ServiceResponse<string>.Fail($"Error applying opacity: {ex.Message}");
            }
            finally
            {
                if (tempFileAbsolutePath != null)
                {
                    _fileService.DeleteFile(tempFileAbsolutePath);
                }
            }

        }

        // returns base64 string of the image
        public ServiceResponse<string> ApplyCornerRadius(string pngPath, float radius)
        {
            string tempFileAbsolutePath = null!;
            try
            {
                if (!File.Exists(pngPath))
                    return ServiceResponse<string>.Fail("File does not exist");

                ServiceResponse<FileInfo> response = _fileService.GetFileInfo(pngPath);
                if (!response.Success)
                    return ServiceResponse<string>.Fail(response.Message ?? "Unknown error getting file info");

                FileInfo fileInfo = response.Data!;
                if (fileInfo.Extension.ToLower() != ".png")
                    return ServiceResponse<string>.Fail("File is not a PNG image");

                string tempFolderPath = SD.TempFolderAbsolutePath;

                string workingDir = Directory.GetCurrentDirectory();
                string rootPath = Path.Combine(workingDir, "tools");
                string exeAbsolutepath = Path.Combine(rootPath, "ImageMagic", "magick.exe");

                string tempFileName = $"{Guid.NewGuid().ToString()}.png";
                tempFileAbsolutePath = Path.Combine(tempFolderPath, tempFileName);


                ServiceResponse<(int, int)> response_ = GetDimentions(pngPath);
                if (!response_.Success)
                {
                    return ServiceResponse<string>.Fail(response.Message ?? "Unknown error getting image dimensions");
                }
                (int width, int height) = response_.Data!;

                int minDimension = Math.Min(width, height);
                int radiusInPixels = (int)(radius * minDimension / 2);


                string arguments = $"\"{pngPath}\" " +
                                  $"( +clone -alpha extract -virtual-pixel black -blur 0x{radiusInPixels} -threshold 50%% ) " +
                                  $"-alpha off -compose CopyOpacity -composite " +
                                  $"\"{tempFileAbsolutePath}\"";

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    FileName = exeAbsolutepath,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = arguments,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                };

                using (Process exeProcess = Process.Start(startInfo)!)
                {
                    string error = exeProcess.StandardError.ReadToEnd();
                    string output = exeProcess.StandardOutput.ReadToEnd();
                    exeProcess.WaitForExit();

                    if (exeProcess.ExitCode != 0)
                        return ServiceResponse<string>.Fail($"ImageMagick failed: {error}");
                }

                ServiceResponse<string> base64Response = _fileService.GetBase64(tempFileAbsolutePath);
                if (!base64Response.Success)
                    return ServiceResponse<string>.Fail(base64Response.Message ?? "Unknown error converting file to base64");

                string base64 = base64Response.Data!;
                return ServiceResponse<string>.Ok(base64);
            }
            catch (Exception ex)
            {
                return ServiceResponse<string>.Fail($"Error applying corner radius: {ex.Message}");
            }
            finally
            {
                if (tempFileAbsolutePath != null)
                    _fileService.DeleteFile(tempFileAbsolutePath);
            }
        }

    }
}
