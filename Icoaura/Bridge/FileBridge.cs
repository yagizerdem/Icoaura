using Icoaura.Controller;
using Icoaura.Model;
using Icoaura.Util;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Icoaura.Bridge
{
    [System.Runtime.InteropServices.ClassInterface(System.Runtime.InteropServices.ClassInterfaceType.AutoDual)]
    [System.Runtime.InteropServices.ComVisible(true)]
    public class FileBridge
    {
        private readonly FileController _fileController;
        public FileBridge()
        {
            _fileController = DIProvider.Provider.GetRequiredService<FileController>();
        }

        public string GetBase64FromPath(string path)
        {
            ApiResponse<string> response =  _fileController.GetBase64(path);
            string serializedResponse = JsonUtil.Serialize(response);
            return serializedResponse;
        }

        public string SelectFilePath(string allowedExtensionsJsonArray)
        {
            string[] allowedExtensions = JsonUtil.Deserialize<string[]>(allowedExtensionsJsonArray) ?? [];
            string? filePath = PickFile(allowedExtensions);
            return filePath ?? string.Empty;
        }


        private string? PickFile(string[] allowedExtensions)
        {
            string joinedExtensions = string.Join(";", allowedExtensions.Select(ext =>
                ext.StartsWith(".") ? $"*{ext}" : $"*.{ext}"
            ));

            string titleExtensions = string.Join(", ", allowedExtensions.Select(ext =>
                ext.StartsWith(".") ? ext : $".{ext}"
            ));

            var dlg = new OpenFileDialog
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
    
    }
}
