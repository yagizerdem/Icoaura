using Icoaura.Enum;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Icoaura.Model
{
    public class AppConfig
    {
        // pack
        public bool MatchLnkByTargetExe { get; set; } = true;
        public bool MatchUrlByTargetUrl { get; set; } = true;
        public bool ChangeDescriptionOfMatchedLnkFiles { get; set; } = true;
        public float PackOpacity { get; set; } = 1f;
        public float PackCornerRadius { get; set; } = 0f;

        // panel
        public float WindowOpacity { get; set; } = 1f;


        public bool ForceExplorerRefreshAfterIcoChange { get; set; } = false;

        // loggings
        public bool EnableLogging { get; set; } = true;
        public int MaxLogCount { get; set; } = 20;

        public bool EnableTraceLogging { get; set; } = false;
        public bool EnableDebugLogging { get; set; } = false;
        public bool EnableInfoLogging { get; set; } = false;
        public bool EnableWarningLogging { get; set; } = false;
        public bool EnableErrorLogging { get; set; } = true;
        public bool EnableFatalLogging { get; set; } = true;

        public int RecursiveScanningDepth { get; set; } = 2;

        // thme
        public Theme Theme { get; set; } = Theme.DefaultSystem;

        public Language Language { get; set; } = Language.En;

        public Filter Filter { get; set; } = Filter.Default;

        public static AppConfig GetDefault()
        {
            return new AppConfig
            {
                EnableLogging = true,
                MaxLogCount = 20,

                MatchLnkByTargetExe = true,
                MatchUrlByTargetUrl = true,
                ChangeDescriptionOfMatchedLnkFiles = true,
                PackOpacity = 1f,
                PackCornerRadius = 0f,
                RecursiveScanningDepth = 2,

                WindowOpacity = 1f,

                ForceExplorerRefreshAfterIcoChange = false,

                EnableTraceLogging = false,
                EnableDebugLogging = false,
                EnableInfoLogging = false,
                EnableWarningLogging = false,
                EnableErrorLogging = true,
                EnableFatalLogging = true,
      

                Theme = Theme.DefaultSystem,
                Language = Language.En,
                Filter = Filter.Default
            };
        }
    }
}
