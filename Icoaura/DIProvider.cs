using Icoaura.Controller;
using Icoaura.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Icoaura
{

    public static class DIProvider
    {
        public static IServiceProvider Provider { get; private set; } = null!;

        public static void Initialize()
        {
            ServiceCollection services = new();
            services.AddSingleton<FileController>();

            Provider = services.BuildServiceProvider();
        }
    }
}
