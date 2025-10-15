using Model.Configs;
using Model.DTO;
using Model.ResponseTypes;
using System.Text.RegularExpressions;
using Util;

namespace Service
{
    public class PackService
    {
        private readonly FileService _fileService;

        private readonly ImageProcessorService _imageProcessorService;
        public PackService(FileService fileService, ImageProcessorService imageProcessorService)
        {
            _fileService = fileService;
            _imageProcessorService = imageProcessorService;
        }

        public ServiceResponse<PackConfig> CreatePack(CreatePackDTO dto)
        {
            string tempPngPath = string.Empty;
            try
            {
                string packRootFolderAbsolutePath = SD.PackFolderAbsolutePath;

                if (string.IsNullOrEmpty(dto.Version))
                    return ServiceResponse<PackConfig>.Fail("Version is required", true);
                
                Regex versionRegex = new Regex(@"^v\d+\.\d+\.\d+$");

                string normalizedVerison = !dto.Version.StartsWith("v") ? "v" + dto.Version : dto.Version;
                if(!versionRegex.IsMatch(normalizedVerison))
                    return ServiceResponse<PackConfig>.Fail("Version must be in format vX.Y.Z where X,Y,Z are integers", true);

                if (string.IsNullOrEmpty(dto.PackName))
                    return ServiceResponse<PackConfig>.Fail("PackName is required", true);

                // map dto to packConfig
                PackConfig packConfig = new PackConfig
                {
                    PackName = dto.PackName,
                    Uid = Guid.NewGuid().ToString(),
                    Version = normalizedVerison,
                    Author = dto.Author,
                    Licance = dto.Licance,
                    Description = dto.Description,
                    OpacityAmount = 1f, // default opacity of pack
                    CornerRadiusAmount = 0f // default corner radius of pack
                };

                // create pack Directory
                string packAbsolutePath = Path.Combine(packRootFolderAbsolutePath, packConfig.Uid);
                // 99.99999 chance guid is unique but just in case ENSURE IT
                if (Directory.Exists(packAbsolutePath))
                    return ServiceResponse<PackConfig>.Fail("Pack directory already exists", true);

                Directory.CreateDirectory(packAbsolutePath);

                // save base 64 cover img if exists
                if (!(string.IsNullOrEmpty(dto.Base64CoverImage) || string.IsNullOrWhiteSpace(dto.Base64CoverImage)))
                {
                    ServiceResponse<FileInfo> fileInfoResponse =  _fileService.GetFileInfoOfBase64(dto.Base64CoverImage);
                    if (!fileInfoResponse.Success || fileInfoResponse.Data == null)
                        return ServiceResponse<PackConfig>.Fail(
                            fileInfoResponse.Message ?? "Failed to read file information from the provided Base64 string.",
                            fileInfoResponse.IsOperational
                        );
                    FileInfo fileInfo = fileInfoResponse.Data;
                    if(fileInfo.Extension != ".png") 
                        return ServiceResponse<PackConfig>.Fail("Only PNG images are supported for cover image.", true);

                    tempPngPath = Path.Combine(SD.TempFolderAbsolutePath, $"{Guid.NewGuid().ToString()}.png");
                    _fileService.WriteBase64ToPath(dto.Base64CoverImage, tempPngPath);

                    // normlize dimantions to 255 x 255
                    ServiceResponse<string> imgProcessorResponse = _imageProcessorService.ResizePng(tempPngPath, 255 ,255);
                    if(!imgProcessorResponse.Success || imgProcessorResponse.Data == null) 
                        return ServiceResponse<PackConfig>.Fail(imgProcessorResponse.Message 
                            ?? "Failed to process image."
                            , imgProcessorResponse.IsOperational);

                    string normalizedBase64Png = imgProcessorResponse.Data;

                    string coverImagePath = Path.Combine(packAbsolutePath, "cover.png");
                    _fileService.WriteBase64ToPath(normalizedBase64Png, coverImagePath);

                }

                // save packConfig to packConfig.json inside pack directory
                string packConfigPath = Path.Combine(packAbsolutePath, "config.json");
                string serializedPackConfig = JsonUtil.Serialize(packConfig);

                File.WriteAllText(packConfigPath, serializedPackConfig);

                return ServiceResponse<PackConfig>.Ok(packConfig, "Pack created successfully");
            }
            catch (Exception ex)
            {
                return ServiceResponse<PackConfig>.Fail($"Exception: {ex.Message}", false);
            }
            finally
            {
                if(!string.IsNullOrEmpty(tempPngPath))
                {
                    _fileService.DeleteFile(tempPngPath);
                }
            }


        }


        public ServiceResponse<PackConfig> ReadPackConfig(string packId)
        {
            try
            {
                string packRootFolderAbsolutePath = SD.PackFolderAbsolutePath;
                string[] packDirectories = Directory.GetDirectories(packRootFolderAbsolutePath);
                string? selectedPackAbsolutePath = packDirectories.FirstOrDefault(dir => dir.Split("\\").LastOrDefault() == packId);
                if(string.IsNullOrEmpty(selectedPackAbsolutePath)) 
                    return ServiceResponse<PackConfig>.Fail("Pack not found", true);

                string packConfigPath = Path.Combine(selectedPackAbsolutePath, "config.json");
                if(!File.Exists(packConfigPath)) 
                    return ServiceResponse<PackConfig>.Fail("Pack config not found", true);

                string packConfigJson = File.ReadAllText(packConfigPath);
                PackConfig? config = JsonUtil.Deserialize<PackConfig>(packConfigJson);
                if(config == null) 
                    return ServiceResponse<PackConfig>.Fail("Failed to deserialize pack config", true);

                return ServiceResponse<PackConfig>.Ok(config, "Pack config read successfully");
            }
            catch(Exception ex)
            {
                return ServiceResponse<PackConfig>.Fail($"Exception: {ex.Message}", false);
            }
        }

        public ServiceResponse<object> DeletePackCoverImage(string packId)
        {
            try
            {
                string packRootFolderAbsolutePath = SD.PackFolderAbsolutePath;
                string[] packDirectories = Directory.GetDirectories(packRootFolderAbsolutePath);
                string? selectedPackAbsolutePath = packDirectories.FirstOrDefault(dir => dir.Split("\\").LastOrDefault() == packId);
                if (string.IsNullOrEmpty(selectedPackAbsolutePath))
                    return ServiceResponse<object>.Fail("Pack not found", true);
            
                string coverImagePath = Path.Combine(selectedPackAbsolutePath, "cover.png");
                if (!File.Exists(coverImagePath))
                    return ServiceResponse<object>.Fail("Cover image not found", true);

                ServiceResponse<object> deletionResponse = _fileService.DeleteFile(coverImagePath);
                if(!deletionResponse.Success)
                    return ServiceResponse<object>.Fail(deletionResponse.Message ?? "Failed to delete cover image", deletionResponse.IsOperational);
            
                return ServiceResponse<object>.OkMessage("Cover image deleted successfully");
            }
            catch(Exception ex)
            {
                return ServiceResponse<object>.Fail($"Exception: {ex.Message}", false);
            }
        }

        public ServiceResponse<object> ApplyCoverImage(string packId,  string base64Png)
        {
            try
            {
                string packRootFolderAbsolutePath = SD.PackFolderAbsolutePath;
                string[] packDirectories = Directory.GetDirectories(packRootFolderAbsolutePath);
                string? selectedPackAbsolutePath = packDirectories.FirstOrDefault(dir => dir.Split("\\").LastOrDefault() == packId);
                if (string.IsNullOrEmpty(selectedPackAbsolutePath))
                    return ServiceResponse<object>.Fail("Pack not found", true);

                string coverImagePath = Path.Combine(selectedPackAbsolutePath, "cover.png");
                // ensure delete existing cover image if exists
                if (File.Exists(coverImagePath))
                {
                    _fileService.DeleteFile(coverImagePath);
                }

                ServiceResponse<object> writeResponse = _fileService.WriteBase64ToPath(base64Png, coverImagePath, overwrite:true);
                if(!writeResponse.Success)
                    return ServiceResponse<object>.Fail(writeResponse.Message ?? "Failed to write cover image", writeResponse.IsOperational);
            
                return ServiceResponse<object>.OkMessage("Cover image applied successfully");
            }
            catch (Exception ex)
            {
                return ServiceResponse<object>.Fail($"Exception: {ex.Message}", false);
            }
        }

        public ServiceResponse<object> UpdatePackConfig(string packId, PackConfig newConfig)
        {
            try
            {
                string packRootFolderAbsolutePath = SD.PackFolderAbsolutePath;
                string[] packDirectories = Directory.GetDirectories(packRootFolderAbsolutePath);
                string? selectedPackAbsolutePath = packDirectories.FirstOrDefault(dir => dir.Split("\\").LastOrDefault() == packId);
                if (string.IsNullOrEmpty(selectedPackAbsolutePath))
                    return ServiceResponse<object>.Fail("Pack not found", true);

                string packConfigPath = Path.Combine(selectedPackAbsolutePath, "config.json");
                if(File.Exists(packConfigPath))
                    _fileService.DeleteFile(packConfigPath);    

                string serializedPackConfig = JsonUtil.Serialize(newConfig);
                File.WriteAllText(packConfigPath, serializedPackConfig);
                return ServiceResponse<object>.OkMessage("Pack config updated successfully");

            }
            catch(Exception ex)
            {
                return ServiceResponse<object>.Fail($"Exception: {ex.Message}", false);
            }
        }

        public ServiceResponse<object> DeletePack(string packId)
        {
            try
            {
                string packRootFolderAbsolutePath = SD.PackFolderAbsolutePath;
                string[] packDirectories = Directory.GetDirectories(packRootFolderAbsolutePath);
                string? selectedPackAbsolutePath = packDirectories.FirstOrDefault(dir => dir.Split("\\").LastOrDefault() == packId);
                if (string.IsNullOrEmpty(selectedPackAbsolutePath))
                    return ServiceResponse<object>.Fail("Pack not found", true);
                Directory.Delete(selectedPackAbsolutePath, true);
                return ServiceResponse<object>.OkMessage("Pack deleted successfully");
            }
            catch(Exception ex)
            {
                return ServiceResponse<object>.Fail($"Exception: {ex.Message}", false);
            }
        }
    }
}
