using Icoaura;
using Icoaura.Controller;
using Microsoft.Extensions.DependencyInjection;

namespace Test
{
    public class BaseTest
    {
        public TestContext TestContext { get; set; }

        public FileController fileController { get; private set; }  


        [TestInitialize]
        public void Initialize()
        {
            App app = new App();
            app.Initialize();

            fileController = DIProvider.Provider.GetRequiredService<FileController>();

        }
    }
}
