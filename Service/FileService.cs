using Model.ResponseTypes;
using Util;

namespace Service
{
    public class FileService
    {
        public FileService()
        {
        
        }

        public ServiceResponse<FileInfo> GetFileInfo(string absolutePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(absolutePath))
                    return ServiceResponse<FileInfo>.Fail("Path cannot be null or empty", true);

                if (!File.Exists(absolutePath))
                    return ServiceResponse<FileInfo>.Fail("File does not exist", true);

                var fileInfo = new FileInfo(absolutePath);
                return ServiceResponse<FileInfo>.Ok(fileInfo);
            }
            catch (UnauthorizedAccessException ex)
            {
                return ServiceResponse<FileInfo>.Fail($"Access denied: {ex.Message}", false);
            }
            catch (IOException ex)
            {
                return ServiceResponse<FileInfo>.Fail($"IO error: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return ServiceResponse<FileInfo>.Fail($"Unexpected error: {ex.Message}", false);
            }
        }

        public ServiceResponse<string> GetBase64(string absolutePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(absolutePath))
                    return ServiceResponse<string>.Fail("Path cannot be null or empty", true);

                if (!File.Exists(absolutePath))
                    return ServiceResponse<string>.Fail("File does not exist", true);

                byte[] buffer = File.ReadAllBytes(absolutePath);
                string base64 = Convert.ToBase64String(buffer);
                return ServiceResponse<string>.Ok(base64);
            }
            catch (UnauthorizedAccessException ex)
            {
                return ServiceResponse<string>.Fail($"Access denied: {ex.Message}", false);
            }
            catch (IOException ex)
            {
                return ServiceResponse<string>.Fail($"IO error: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return ServiceResponse<string>.Fail($"Unexpected error: {ex.Message}", false);
            }
        }

        public ServiceResponse<byte[]> GetBuffer(string absolutePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(absolutePath))
                    return ServiceResponse<byte[]>.Fail("Path cannot be null or empty", true);

                if (!File.Exists(absolutePath))
                    return ServiceResponse<byte[]>.Fail("File does not exist", true);

                byte[] buffer = File.ReadAllBytes(absolutePath);
                return ServiceResponse<byte[]>.Ok(buffer);
            }
            catch (UnauthorizedAccessException ex)
            {
                return ServiceResponse<byte[]>.Fail($"Access denied: {ex.Message}", false);
            }
            catch (IOException ex)
            {
                return ServiceResponse<byte[]>.Fail($"IO error: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return ServiceResponse<byte[]>.Fail($"Unexpected error: {ex.Message}", false);
            }
        }

        public ServiceResponse<object> DeleteFile(string absolutePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(absolutePath))
                    return ServiceResponse<object>.Fail("Path cannot be null or empty", true);

                if (!File.Exists(absolutePath))
                    return ServiceResponse<object>.Fail("File does not exist", true);

                File.Delete(absolutePath);
                return ServiceResponse<object>.OkMessage("File deleted successfully");
            }
            catch (UnauthorizedAccessException ex)
            {
                return ServiceResponse<object>.Fail($"Access denied: {ex.Message}", false);
            }
            catch (IOException ex)
            {
                return ServiceResponse<object>.Fail($"IO error: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return ServiceResponse<object>.Fail($"Unexpected error: {ex.Message}", false);
            }
        }


        // cut + paste old file is removed from old path and added to new path
        public ServiceResponse<object> MoveToPath(string sourceAbsolutePath, string destAbsolutePath, bool overwrite = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sourceAbsolutePath) || string.IsNullOrWhiteSpace(destAbsolutePath))
                    return ServiceResponse<object>.Fail("Source or destination path cannot be null or empty", true);

                if (!File.Exists(sourceAbsolutePath))
                    return ServiceResponse<object>.Fail("Source file does not exist", true);

                if (File.Exists(destAbsolutePath))
                {
                    if (overwrite)
                    {
                        File.Delete(destAbsolutePath);
                    }
                    else
                    {
                        return ServiceResponse<object>.Fail("Destination file already exists", true);
                    }
                }

                File.Move(sourceAbsolutePath, destAbsolutePath);
                return ServiceResponse<object>.OkMessage("File moved successfully");
            }
            catch (UnauthorizedAccessException ex)
            {
                return ServiceResponse<object>.Fail($"Access denied: {ex.Message}", false);
            }
            catch (IOException ex)
            {
                return ServiceResponse<object>.Fail($"IO error: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return ServiceResponse<object>.Fail($"Unexpected error: {ex.Message}", false);
            }
        }

        public ServiceResponse<object> WriteBase64ToPath(string base64, string destAbsolutePath, bool overwrite = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(base64))
                    return ServiceResponse<object>.Fail("Base64 string cannot be null or empty", true);

                if (string.IsNullOrWhiteSpace(destAbsolutePath))
                    return ServiceResponse<object>.Fail("Destination path cannot be null or empty", true);

                string fullPath = Path.GetFullPath(destAbsolutePath);
                string? directory = Path.GetDirectoryName(fullPath);

                if (string.IsNullOrWhiteSpace(directory))
                    return ServiceResponse<object>.Fail("Invalid destination path", true);

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                if (File.Exists(fullPath))
                {
                    if (overwrite)
                        File.Delete(fullPath);
                    else
                        return ServiceResponse<object>.Fail("File already exists", true);
                }

                byte[] bytes;
                try
                {
                    bytes = Convert.FromBase64String(base64);
                }
                catch (FormatException)
                {
                    return ServiceResponse<object>.Fail("Invalid Base64 string", true);
                }

                File.WriteAllBytes(fullPath, bytes);
                return ServiceResponse<object>.OkMessage("File written successfully");
            }
            catch (UnauthorizedAccessException ex)
            {
                return ServiceResponse<object>.Fail($"Access denied: {ex.Message}", false);
            }
            catch (IOException ex)
            {
                return ServiceResponse<object>.Fail($"IO error: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return ServiceResponse<object>.Fail($"Unexpected error: {ex.Message}", false);
            }
        }


        public ServiceResponse<object> WriteBufferToPath(byte[] buffer, string destAbsolutePath, bool overwrite = false)
        {
            try
            {
                if (buffer == null || buffer.Length == 0)
                    return ServiceResponse<object>.Fail("Buffer cannot be null or empty", true);

                if (string.IsNullOrWhiteSpace(destAbsolutePath))
                    return ServiceResponse<object>.Fail("Destination path cannot be null or empty", true);

                string fullPath = Path.GetFullPath(destAbsolutePath);
                string? directory = Path.GetDirectoryName(fullPath);

                if (string.IsNullOrWhiteSpace(directory))
                    return ServiceResponse<object>.Fail("Invalid destination path", true);

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                if (File.Exists(fullPath))
                {
                    if (overwrite)
                        File.Delete(fullPath);
                    else
                        return ServiceResponse<object>.Fail("File already exists", true);
                }

                File.WriteAllBytes(fullPath, buffer);
                return ServiceResponse<object>.OkMessage("File written successfully");
            }
            catch (UnauthorizedAccessException ex)
            {
                return ServiceResponse<object>.Fail($"Access denied: {ex.Message}", false);
            }
            catch (IOException ex)
            {
                return ServiceResponse<object>.Fail($"IO error: {ex.Message}", false);
            }
            catch (Exception ex)
            {
                return ServiceResponse<object>.Fail($"Unexpected error: {ex.Message}", false);
            }
        }

        public ServiceResponse<bool> IsFileAllowedMimeType(string[] extensions, string absolutePath)
        {
            ServiceResponse<FileInfo> response = this.GetFileInfo(absolutePath);
            if (!response.Success || response.Data == null)
                return ServiceResponse<bool>.Fail(response.Message ?? "Failed to get file info", response.IsOperational);

            string fileExtension = response.Data.Extension.ToLower();
            bool isAllowed = extensions.Select(e => e.ToLower()).ToList().Contains(fileExtension);
            return ServiceResponse<bool>.Ok(isAllowed);
        }

        public ServiceResponse<bool> IsFilePng(string absolutePath) => IsFileAllowedMimeType([".png"], absolutePath);
        public ServiceResponse<bool> IsFileIco(string absolutePath) => IsFileAllowedMimeType([".ico"], absolutePath);
    
        public ServiceResponse<FileInfo> GetFileInfoOfBase64(string base64)
        {
            string tempFilePath = Path.Combine(SD.TempFolderAbsolutePath, Guid.NewGuid().ToString());

            try
            {
                ServiceResponse<object> resopnse = this.WriteBase64ToPath(base64, tempFilePath, false);
                if(!resopnse.Success)
                {
                    return ServiceResponse<FileInfo>.Fail(resopnse.Message ?? 
                        "failed to write file to temp folder", resopnse.IsOperational);
                }
                return this.GetFileInfo(tempFilePath);
            }
            finally
            {
                this.DeleteFile(tempFilePath);
            }
        }
    
        public ServiceResponse<FileInfo> GetFileInfoOfBuffer(byte[] buffer)
        {
            string tempFilePath = Path.Combine(SD.TempFolderAbsolutePath, Guid.NewGuid().ToString());

            try
            {
                ServiceResponse<object> resopnse = this.WriteBufferToPath(buffer, tempFilePath, false);
                if (!resopnse.Success)
                {
                    return ServiceResponse<FileInfo>.Fail(resopnse.Message ??
                        "failed to write file to temp folder", resopnse.IsOperational);
                }
                return this.GetFileInfo(tempFilePath);
            }
            finally
            {
                this.DeleteFile(tempFilePath);
            }
        }
        
    }

}
