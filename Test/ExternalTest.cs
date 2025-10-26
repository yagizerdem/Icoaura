using Icoaura.Context;
using Icoaura.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    [TestClass]
    public class ExternalTest : BaseTest
    {
        [TestMethod]
        public void GetExIcon()
        {
            string exepath = @"C:\Program Files (x86)\EasyAntiCheat\EasyAntiCheat.exe";
            ApiResponse<string> response = externalController.GetExeIconAsPngBase64(exepath);
            Assert.IsTrue(response.Success);

            string base64 = response.Data;

            string tempPath = Path.Combine(ApplicationPathContext.AppTempFolderPath, "test.png");
            fileController.WriteBase64(tempPath, base64, overwrite: true);

        }

        [TestMethod]
        public void IcoToPngBase64()
        {
            string icoPath = @"C:\Users\yagiz\Desktop\testIcons\writer.ico";
            ApiResponse<string> response = externalController.IcoToPngBase64(icoPath, 256, 256);
            Assert.IsTrue(response.Success);
            string base64 = response.Data;
            string tempPath = Path.Combine(ApplicationPathContext.AppTempFolderPath, "icon_test.png");
            fileController.WriteBase64(tempPath, base64, overwrite: true);
        }

        [TestMethod]
        public void PngToIcoBase64()
        {
            string pngPath = @"C:\Users\yagiz\Pictures\Screenshots\Ekran görüntüsü 2024-06-25 174053 - Kopya.png";
            ApiResponse<string> response = externalController.PngToIcoBase64(pngPath, 256);
            Assert.IsTrue(response.Success);
            string base64 = response.Data;
            string tempPath = Path.Combine(ApplicationPathContext.AppTempFolderPath, "icon_test.ico");
            fileController.WriteBase64(tempPath, base64, overwrite: true);
        }

        [TestMethod]
        public void IcoBas64ToPngBase64()
        {
            string icoPath = @"C:\Users\yagiz\Desktop\testIcons\writer.ico";
            string icoBase64 = fileController.GetBase64(icoPath).Data;
            ApiResponse<string> response = externalController.IcoBase64ToPngBase64(icoBase64);
            Assert.IsTrue(response.Success);
            string base64 = response.Data;
            string tempPath = Path.Combine(ApplicationPathContext.AppTempFolderPath, "icon_test2.png");
            fileController.WriteBase64(tempPath, base64, overwrite: true);
        }


        [TestMethod]
        public void PngBase64ToIcoBase64()
        {
            string pngPath = @"C:\Users\yagiz\Pictures\Screenshots\Ekran görüntüsü 2024-06-25 174053 - Kopya.png";
            string pngBase64 = fileController.GetBase64(pngPath).Data;
            ApiResponse<string> response = externalController.PngBase64ToIcoBase64(pngBase64);
            Assert.IsTrue(response.Success);
            string base64 = response.Data;
            string tempPath = Path.Combine(ApplicationPathContext.AppTempFolderPath, "icon_test2.ico");
            fileController.WriteBase64(tempPath, base64, overwrite: true);
        }


        [TestMethod]
        public void ResizePngBase64()
        {
            string pngPath = @"C:\Users\yagiz\Pictures\Screenshots\Ekran görüntüsü 2024-06-25 174053 - Kopya.png";
            string pngBase64 = fileController.GetBase64(pngPath).Data;
            ApiResponse<string> response = externalController.ResizePngBase64(pngBase64, 128, 128);
            Assert.IsTrue(response.Success);
            string base64 = response.Data;
            string tempPath = Path.Combine(ApplicationPathContext.AppTempFolderPath, "icon_test_resized.png");
            fileController.WriteBase64(tempPath, base64, overwrite: true);
        }


        [TestMethod]
        public void ApplyOpacityOnPngBase64()
        {
            string pngPath = @"C:\Users\yagiz\Pictures\Screenshots\Ekran görüntüsü 2024-06-25 174053 - Kopya.png";
            string pngBase64 = fileController.GetBase64(pngPath).Data;
            ApiResponse<string> response = externalController.ApplyOpacityOnPngBase64(pngBase64, 0.5f);
            Assert.IsTrue(response.Success);
            string base64 = response.Data;
            string tempPath = Path.Combine(ApplicationPathContext.AppTempFolderPath, "icon_test_opacity.png");
            fileController.WriteBase64(tempPath, base64, overwrite: true);
        }

        [TestMethod]
        public void ApplyCornerRadiusOnPngBase64()
        {
            string pngPath = @"C:\Users\yagiz\Pictures\Screenshots\Ekran görüntüsü 2024-06-25 174053 - Kopya.png";
            string pngBase64 = fileController.GetBase64(pngPath).Data;
            ApiResponse<string> response = externalController.ApplyCornerRadiusOnPngBase64(pngBase64, 0.3f);
            Assert.IsTrue(response.Success);
            string base64 = response.Data;
            string tempPath = Path.Combine(ApplicationPathContext.AppTempFolderPath, "icon_test_cornerradius.png");
            fileController.WriteBase64(tempPath, base64, overwrite: true);
        }

        [TestMethod]
        public void CombinedTest()
        {
            string pngPath = @"C:\Users\yagiz\Pictures\Screenshots\Ekran görüntüsü 2024-06-25 174053 - Kopya.png";
            string pngBase64 = fileController.GetBase64(pngPath).Data;
            ApiResponse<string> response = externalController.ResizePngBase64(pngBase64, 128, 128);
            Assert.IsTrue(response.Success);
            string base64 = response.Data;
            response = externalController.ApplyCornerRadiusOnPngBase64(base64, 0.5f);
            Assert.IsTrue(response.Success);
            base64 = response.Data;
            response = externalController.ApplyOpacityOnPngBase64(base64, 0.3f);
            Assert.IsTrue(response.Success);
            base64 = response.Data;
            string tempPath = Path.Combine(ApplicationPathContext.AppTempFolderPath, "icon_test_combined.png");
            fileController.WriteBase64(tempPath, base64, overwrite: true);
        }

    }
}
