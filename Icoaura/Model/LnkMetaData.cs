using System;

namespace Model.DTO
{
    public class LnkMetaData
    {
        public string ShortcutPath { get; set; } = string.Empty;
        public string TargetPath { get; set; } = string.Empty;
        public string WorkingDirectory { get; set; } = string.Empty;
        public string Arguments { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconPath { get; set; } = string.Empty;
        public int IconIndex { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public DateTime LastAccessedTime { get; set; }
        public bool IsTargetValid { get; set; }
        public string TargetMimeType { get; set; } = string.Empty;
        public string TargetExtension { get; set; } = string.Empty;

        public string DisplayName => System.IO.Path.GetFileNameWithoutExtension(ShortcutPath);
    }
}