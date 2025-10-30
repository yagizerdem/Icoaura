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
                this._logger.Log("Entering ChangeLnkIconPath()", TraceContext.TraceId, LogLevel.Trace);
                this._logger.Log($"Parameters: LnkPath='{lnkPath}', IconPath='{iconPath}'", TraceContext.TraceId, LogLevel.Debug);

                // --- Validation ---
                FileUtil.EnsureFileExist(lnkPath, LogLevel.Error);
                FileUtil.EnsureFileHasExtension(lnkPath, [".lnk", "lnk"]);
                FileUtil.EnsureFileExist(iconPath, LogLevel.Error);

                try
                {
                    var wshShell = new IWshRuntimeLibrary.WshShell();
                    var shortcut = (IWshRuntimeLibrary.IWshShortcut)wshShell.CreateShortcut(lnkPath);

                    shortcut.IconLocation = iconPath;
                    shortcut.Save();

                    this._logger.Log($"Icon successfully updated for '{lnkPath}' → '{iconPath}'", TraceContext.TraceId, LogLevel.Info);
                    return new object();
                }
                catch (UnauthorizedAccessException ex)
                {
                    this._logger.Log($"UnauthorizedAccessException during ChangeLnkIconPath: {ex.Message}", TraceContext.TraceId, LogLevel.Error);
                    throw AppException.Operational(
                        userMessage: this._l10nService.GetLocalizedMessage("Errors.UnauthorizedAccess"),
                        logMessage: $"UnauthorizedAccessException while modifying '{lnkPath}': {ex.Message}",
                        level: LogLevel.Error
                    );
                }
                catch (COMException ex)
                {
                    this._logger.Log($"COMException during IWshShortcut operation: {ex.Message}", TraceContext.TraceId, LogLevel.Error);
                    throw AppException.Operational(
                        userMessage: this._l10nService.GetLocalizedMessage("Errors.IconUpdateFailed"),
                        logMessage: $"COMException during IWshShortcut operation for '{lnkPath}': {ex.Message}",
                        level: LogLevel.Error
                    );
                }
                catch (System.Exception ex)
                {
                    this._logger.Log($"Unexpected exception during ChangeLnkIconPath: {ex.Message}", TraceContext.TraceId, LogLevel.Error);
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
                this._logger.Log("Entering ChangeLnkDescription()", TraceContext.TraceId, LogLevel.Trace);
                this._logger.Log($"Parameters: LnkPath='{lnkPath}', Description='{description}'", TraceContext.TraceId, LogLevel.Debug);

                // --- Validation ---
                FileUtil.EnsureFileExist(lnkPath, LogLevel.Error);
                FileUtil.EnsureFileHasExtension(lnkPath, new[] { ".lnk" });

                if (string.IsNullOrWhiteSpace(description))
                {
                    this._logger.Log("Empty or null description provided.", TraceContext.TraceId, LogLevel.Warning);
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

                    this._logger.Log($"Description successfully updated for '{lnkPath}'", TraceContext.TraceId, LogLevel.Info);
                    return new object();
                }
                catch (UnauthorizedAccessException ex)
                {
                    this._logger.Log($"UnauthorizedAccessException while updating description: {ex.Message}", TraceContext.TraceId, LogLevel.Error);
                    throw AppException.Operational(
                        userMessage: this._l10nService.GetLocalizedMessage("Errors.UnauthorizedAccess"),
                        logMessage: $"UnauthorizedAccessException while modifying '{lnkPath}': {ex.Message}",
                        level: LogLevel.Error
                    );
                }
                catch (COMException ex)
                {
                    this._logger.Log($"COMException during IWshShortcut operation: {ex.Message}", TraceContext.TraceId, LogLevel.Error);
                    throw AppException.Operational(
                        userMessage: this._l10nService.GetLocalizedMessage("Errors.IconUpdateFailed"),
                        logMessage: $"COMException during IWshShortcut operation for '{lnkPath}': {ex.Message}",
                        level: LogLevel.Error
                    );
                }
                catch (System.Exception ex)
                {
                    this._logger.Log($"Unexpected exception while updating description: {ex.Message}", TraceContext.TraceId, LogLevel.Error);
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
                this._logger.Log("Entering ChangeUrlOfUrlFile()", TraceContext.TraceId, LogLevel.Trace);
                this._logger.Log($"Parameters: UrlPath='{urlAbsolutePath}', NewUrl='{newUrl}'", TraceContext.TraceId, LogLevel.Debug);

                // --- Validation ---
                FileUtil.EnsureFileExist(urlAbsolutePath, LogLevel.Error);
                FileUtil.EnsureFileHasExtension(urlAbsolutePath, new[] { ".url" });

                if (string.IsNullOrWhiteSpace(newUrl))
                {
                    this._logger.Log("New URL is empty or null.", TraceContext.TraceId, LogLevel.Warning);
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
                            this._logger.Log($"Replaced URL line in '{urlAbsolutePath}'", TraceContext.TraceId, LogLevel.Debug);
                            break;
                        }
                    }

                    if (!urlFound)
                    {
                        this._logger.Log($"'URL=' line missing in '{urlAbsolutePath}'", TraceContext.TraceId, LogLevel.Error);
                        throw AppException.Operational(
                            userMessage: this._l10nService.GetLocalizedMessage("Errors.UrlLineNotFound"),
                            logMessage: $"ChangeUrlOfUrlFile failed: 'URL=' line missing in '{urlAbsolutePath}'.",
                            level: LogLevel.Error
                        );
                    }

                    File.WriteAllLines(urlAbsolutePath, lines);
                    this._logger.Log($"URL updated successfully in '{urlAbsolutePath}'", TraceContext.TraceId, LogLevel.Info);
                    return "URL updated successfully.";
                }
                catch (IOException ex)
                {
                    this._logger.Log($"IOException during ChangeUrlOfUrlFile: {ex.Message}", TraceContext.TraceId, LogLevel.Error);
                    throw AppException.Operational(
                        userMessage: this._l10nService.GetLocalizedMessage("Errors.IOException"),
                        logMessage: $"IOException while modifying '{urlAbsolutePath}': {ex.Message}",
                        level: LogLevel.Error
                    );
                }
                catch (UnauthorizedAccessException ex)
                {
                    this._logger.Log($"UnauthorizedAccessException while writing URL file: {ex.Message}", TraceContext.TraceId, LogLevel.Error);
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
                this._logger.Log("Entering ChangeIconPathOfUrlFile()", TraceContext.TraceId, LogLevel.Trace);
                this._logger.Log($"Parameters: UrlPath='{urlAbsolutePath}', NewIcon='{newIconFilePath}'", TraceContext.TraceId, LogLevel.Debug);

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
                            this._logger.Log($"Replaced IconFile line in '{urlAbsolutePath}'", TraceContext.TraceId, LogLevel.Debug);
                            break;
                        }
                    }

                    if (!iconFileFound)
                    {
                        var list = lines.ToList();
                        list.Add("IconFile=" + newIconFilePath);
                        lines = list.ToArray();
                        this._logger.Log($"Added missing IconFile line in '{urlAbsolutePath}'", TraceContext.TraceId, LogLevel.Warning);
                    }

                    File.WriteAllLines(urlAbsolutePath, lines);
                    this._logger.Log($"Icon path successfully updated in '{urlAbsolutePath}'", TraceContext.TraceId, LogLevel.Info);
                    return new object();
                }
                catch (IOException ex)
                {
                    this._logger.Log($"IOException during ChangeIconPathOfUrlFile: {ex.Message}", TraceContext.TraceId, LogLevel.Error);
                    throw AppException.Operational(
                        userMessage: this._l10nService.GetLocalizedMessage("Errors.IOException"),
                        logMessage: $"IOException while modifying '{urlAbsolutePath}': {ex.Message}",
                        level: LogLevel.Error
                    );
                }
                catch (UnauthorizedAccessException ex)
                {
                    this._logger.Log($"UnauthorizedAccessException while updating icon in URL file: {ex.Message}", TraceContext.TraceId, LogLevel.Error);
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
                this._logger.Log("Entering ChangeDirectoryIcon()", TraceContext.TraceId, LogLevel.Trace);
                this._logger.Log($"Parameters: DirPath='{dirPath}', IconPath='{iconPath}'", TraceContext.TraceId, LogLevel.Debug);

                FileUtil.EnsureDirectoryExist(dirPath, LogLevel.Error);
                FileUtil.EnsureFileExist(iconPath, LogLevel.Error);

                string desktopIniPath = Path.Combine(dirPath, "desktop.ini");
                this._logger.Log($"desktop.ini path resolved: {desktopIniPath}", TraceContext.TraceId, LogLevel.Debug);

                try
                {
                    var dirInfo = new DirectoryInfo(dirPath);

                    if (File.Exists(desktopIniPath))
                    {
                        File.SetAttributes(desktopIniPath, FileAttributes.Normal);
                        this._logger.Log($"Existing desktop.ini attributes cleared for '{dirPath}'", TraceContext.TraceId, LogLevel.Debug);
                    }

                    string[] lines =
                    {
                "[.ShellClassInfo]",
                $"IconResource={iconPath},0",
                $"IconFile={iconPath}",
                "IconIndex=0"
            };

                    File.WriteAllLines(desktopIniPath, lines, Encoding.Unicode);
                    this._logger.Log($"desktop.ini written successfully for '{dirPath}'", TraceContext.TraceId, LogLevel.Info);

                    File.SetAttributes(desktopIniPath, FileAttributes.Hidden | FileAttributes.System);
                    dirInfo.Attributes |= FileAttributes.System | FileAttributes.ReadOnly;
                    this._logger.Log($"Applied System & ReadOnly attributes to '{dirPath}'", TraceContext.TraceId, LogLevel.Trace);

                    RefreshFolderIcon(dirPath);
                    this._logger.Log($"Folder icon refreshed: '{dirPath}'", TraceContext.TraceId, LogLevel.Info);

                    return new object();
                }
                catch (UnauthorizedAccessException ex)
                {
                    this._logger.Log($"UnauthorizedAccessException during ChangeDirectoryIcon: {ex.Message}", TraceContext.TraceId, LogLevel.Error);
                    throw AppException.Operational(
                        userMessage: this._l10nService.GetLocalizedMessage("Errors.UnauthorizedAccess"),
                        logMessage: $"UnauthorizedAccessException while modifying folder '{dirPath}': {ex.Message}",
                        level: LogLevel.Error
                    );
                }
                catch (IOException ex)
                {
                    this._logger.Log($"IOException during ChangeDirectoryIcon: {ex.Message}", TraceContext.TraceId, LogLevel.Error);
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
            this._logger.Log("Entering RefreshWindowsShell()", TraceContext.TraceId, LogLevel.Trace);

            try
            {
                SHChangeNotify(0x8000000, 0x1000, IntPtr.Zero, IntPtr.Zero);
                this._logger.Log("Windows shell refreshed successfully (SHChangeNotify broadcast).", TraceContext.TraceId, LogLevel.Info);
            }
            catch (System.Exception ex)
            {
                this._logger.Log($"Exception during RefreshWindowsShell: {ex.Message}", TraceContext.TraceId, LogLevel.Error);
            }
        }

        public static void RefreshFolderIcon(string folderPath)
        {
            // Static method — no logger instance, but we can optionally log to Debug output
            try
            {
                IntPtr pathPtr = Marshal.StringToHGlobalUni(folderPath);
                SHChangeNotify(0x00002000, 0x0000, pathPtr, IntPtr.Zero);
                Marshal.FreeHGlobal(pathPtr);

                System.Diagnostics.Debug.WriteLine($"[FolderIconRefresh] Folder icon refreshed for: {folderPath}");
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FolderIconRefresh] Failed to refresh icon for '{folderPath}': {ex.Message}");
            }
        }

        public void ForceExplorerIconRefresh()
        {
            this._logger.Log("Entering ForceExplorerIconRefresh()", TraceContext.TraceId, LogLevel.Trace);
            this._logger.Log("Aggressively refreshing Explorer icons — may cause temporary lag on low-end systems.", TraceContext.TraceId, LogLevel.Warning);

            try
            {
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string explorerDir = Path.Combine(localAppData, "Microsoft", "Windows", "Explorer");

                this._logger.Log($"Explorer cache directory: '{explorerDir}'", TraceContext.TraceId, LogLevel.Debug);

                foreach (string file in Directory.GetFiles(explorerDir, "iconcache*"))
                {
                    try
                    {
                        File.Delete(file);
                        this._logger.Log($"Deleted cache file: {file}", TraceContext.TraceId, LogLevel.Trace);
                    }
                    catch (System.Exception ex)
                    {
                        this._logger.Log($"Failed to delete cache file '{file}': {ex.Message}", TraceContext.TraceId, LogLevel.Warning);
                    }
                }

                foreach (var process in Process.GetProcessesByName("explorer"))
                {
                    try
                    {
                        this._logger.Log($"Terminating explorer.exe (PID={process.Id})", TraceContext.TraceId, LogLevel.Debug);
                        process.Kill();
                        process.WaitForExit();
                    }
                    catch (System.Exception ex)
                    {
                        this._logger.Log($"Failed to terminate explorer.exe PID={process.Id}: {ex.Message}", TraceContext.TraceId, LogLevel.Warning);
                    }
                }

                Process.Start("explorer.exe");
                this._logger.Log("Explorer restarted successfully after icon cache refresh.", TraceContext.TraceId, LogLevel.Info);
            }
            catch (System.Exception ex)
            {
                this._logger.Log($"Exception during ForceExplorerIconRefresh: {ex.Message}", TraceContext.TraceId, LogLevel.Error);
                throw AppException.Operational(
                    userMessage: this._l10nService.GetLocalizedMessage("Errors.IconUpdateFailed"),
                    logMessage: $"ForceExplorerIconRefresh failed: {ex.Message}",
                    level: LogLevel.Error
                );
            }
        }



    }
}
