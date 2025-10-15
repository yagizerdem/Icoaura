using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Configs
{
    public class PackConfig
    {
        public string PackName { get; set; } = string.Empty;
        public string Uid { get; set; } = string.Empty; // director name of pack
        public string Version { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Licance { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty ;
        public float OpacityAmount { get; set; } = 0;
        public float CornerRadiusAmount { get; set; } = 0;
    }
}
