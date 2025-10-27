using Icoaura.Context;
using Icoaura.Model;
using Icoaura.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Icoaura
{
    public class l10nService
    {

        private readonly Dictionary<string, dynamic> _en;
        private readonly Dictionary<string, dynamic> _tr;

        public l10nService()
        {
            string enJson = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "Assets",
                "l10n", 
                "en.json");
            
            string trJson = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "Assets",
                "l10n",
                "tr.json");

            if (File.Exists(enJson))
            {
                string enContent = File.ReadAllText(enJson);
                _en = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(enContent) ?? new();
            }

            if(File.Exists(trJson))
            {
                string trContent = File.ReadAllText(trJson);
                _tr = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(trContent) ?? new();
            }
        }

        public string GetLocalizedMessage(string key)
        {
            AppConfig config = GlobalContext.AppConfig;
            string[] sections = key.Split(".");

            dynamic current = _en;
            if (config.Language == Enum.Language.Tr)
            {
                current = _tr;
            }
            if(config.Language == Enum.Language.En)
            {
                current = _en;
            }

            foreach (var section in sections)
            {
                if (current.ContainsKey(section))
                {
                    current = current[section];
                }
                else
                {
                    return key; // key not found, return the key itself
                }
            }
            return current.ToString();
        }




    }
}
