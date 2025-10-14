using IWshRuntimeLibrary;
using Model.FileContext;
using Model.ResponseTypes;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Util;

namespace Service
{
    public class LnkService
    {
        private readonly FileService _fileService;
        public LnkService(FileService fileService)
        {
            _fileService = fileService;
        }

        // get lnk file absolute paths whcih are inside of a directory NOT RECURSIVE
        public ServiceResponse<List<string>> GetLnkAbsolutePathsInsideDir(string rootFolder)
        {
            if(!Directory.Exists(rootFolder))
            {
                return ServiceResponse<List<string>>.Fail("Directory does not exist", true);
            }
            List<string> lnkAbsolutePaths = Directory.GetFiles(rootFolder, "*.lnk", SearchOption.TopDirectoryOnly).ToList();
            return ServiceResponse<List<string>>.Ok(lnkAbsolutePaths);
        }

        public ServiceResponse<List<string>> GetLnkAbsolutePathsUnderDesktop() => GetLnkAbsolutePathsInsideDir(Environment.ExpandEnvironmentVariables("%DESKTOP%"));

        public ServiceResponse<LnkMetaData> GetLnkMetaData(string lnkAbsolutePath)
        {
            if(!System.IO.File.Exists(lnkAbsolutePath))
            {
                return ServiceResponse<LnkMetaData>.Fail("Lnk file does not exist", true);
            }

            LnkMetaData lnkMetaData = new LnkMetaData();

            ServiceResponse<string> targetExePathResponse = ExtractLnkTargetExePath(lnkAbsolutePath);
            if (targetExePathResponse.Success)
            {
                lnkMetaData.TargetExePath = targetExePathResponse.Data!;
            }

            ServiceResponse<string> iconPathResponse = ExtractLnkIconPath(lnkAbsolutePath);
            if (iconPathResponse.Success)
            {
                lnkMetaData.TargetIcoPath = iconPathResponse.Data!;
            }

            ServiceResponse<string> descriptionResponse = ExtractLnkDescription(lnkAbsolutePath);
            if (descriptionResponse.Success)
            {
                lnkMetaData.Description = descriptionResponse.Data!;
            }

            return ServiceResponse<LnkMetaData>.Ok(lnkMetaData);
        }


        // extract built in icon from exe binary
        public ServiceResponse<string> ExtractIconAsBase64FromExe(string exeAbsolutePath)
        {
            string extractIconToolAbsolutePath = Path.Combine(Environment.CurrentDirectory,
                "tools",
                "extracticon.exe");

            string tempFileAbsolutePath = Path.Combine(SD.TempFolderAbsolutePath,
                $"{Guid.NewGuid().ToString()}.png");

            try
            {
         
                if(!System.IO.File.Exists(exeAbsolutePath))
                {
                    return ServiceResponse<string>.Fail("Executable file does not exist", true);
                }


                using Process process = new Process();

                process.StartInfo.FileName = extractIconToolAbsolutePath;
                process.StartInfo.Arguments = $"\"{exeAbsolutePath}\" \"{tempFileAbsolutePath}\"";
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false; 
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            
                process.Start();

                string stdOut = process.StandardOutput.ReadToEnd();
                string stdErr = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if(process.ExitCode != 0)
                {
                    return ServiceResponse<string>.Fail($"Failed to extract icon. Error: {stdErr}", false);
                }

                ServiceResponse<string> base64Response = _fileService.GetBase64(tempFileAbsolutePath);

                if (!base64Response.Success)
                {
                    return ServiceResponse<string>.Fail(base64Response.Message ?? 
                        "Failed to convert extracted icon to base64", base64Response.IsOperational);
                }

                return ServiceResponse<string>.Ok(base64Response.Data!);

            }
            finally
            {
                _fileService.DeleteFile(tempFileAbsolutePath);
            }
        }


        // extract lnk meta data functions
        public ServiceResponse<string> ExtractLnkTargetExePath(string lnkPath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(lnkPath))
                    return ServiceResponse<string>.Fail("Path cannot be null or empty.", true);

                if (!System.IO.File.Exists(lnkPath))
                    return ServiceResponse<string>.Fail($"Shortcut not found: {lnkPath}", true);

                var shell = new WshShell();
                var shortcut = (IWshShortcut)shell.CreateShortcut(lnkPath);

                string target = shortcut.TargetPath;

                if (string.IsNullOrWhiteSpace(target))
                {
                    return ServiceResponse<string>.Fail("Could not resolve target path from shortcut.", true);
                }

                return ServiceResponse<string>.Ok(target);

            }
            catch (Exception ex)
            {
                return ServiceResponse<string>.Fail($"Error resolving shortcut target: {ex.Message}", false);
            }
        }

        public ServiceResponse<string> ExtractLnkIconPath(string lnkPath)
        {
            using (var br = new BinaryReader(System.IO.File.OpenRead(lnkPath)))
            {
                uint headerSize = br.ReadUInt32();
                br.ReadBytes(16);
                uint flags = br.ReadUInt32();
                bool hasiconLocation = (flags & 0x40) == 0x40;
                bool hasLinkTargetIDList = (flags & 0x1) == 0x1;
                bool hasLinkInfo = (flags & 0x2) == 0x2;
                bool isUnicode = (flags & 0x80) == 0x80;
                bool hasName = (flags & 0x04) == 0x04;
                bool hasRelativePath = (flags & 0x08) == 0x08;
                bool hasWorkingDir = (flags & 0x10) == 0x10;
                bool hasArguments = (flags & 0x20) == 0x20;
                bool hasLocation = (flags & 0x40) == 0x40;

                br.ReadBytes((int)headerSize - 24);
                if (!hasiconLocation) return ServiceResponse<string>.Fail("has no associated icon location", true);

                if (hasLinkTargetIDList)
                {
                    ushort idListSize = br.ReadUInt16();
                    br.ReadBytes(idListSize);
                }

                if (hasLinkInfo)
                {
                    uint linkInfoSize = br.ReadUInt32();
                    br.ReadBytes((int)linkInfoSize - 4);
                }

                // parse String data field

                // NAME_STRING
                if (hasName)
                {
                    ushort name_string_count = br.ReadUInt16();
                    br.ReadBytes(isUnicode ? name_string_count * 2 : name_string_count * 1);
                }

                //RELATIVE_PATH
                if (hasRelativePath)
                {
                    ushort relative_path_count = br.ReadUInt16();
                    br.ReadBytes(isUnicode ? relative_path_count * 2 : relative_path_count * 1);
                }

                //WORKING_DIR
                if (hasWorkingDir)
                {
                    ushort work_dir_count = br.ReadUInt16();
                    br.ReadBytes(isUnicode ? work_dir_count * 2 : work_dir_count * 1);
                }

                //COMMAND_LINE_ARGUMENTS
                if (hasArguments)
                {
                    ushort command_line_arguments_count = br.ReadUInt16();
                    br.ReadBytes(isUnicode ? command_line_arguments_count * 2 : command_line_arguments_count * 1);

                }

                //ICON_LOCATION
                if (hasLocation)
                {
                    ushort icon_location_count = br.ReadUInt16();
                    byte[] buffer = br.ReadBytes(isUnicode ? icon_location_count * 2 : icon_location_count * 1);

                    string iconLocation = isUnicode
                        ? Encoding.Unicode.GetString(buffer)
                        : Encoding.Default.GetString(buffer);

                    return ServiceResponse<string>.Ok(iconLocation);
                }


                return ServiceResponse<string>.Fail("has no associated icon location", true);
            }
        }

        public ServiceResponse<string> ExtractLnkDescription(string lnkPath)
        {
            using (var br = new BinaryReader(System.IO.File.OpenRead(lnkPath)))
            {
                uint headerSize = br.ReadUInt32();
                br.ReadBytes(16);
                uint flags = br.ReadUInt32();
                bool hasiconLocation = (flags & 0x40) == 0x40;
                bool hasLinkTargetIDList = (flags & 0x1) == 0x1;
                bool hasLinkInfo = (flags & 0x2) == 0x2;
                bool isUnicode = (flags & 0x80) == 0x80;
                bool hasName = (flags & 0x04) == 0x04;
                bool hasRelativePath = (flags & 0x08) == 0x08;
                bool hasWorkingDir = (flags & 0x10) == 0x10;
                bool hasArguments = (flags & 0x20) == 0x20;
                bool hasLocation = (flags & 0x40) == 0x40;

                br.ReadBytes((int)headerSize - 24);
                if (!hasiconLocation) return ServiceResponse<string>.Fail("has no associated description", true);

                if (hasLinkTargetIDList)
                {
                    ushort idListSize = br.ReadUInt16();
                    br.ReadBytes(idListSize);
                }

                if (hasLinkInfo)
                {
                    uint linkInfoSize = br.ReadUInt32();
                    br.ReadBytes((int)linkInfoSize - 4);
                }

                // parse String data field

                // NAME_STRING
                if (hasName)
                {
                    ushort name_string_count = br.ReadUInt16();
                    byte[] buffer = br.ReadBytes(isUnicode ? name_string_count * 2 : name_string_count);

                    string description = isUnicode
                        ? Encoding.Unicode.GetString(buffer)
                        : Encoding.Default.GetString(buffer);

                    return ServiceResponse<string>.Ok(description);
                }

                return ServiceResponse<string>.Fail("has no associated description", true);
            }
        }



        // change target icon path of lnk file 
        public ServiceResponse<bool> ChangeLnkIconPath(string lnkPath, string iconPath)
        {
            try
            {
                if (!System.IO.File.Exists(lnkPath))
                    return ServiceResponse<bool>.Fail($"Shortcut not found: {lnkPath}", true);

                if (!System.IO.File.Exists(iconPath))
                    return ServiceResponse<bool>.Fail($"Icon file not found: {iconPath}", true);

                WshShell wshShell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)wshShell.CreateShortcut(lnkPath);

                shortcut.IconLocation = iconPath;
                shortcut.Save();

                SHChangeNotify(0x8000000, 0x1000, IntPtr.Zero, IntPtr.Zero);

                return ServiceResponse<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ServiceResponse<bool>.Fail($"Error changing icon: {ex.Message}", false);
            }
        }

        // change the description of lnk file

        public ServiceResponse<bool> ChangeLnkDescription(string lnkPath, string description)
        {
            try
            {
                if (!System.IO.File.Exists(lnkPath))
                    return ServiceResponse<bool>.Fail($"Shortcut not found: {lnkPath}", true);

                WshShell wshShell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)wshShell.CreateShortcut(lnkPath);

                shortcut.Description = description;
                shortcut.Save();

                SHChangeNotify(0x8000000, 0x1000, IntPtr.Zero, IntPtr.Zero);

                return ServiceResponse<bool>.Ok(true);

            }
            catch (Exception ex)
            {
                return ServiceResponse<bool>.Fail($"Error changing description: {ex.Message}", false);
            }
        }

        [DllImport("Shell32.dll")]
        static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

    }
}
