using Icoaura.Context;
using Icoaura.Controller;
using Icoaura.Model;
using Icoaura.Util;
using Microsoft.Extensions.DependencyInjection;
using Model.DTO;
using System.IO;
using System.Windows.Data;
using System.Windows.Forms;

namespace Icoaura.Bridge
{
    [System.Runtime.InteropServices.ClassInterface(System.Runtime.InteropServices.ClassInterfaceType.AutoDual)]
    [System.Runtime.InteropServices.ComVisible(true)]
    public class FileBridge
    {
        private readonly FileController _fileController;

        private readonly ImportExportController _importExporController;
        public FileBridge()
        {
            _fileController = DIProvider.Provider.GetRequiredService<FileController>();
            _importExporController = DIProvider.Provider.GetRequiredService<ImportExportController>();
        }

        public string GetBase64FromPath(string path)
        {
            TraceContext.Reset();
            ApiResponse<string> response =  _fileController.GetBase64(path);
            string serializedResponse = JsonUtil.Serialize(response);
            return serializedResponse;
        }

        public string SelectFilePath(string allowedExtensionsJsonArray)
        {
            TraceContext.Reset();
            string[] allowedExtensions = JsonUtil.Deserialize<string[]>(allowedExtensionsJsonArray) ?? [];
            string? filePath = PickFile(allowedExtensions);
            return filePath ?? string.Empty;
        }

        public string SelectFileRelativeFilePath(string allowedExtensionJsonArray)
        {
            TraceContext.Reset();
            string[] allowedExtensions = JsonUtil.Deserialize<string[]>(allowedExtensionJsonArray) ?? [];
            string? filePath = PickFile(allowedExtensions);
            if (filePath == null)
            {
                return string.Empty;
            }
            string relativePath = PathUtil.ConvertToRelativePath(filePath);
            return relativePath;
        }

        public string SelectRelativeDirectoryPath()
        {
            TraceContext.Reset();
            string? folderPath = PickFolder();
            string relativePath = PathUtil.ConvertToRelativePath(folderPath ?? string.Empty);
            return relativePath;
        }

        public string SelectDirectoryPath()
        {
            TraceContext.Reset();
            string? folderPath = PickFolder();
            return folderPath ?? string.Empty;
        }


        public bool IsFileSystemEntryExist(string relativePath)
        {
            TraceContext.Reset();
            if (File.Exists(relativePath)) return true;
            string? firstPart = relativePath.Split("\\").FirstOrDefault();
            string remainingPart = relativePath.Substring(firstPart?.Length ?? 0).TrimStart('\\');

            List<string> matches = new();

            if (!string.IsNullOrEmpty(firstPart))
            {
                matches = _fileController.GetMatchingFileSystemEntries(
                    PathUtil.Resolve(firstPart),
                    remainingPart
                ).Data ?? new();

                return matches.Count > 0;
            }

            return false;
        }

        public string ExportPack(string packId)
        {
            TraceContext.Reset();
            ApiResponse<object> response = _importExporController.ExportPack(packId);
            string serializedResponse = JsonUtil.Serialize(response);
            return serializedResponse;
        }

        public string ImportPack(string packPath)
        {
            TraceContext.Reset();
            ApiResponse<object> response = _importExporController.ImportPack(packPath);
            string serializedResponse = JsonUtil.Serialize(response);
            return serializedResponse;
        }

        private string? PickFile(string[] allowedExtensions)
        {
            string joinedExtensions = string.Join(";", allowedExtensions.Select(ext =>
                ext.StartsWith(".") ? $"*{ext}" : $"*.{ext}"
            ));

            string titleExtensions = string.Join(", ", allowedExtensions.Select(ext =>
                ext.StartsWith(".") ? ext : $".{ext}"
            ));

            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Title = $"Select File ({titleExtensions})",
                Filter = $"Supported Files|{joinedExtensions}|All Files (*.*)|*.*",
                Multiselect = false,
                CheckFileExists = true,
                CheckPathExists = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                RestoreDirectory = true
            };

            return dlg.ShowDialog() == true ? dlg.FileName : null;
        }

        private string? PickFolder()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select a folder";
                dialog.UseDescriptionForTitle = true;
                dialog.ShowNewFolderButton = true;
                dialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                    return dialog.SelectedPath;

                return null;
            }
        }

        public string GetLnkMetaData(string lnkPath)
        {
            TraceContext.Reset();
            ApiResponse<LnkMetaData> response = _fileController.GetLnkMetaData(lnkPath);
            string serializedResponse = JsonUtil.Serialize(response);
            return serializedResponse;
        }

        public string GetUrlMetaData(string urlPath)
        {
            TraceContext.Reset();
            ApiResponse<UrlMetaData> response = _fileController.GetUrlMetaData(urlPath);
            string serializedResponse = JsonUtil.Serialize(response);
            return serializedResponse;
        }

        public string GetDirMetaData(string dirPath)
        {
            TraceContext.Reset();
            ApiResponse<DirMetaData> response = _fileController.GetDirMetaData(dirPath);
            string serializedResponse = JsonUtil.Serialize(response);
            return serializedResponse;
        }

    }
}
