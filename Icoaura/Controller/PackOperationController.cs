using Icoaura.Enum;
using Icoaura.Exception;
using Icoaura.Model;
using Icoaura.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Icoaura.Controller
{
    public class PackOperationController : BaseController
    {

        public PackOperationController()
        {
            
        }

        public ApiResponse<object> ChangeLnkIconPath(string lnkPath, string iconPath)
        {
            return ExecuteSafe(() =>
            {
                // --- Validation ---
                FileUtil.EnsureFileExist(lnkPath, LogLevel.Error);
                FileUtil.EnsureFileHasExtension(lnkPath, [".lnk", "lnk"] );
                FileUtil.EnsureFileExist(iconPath, LogLevel.Error);

                try
                {
                    var wshShell = new IWshRuntimeLibrary.WshShell();
                    var shortcut = (IWshRuntimeLibrary.IWshShortcut)wshShell.CreateShortcut(lnkPath);

                    shortcut.IconLocation = iconPath;
                    shortcut.Save();


                    return new object();
                }
                catch (UnauthorizedAccessException ex)
                {
                    throw AppException.Operational(
                        userMessage: this._l10nService.GetLocalizedMessage("Errors.UnauthorizedAccess"),
                        logMessage: $"UnauthorizedAccessException while modifying '{lnkPath}': {ex.Message}",
                        level: LogLevel.Error
                    );

                }
                catch (COMException ex)
                {
                    throw AppException.Operational(
                        userMessage: this._l10nService.GetLocalizedMessage("Errors.IconUpdateFailed"),
                        logMessage: $"COMException during IWshShortcut operation for '{lnkPath}': {ex.Message}",
                        level: LogLevel.Error
                    );

                }
                catch (System.Exception ex)
                {
                    throw AppException.Operational(
                        userMessage: this._l10nService.GetLocalizedMessage("Errors.IconUpdateFailed"),
                        logMessage: $"Exception while updating '{lnkPath}': {ex.Message}",
                        level: LogLevel.Error
                    );
                }
            });
        }

        public ApiResponse<object> ChangeLnkDescription(string lnkPath, string description)
        {
            return ExecuteSafe(() =>
            {
                // --- Validation ---
                FileUtil.EnsureFileExist(lnkPath, LogLevel.Error);
                FileUtil.EnsureFileHasExtension(lnkPath, new[] { ".lnk" });

                if (string.IsNullOrWhiteSpace(description))
                {
                    throw AppException.Operational(
                        userMessage: this._l10nService.GetLocalizedMessage("Errors.DescriptionEmpty"),
                        logMessage: $"ChangeLnkDescription failed: provided description is null or whitespace. [Input: '{description ?? "null"}']",
                        level: LogLevel.Warning
                    );
                }

                try
                {
                    var wshShell = new IWshRuntimeLibrary.WshShell();
                    var shortcut = (IWshRuntimeLibrary.IWshShortcut)wshShell.CreateShortcut(lnkPath);

                    shortcut.Description = description;
                    shortcut.Save();


                    return new object();
                }
                catch (UnauthorizedAccessException ex)
                {
                    throw AppException.Operational(
                        userMessage: this._l10nService.GetLocalizedMessage("Errors.UnauthorizedAccess"),
                        logMessage: $"UnauthorizedAccessException while modifying '{lnkPath}': {ex.Message}",
                        level: LogLevel.Error
                    );
                }
                catch (COMException ex)
                {
                    throw AppException.Operational(
                        userMessage: this._l10nService.GetLocalizedMessage("Errors.IconUpdateFailed"),
                        logMessage: $"COMException during IWshShortcut operation for '{lnkPath}': {ex.Message}",
                        level: LogLevel.Error
                    );
                }
                catch (System.Exception ex)
                {
                    throw AppException.Operational(
                        userMessage: this._l10nService.GetLocalizedMessage("Errors.UnexpectedOperational"),
                        logMessage: $"Unexpected exception while updating '{lnkPath}': {ex.Message}",
                        level: LogLevel.Error
                    );
                }
            });
        }

        public ApiResponse<string> ChangeUrlOfUrlFile(string urlAbsolutePath, string newUrl)
        {
            return ExecuteSafe(() =>
            {
                // --- Validation ---
                FileUtil.EnsureFileExist(urlAbsolutePath, LogLevel.Error);
                FileUtil.EnsureFileHasExtension(urlAbsolutePath, new[] { ".url" });

                if (string.IsNullOrWhiteSpace(newUrl))
                {
                    throw AppException.Operational(
                        userMessage: this._l10nService.GetLocalizedMessage("Errors.UrlFileLoadFailed"),
                        logMessage: $"ChangeUrlOfUrlFile failed: new URL is null or whitespace. [Input: '{newUrl ?? "null"}']",
                        level: LogLevel.Warning
                    );
                }

                try
                {
                    string[] lines = File.ReadAllLines(urlAbsolutePath);
                    bool urlFound = false;

                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (lines[i].StartsWith("URL=", StringComparison.OrdinalIgnoreCase))
                        {
                            lines[i] = "URL=" + newUrl;
                            urlFound = true;
                            break;
                        }
                    }

                    if (!urlFound)
                    {
                        throw AppException.Operational(
                            userMessage: this._l10nService.GetLocalizedMessage("Errors.UrlLineNotFound"),
                            logMessage: $"ChangeUrlOfUrlFile failed: 'URL=' line missing in '{urlAbsolutePath}'.",
                            level: LogLevel.Error
                        );
                    }

                    File.WriteAllLines(urlAbsolutePath, lines);
                    return "URL updated successfully.";
                }
                catch (IOException ex)
                {
                    throw AppException.Operational(
                        userMessage: this._l10nService.GetLocalizedMessage("Errors.IOException"),
                        logMessage: $"IOException while modifying '{urlAbsolutePath}': {ex.Message}",
                        level: LogLevel.Error
                    );
                }
                catch (UnauthorizedAccessException ex)
                {
                    throw AppException.Operational(
                        userMessage: this._l10nService.GetLocalizedMessage("Errors.UnauthorizedAccess"),
                        logMessage: $"UnauthorizedAccessException while modifying '{urlAbsolutePath}': {ex.Message}",
                        level: LogLevel.Error
                    );
                }
            });
        }


        public ApiResponse<object> ChangeIconPathOfUrlFile(string urlAbsolutePath, string newIconFilePath)
        {
            return ExecuteSafe(() =>
            {
                // --- Validation ---
                FileUtil.EnsureFileExist(urlAbsolutePath, LogLevel.Error);
                FileUtil.EnsureFileHasExtension(urlAbsolutePath, new[] { ".url" });
                FileUtil.EnsureFileExist(newIconFilePath, LogLevel.Error);

                try
                {
                    string[] lines = File.ReadAllLines(urlAbsolutePath);
                    bool iconFileFound = false;

                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (lines[i].StartsWith("IconFile=", StringComparison.OrdinalIgnoreCase))
                        {
                            lines[i] = "IconFile=" + newIconFilePath;
                            iconFileFound = true;
                            break;
                        }
                    }

                    if (!iconFileFound)
                    {
                        var list = lines.ToList();
                        list.Add("IconFile=" + newIconFilePath);
                        lines = list.ToArray();
                    }

                    File.WriteAllLines(urlAbsolutePath, lines);
                    return new object();
                }
                catch (IOException ex)
                {
                    throw AppException.Operational(
                        userMessage: this._l10nService.GetLocalizedMessage("Errros.IOException"),
                        logMessage: $"IOException while modifying '{urlAbsolutePath}': {ex.Message}",
                        level: LogLevel.Error
                    );
                }
                catch (UnauthorizedAccessException ex)
                {
                    throw AppException.Operational(
                        userMessage: this._l10nService.GetLocalizedMessage("Errors.UnauthorizedAccess"),
                        logMessage: $"UnauthorizedAccessException while modifying '{urlAbsolutePath}': {ex.Message}",
                        level: LogLevel.Error
                    );
                }
            });
        }

        public ApiResponse<object> ChangeDirectoryIcon(string dirPath, string iconPath)
        {
            return ExecuteSafe(() =>
            {
                FileUtil.EnsureDirectoryExist(dirPath, LogLevel.Error);
                FileUtil.EnsureFileExist(iconPath, LogLevel.Error);

                string desktopIniPath = Path.Combine(dirPath, "desktop.ini");

                try
                {
                    var dirInfo = new DirectoryInfo(dirPath);
                    //dirInfo.Attributes &= ~(FileAttributes.System | FileAttributes.ReadOnly | FileAttributes.Hidden);

                    if (File.Exists(desktopIniPath))
                    {
                        File.SetAttributes(desktopIniPath, FileAttributes.Normal);
                    }

                    string[] lines =
                    {
                "[.ShellClassInfo]",
                $"IconResource={iconPath},0",
                $"IconFile={iconPath}",
                "IconIndex=0"
            };

                    File.WriteAllLines(desktopIniPath, lines, Encoding.Unicode);

                    File.SetAttributes(desktopIniPath, FileAttributes.Hidden | FileAttributes.System);

                    dirInfo.Attributes |= FileAttributes.System | FileAttributes.ReadOnly;


                    RefreshFolderIcon(dirPath);

                    return new object();
                }
                catch (UnauthorizedAccessException ex)
                {
                    throw AppException.Operational(
                        userMessage: this._l10nService.GetLocalizedMessage("Errors.UnauthorizedAccess"),
                        logMessage: $"UnauthorizedAccessException while modifying folder '{dirPath}': {ex.Message}",
                        level: LogLevel.Error
                    );
                }
                catch (IOException ex)
                {
                    throw AppException.Operational(
                        userMessage: this._l10nService.GetLocalizedMessage("Errors.IOException"),
                        logMessage: $"IOException while updating '{dirPath}\\desktop.ini': {ex.Message}",
                        level: LogLevel.Error
                    );
                }
            });
        }

        [DllImport("Shell32.dll")]
        static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);
    
        public void RefreshWindowsShell()
        {
            SHChangeNotify(0x8000000, 0x1000, IntPtr.Zero, IntPtr.Zero);
        }

        public static void RefreshFolderIcon(string folderPath)
        {
            IntPtr pathPtr = Marshal.StringToHGlobalUni(folderPath);
            SHChangeNotify(0x00002000, 0x0000, pathPtr, IntPtr.Zero);
            Marshal.FreeHGlobal(pathPtr);
        }

        public void ForceExplorerIconRefresh()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string explorerDir = Path.Combine(localAppData, "Microsoft", "Windows", "Explorer");

            foreach (string file in Directory.GetFiles(explorerDir, "iconcache*"))
            {
                try { File.Delete(file); } catch { /* ignore */ }
            }

            foreach (var process in Process.GetProcessesByName("explorer"))
            {
                process.Kill();
                process.WaitForExit();
            }

            Process.Start("explorer.exe");
        }


    }
}
