
using Icoaura.Model;

namespace Icoaura.Context
{
    public static class GlobalContext
    {
        public static bool IsFirstRun { get; set; }
    
        public static AppConfig AppConfig { get; set; } =  AppConfig.GetDefault();
    }
}
