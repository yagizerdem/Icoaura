using Icoaura.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
 


    }
}
