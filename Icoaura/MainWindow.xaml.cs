using Icoaura.Bridge;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Icoaura
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            InitializeAsync();
        }


        private async Task InitializeAsync()
        {


            await webView.EnsureCoreWebView2Async();

#if DEBUG
            webView.CoreWebView2.Navigate("http://localhost:5173/");


#else
            // Load from local file system
            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            string distFolder = System.IO.Path.Combine(exeDir, "reactDist"); // örn: bin/Release/net9.0-windows/reactDist
            string indexPath = System.IO.Path.Combine(distFolder, "index.html");

            if (System.IO.File.Exists(indexPath))
            {
                // reactDist klasörünü sanal domain'e map et
                webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                    "app.local",
                    distFolder,
                    Microsoft.Web.WebView2.Core.CoreWebView2HostResourceAccessKind.Allow
                );

                // file:/// yerine HTTPS ile sanal domain üzerinden yükle
                webView.CoreWebView2.Navigate("https://app.local/index.html");
            }
            else
            {
                throw new FileNotFoundException("index.html not found in reactDist folder.", indexPath);
            }
#endif

            webView.CoreWebView2.AddHostObjectToScript("windowBridge", new WindowBridge(this));
            webView.CoreWebView2.AddHostObjectToScript("packBridge", new PackBridge());
            webView.CoreWebView2.AddHostObjectToScript("fileBridge", new FileBridge());
            webView.CoreWebView2.AddHostObjectToScript("appConfigBridge", new AppConfigBridge());


        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            webView.Width = ActualWidth;
            webView.Height = ActualHeight;
        }


    }
}