using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DTO
{
    public class CreatePackDTO
    {
        public string PackName { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Licance { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Base64CoverImage { get; set; } = string.Empty; // optional accepts only png !
    }
}
