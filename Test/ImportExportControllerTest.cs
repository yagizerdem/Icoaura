using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    [TestClass]
    public class ImportExportControllerTest : BaseTest
    {
        public ImportExportControllerTest()
        {
        }
        
        [TestMethod]
        public void Export()
        {
            var resposne = ImportExportController.ExportPack("e533151f-7316-49d3-a8ee-88d15b4f1750");
            Assert.IsTrue(resposne.Success);
        }

        [TestMethod]
        public void Import()
        {
            var resposne = ImportExportController.ImportPack("C:\\Users\\yagiz\\Downloads\\52f8cf8f-e932-4ad3-988e-051507f9ff90.icr");
            Assert.IsTrue(resposne.Success);
        }

    }

}
