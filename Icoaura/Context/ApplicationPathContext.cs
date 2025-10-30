using System.IO;

namespace Icoaura.Context
{
    public static class ApplicationPathContext
    {

        // derived paths
        public static string AppRootPath
            => Path.Combine(Environment.GetFolderPath
                (Environment.SpecialFolder.ApplicationData), APP_ROOT_FOLDER_NAME);

        public static string AppTempFolderPath => System.IO.Path.Combine(AppRootPath, APP_TEMP_FOLDER_NAME);

        public static string AppConfigFilePath => System.IO.Path.Combine(AppRootPath, APP_CONFIG_FILE_NAME);

        public static string AppPacksFolderPath => System.IO.Path.Combine(AppRootPath, APP_PACKS_FOLDER_NAME);

        public static string AppLogsFolderPath => System.IO.Path.Combine(AppRootPath, APP_LOGS_FOLDER_NAME);

        public static string AppIconsFolderPath => System.IO.Path.Combine(AppRootPath, APP_ICONS_FOLDER_NAME);


        // magic strings

        public static string APP_CONFIG_FILE_NAME = "config.json";

        public static string APP_TEMP_FOLDER_NAME = "Temp";

        public static string APP_PACKS_FOLDER_NAME = "Packs";

        public static string APP_ROOT_FOLDER_NAME = "Icoaura";

        public static string APP_LOGS_FOLDER_NAME = "Logs";

        public static string APP_ICONS_FOLDER_NAME = "Icons";

        public static string PACK_CONFIG_FILE_NAME = "config.json";

        public static string PACK_ICONS_FOLDER_NAME = "Icons";

        public static string PACK_ITEMS_FILE_NAME = "items.json";

        public static string PACK_COVER_PNG_FILE_NAME = "cover.png";


    }
}
