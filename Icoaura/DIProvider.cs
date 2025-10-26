using Icoaura.Controller;
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
            services.AddSingleton<ExternalController>();

            Provider = services.BuildServiceProvider();
        }
    }
}
