using Microsoft.Extensions.DependencyInjection;
using Service;

namespace Icoaura
{
    public class ConfigureServices
    {
        public static ServiceProvider Provider { get; private set; }
        public static void Configure()
        {
            ServiceCollection services = new();
            services.AddSingleton<FileService>();
            services.AddSingleton<LnkService>();
            services.AddSingleton<ImageProcessorService>();
            services.AddSingleton<UrlService>();
            services.AddSingleton<DirIconService>();
            services.AddSingleton<PackService>();

            Provider = services.BuildServiceProvider();
        }
    }
}
