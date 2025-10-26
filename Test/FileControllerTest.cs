using Icoaura.Model;
using Icoaura.Util;
using Model.DTO;

namespace Test
{
    [TestClass]
    public sealed class FileControllerTest : BaseTest
    {
        [TestMethod]
        public void LnkMetaData()
        {
            string resolved = PathUtil.Resolve(Path.Combine("%DESKTOP%", "Visual Studio Code.lnk"));

            ApiResponse<LnkMetaData>  response =  fileController.GetLnkMetaData(resolved);
            Assert.IsTrue(response.Success);

            LnkMetaData metaData = response.Data!;


        }

        [TestMethod]
        public void UrlMetaData()
        {
            string resolved = PathUtil.Resolve(Path.Combine("%DESKTOP%", "Fallout 76.url"));

            ApiResponse<UrlMetaData> response = fileController.GetUrlMetaData(resolved);
            Assert.IsTrue(response.Success);

            UrlMetaData metaData = response.Data!;


        }


    }
}
