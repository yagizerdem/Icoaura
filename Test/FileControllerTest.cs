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


        [TestMethod]
        public void GetDitMetaData()
        {
            string resolved = PathUtil.Resolve(Path.Combine("%DESKTOP%", "gitlearn"));

            ApiResponse<DirMetaData> response = fileController.GetDirMetaData(resolved);
            Assert.IsTrue(response.Success);

            DirMetaData metaData = response.Data!;
        }


        [TestMethod]
        public void GetFilesUnderPath()
        {
            string resolved = PathUtil.Resolve(Path.Combine("%DESKTOP%"));
            ApiResponse<List<string>> response = fileController.GetFilesUnderPath(resolved, ["*"],1);
            Assert.IsTrue(response.Success);
            List<string> files = response.Data!;
        }

        [TestMethod]
        public void GetFoldersUnderPath()
        {
            string resolved = PathUtil.Resolve(Path.Combine("%DESKTOP%"));
            ApiResponse<List<string>> response = fileController.GetFoldersUnderPath(resolved, 1);
            Assert.IsTrue(response.Success);
            List<string> folders = response.Data!;
        }

        [TestMethod]
        public void GetMatchingParts()
        {
            string resolved = PathUtil.Resolve(Path.Combine("%DESKTOP%"));
            ApiResponse<List<string>> response = fileController.GetMatchingFileSystemEntries(resolved , "ahmet");
            Assert.IsTrue(response.Success);
            List<string> parts = response.Data!;
        }



        [TestMethod]
        public void GetMatchingFileSystemEntries()
        {
            string resolved = PathUtil.Resolve(Path.Combine("%LOCALAPPDATA%"));
            ApiResponse<List<string>> response = fileController.GetMatchingFileSystemEntries(resolved, "*\\*\\*\\LTspice.exe");
            Assert.IsTrue(response.Success);
            List<string> entries = response.Data!;
        }
    }
}
