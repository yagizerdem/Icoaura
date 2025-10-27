using Icoaura.Bridge;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
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



        private async void InitializeAsync()
        {


            await webView.EnsureCoreWebView2Async();

#if DEBUG
            webView.CoreWebView2.Navigate("http://localhost:5173/");

#else
    // load from local file system
#endif

            webView.CoreWebView2.AddHostObjectToScript("windowBridge", new WindowBridge(this));
            webView.CoreWebView2.AddHostObjectToScript("packBridge", new PackBridge());


        }




    }
}