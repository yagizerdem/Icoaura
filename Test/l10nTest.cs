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
    public class l10nTest : BaseTest
    {

        [TestMethod]
        public void getLocalizedMessage_Test()
        {
            string trMessage = l10nService.GetLocalizedMessage("Errors.UnauthorizedAccess");
        }

        [TestMethod]
        public void ThorCreatePack()
        {
            PackConfig config = new PackConfig()
            {
                PackName = "TestPack",
                Author = "TestAuthor",
                Version = "1.0",
                Description = "This is a test pack.",
            };


            var response = packController.CreatePack(config);

            ;

        }

    }
}
