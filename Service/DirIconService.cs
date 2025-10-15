using Model.ResponseTypes;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Service
{
    public class DirIconService
    {
        public DirIconService()
        {

        }


        public ServiceResponse<string> ChangeIconOfDir(string dirAbsolutePath, string iconPath)
        {
            try
            {
                if (!Directory.Exists(dirAbsolutePath))
                    return ServiceResponse<string>.Fail("Directory does not exist", true);

                if (!File.Exists(iconPath))
                    return ServiceResponse<string>.Fail("Icon file does not exist", true);

                string desktopIniPath = Path.Combine(dirAbsolutePath, "desktop.ini");

                if (File.Exists(desktopIniPath))
                {
                    File.SetAttributes(desktopIniPath, FileAttributes.Normal);
                    File.Delete(desktopIniPath);
                }

                string[] lines =
                {
                "[.ShellClassInfo]",
                $"IconResource={iconPath},0"
            };
                File.WriteAllLines(desktopIniPath, lines, Encoding.Unicode);

                File.SetAttributes(desktopIniPath, FileAttributes.Hidden | FileAttributes.System);

                DirectoryInfo dirInfo = new DirectoryInfo(dirAbsolutePath);
                dirInfo.Attributes |= FileAttributes.System;

                dirInfo.Attributes |= FileAttributes.ReadOnly;

                NotifyIconChange();

                return ServiceResponse<string>.OkMessage("Directory icon changed successfully");
            }
            catch (Exception ex)
            {
                return ServiceResponse<string>.Fail($"Exception: {ex.Message}", true);
            }
        }

        private void NotifyIconChange()
        {
            try
            {
                SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[NotifyIconChange] Exception: {ex.Message}");
            }
        }

        [DllImport("shell32.dll")]
        private static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

        private const uint SHCNE_ASSOCCHANGED = 0x08000000;
        private const uint SHCNF_IDLIST = 0x0000;

    }
}
