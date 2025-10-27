using System.Runtime.InteropServices;
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
    }
}
