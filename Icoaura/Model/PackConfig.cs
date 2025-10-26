
namespace Icoaura.Model
{
    public class PackConfig
    {

        public string Uid { get; set; } = string.Empty;
        public string PackName { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string License { get; set; } = string.Empty;
        public string CoverPngBase64 { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public float Opacity { get; set; } = 1.0f;
        public float CornerRadius { get; set; } = 0.0f;

        public static PackConfig GetDefaultConfig()
        {
            return new PackConfig
            {
                Uid = Guid.NewGuid().ToString(),
                PackName = "New Pack",
                Version = "1.0.0",
                Author = "Author Name",
                Description = "This is a description of the pack.",
                License = "MIT",
                CoverPngBase64 = "",
                CreatedAt = DateTime.UtcNow,
                Opacity = 1.0f,
                CornerRadius = 0.0f
            };
        }
    }
}
