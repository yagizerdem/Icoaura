using Icoaura.Model;
using Icoaura.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Test
{
    [TestClass]
    public class PackTest : BaseTest
    {
        
        [TestMethod]
        public void CreatePack()
        {
            PackConfig config = PackConfig.GetDefaultConfig();
            var response = packController.CreatePack(config);
            Assert.IsTrue(response.Success);
        }

        [TestMethod]
        public void CreatePackWithCover()
        {
            var fsResponse =fileController.GetBase64(@"C:\Users\yagiz\Pictures\Screenshots\Ekran görüntüsü 2024-06-25 174053 - Kopya.png");
            Assert.IsTrue(fsResponse.Success);  
            var base64 = fsResponse.Data;
            PackConfig config = PackConfig.GetDefaultConfig();
            config.CoverPngBase64 = base64;
            var response = packController.CreatePack(config);
            Assert.IsTrue(response.Success);
        }

        [TestMethod]
        public void GetPackConfig()
        {
            string packId = "16452b1a-a32f-4b5a-9b11-6078d78e7a38";
            var response = packController.GetPackConfig(packId);
            Assert.IsTrue(response.Success);

        }


        [TestMethod]
        public void WritePackConfig()
        {
            PackConfig config = new()
            {
                Uid = "16452b1a-a32f-4b5a-9b11-6078d78e7a38",
                Author = "Test Author",
            };

            var response = packController.WritePackConfig(config);
            Assert.IsTrue(response.Success);
        }

        [TestMethod]
        public void AppendPackItemFromPath()
        {
            string itemPath = Path.Combine(PathUtil.Resolve("%DESKTOP_COMMON%"), "Avast SecureLine VPN.lnk");
            var response = packController.AppendPackItemFromPath("48a939a4-b4f7-4689-a90b-5d7423131d34", itemPath);

            Assert.IsTrue(response.Success);
        }


        [TestMethod]
        public void WritePackItems()
        {
            List<PackItem> list = new()
{
    new PackItem
    {
        Uid = Guid.NewGuid().ToString(),
        TargetPath = @"C:\Users\yagiz\Desktop\NotepadShortcut.lnk",
        Name = "Notepad Shortcut",
        TargetExePath = @"C:\Windows\System32\notepad.exe",
        Description = "Opens Notepad for editing text files."
    },
    new PackItem
    {
        Uid = Guid.NewGuid().ToString(),
        TargetPath = @"C:\Users\yagiz\Desktop\IcoauraProject",
        Name = "IcoauraProject",
        Description = "Main project folder containing source files."
    },
    new PackItem
    {
        Uid = Guid.NewGuid().ToString(),
        TargetPath = @"C:\Users\yagiz\Desktop\DocsShortcut.url",
        Name = "DocsShortcut",
        TargetUrl = "https://docs.microsoft.com",
        Description = "Shortcut to Microsoft Docs."
    }
};

            var dummyBase64 = fileController.GetBase64(@"C:\Users\yagiz\Pictures\Screenshots\Ekran görüntüsü 2024-06-25 174053 - Kopya.png").Data;

            var resposne = packController.WritePackItems("48a939a4-b4f7-4689-a90b-5d7423131d34", list, new()
            {
    { list[0].Uid, dummyBase64 },
    { list[1].Uid, dummyBase64 },
    { list[2].Uid, dummyBase64 }
        });

        }

        [TestMethod]
        public void AddDesktopIcons()
        {
            string packId = "a495e304-f928-4b1d-bd63-3cd5275cf042";
            var response = packController.AddDesktopIcons(packId);
            Assert.IsTrue(response.Success);
        }

        [TestMethod]
        public void ApplyPackOperations()
        {
            string packId = "48a939a4-b4f7-4689-a90b-5d7423131d34";
            var response = packController.ApplyPackOperations(packId);
            Assert.IsTrue(response.Success);
        }


        [TestMethod]
        public void GetAllPackConfigs()
        {
            var response = packController.GetAllPackConfigs();
            Assert.IsTrue(response.Success);
        
        }   


    }
}
