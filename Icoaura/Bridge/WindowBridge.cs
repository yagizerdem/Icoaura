using Icoaura.Enum;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows;
using System.Windows.Interop;

namespace Icoaura.Bridge
{
    [System.Runtime.InteropServices.ClassInterface(System.Runtime.InteropServices.ClassInterfaceType.AutoDual)]
    [System.Runtime.InteropServices.ComVisible(true)]
    public class WindowBridge
    {
        private readonly Window _window;

        public WindowBridge(Window window)
        {
            _window = window;
        }

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;


        public void StartNativeDrag()
        {
            _window.Dispatcher.Invoke(() =>
            {
                var hwnd = new WindowInteropHelper(_window).Handle;
                _window.Activate(); // ensure focus

                // Break any existing capture (e.g. WebView2)
                ReleaseCapture();

                // This tells Windows: “user started dragging the title bar”
                SendMessage(hwnd, WM_NCLBUTTONDOWN, new IntPtr(HTCAPTION), IntPtr.Zero);
            });
        }
    

        // Optional: minimize / maximize / close
        public void Minimize() => Application.Current.Dispatcher.Invoke(() => _window.WindowState = WindowState.Minimized);
        public void Maximize() => Application.Current.Dispatcher.Invoke(() => _window.WindowState = WindowState.Maximized);
        public void Close() => Application.Current.Dispatcher.Invoke(() => _window.Close());

        public void RebootAsAdmin()
        {
            try
            {
                string exePath = Process.GetCurrentProcess().MainModule?.FileName
                                 ?? throw new InvalidOperationException("Cannot determine executable path.");

                bool isAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent())
                    .IsInRole(WindowsBuiltInRole.Administrator);

                if (isAdmin)
                {
                    // already running as admin
                    return;
                }

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    UseShellExecute = true,
                    Verb = "runas", // This triggers the UAC prompt
                    WorkingDirectory = Directory.GetCurrentDirectory(),
                    Arguments = Environment.CommandLine.Replace("\"" + exePath + "\"", "")
                };

                Process.Start(startInfo);

                Application.Current.Shutdown();
            }
            catch (System.Exception ex)
            {
                // silently skip exceptions
            }
        }

        public bool HasAdminPrivilege()
        {
            try
            {
                using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
                {
                    WindowsPrincipal principal = new WindowsPrincipal(identity);
                    return principal.IsInRole(WindowsBuiltInRole.Administrator);
                }
            }
            catch
            {
                return false;
            }
        }

        public int GetSystemDefaultTheme()
        {
            const string registryKey = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
            const string valueName = "AppsUseLightTheme";

            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(registryKey))
                {
                    if (key != null)
                    {
                        object? value = key.GetValue(valueName);
                        if (value is int intValue)
                        {
                            return intValue == 0 ? ((int)Theme.Dark) : ((int)Theme.Light);
                        }
                    }
                }
            }
            catch
            {
            }

            return ((int)Theme.Light);
        }

    }
}
