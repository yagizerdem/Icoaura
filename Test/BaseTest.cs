using Icoaura;
using Icoaura.Controller;
using Microsoft.Extensions.DependencyInjection;

namespace Test
{
    public class BaseTest
    {
        public TestContext TestContext { get; set; }

        public FileController fileController { get; private set; }  

        public ExternalController externalController { get; private set; }

        public PackController packController { get; private set; }


        [TestInitialize]
        public void Initialize()
        {
            App app = new App();
            app.Initialize();

            fileController = DIProvider.Provider.GetRequiredService<FileController>();
            externalController = DIProvider.Provider.GetRequiredService<ExternalController>();
            packController = DIProvider.Provider.GetRequiredService<PackController>();  
        }
    }
}
