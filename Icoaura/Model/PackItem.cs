using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Icoaura.Model
{
    class PackItem
    {
        public string Uid { get; set; } = string.Empty; // unique identifier

        // flags that indicate target entity types
        public bool IsLnkItem => TargetPath.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase);
        public bool IsUrlItem => TargetPath.EndsWith(".url", StringComparison.OrdinalIgnoreCase);
        public bool IsDirItem =>
            !string.IsNullOrEmpty(TargetPath) &&
            !System.IO.Path.HasExtension(TargetPath);

        // common
        public string TargetPath { get; set; } = string.Empty; // target file path that this pack item applies

        public string Name { get; set; } = string.Empty; // file name only  // only for lnk and url items

        // lnk realted properties
        public string TargetExePath { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        // Url related
        public string TargetUrl { get; set; } = string.Empty;
    }
}
