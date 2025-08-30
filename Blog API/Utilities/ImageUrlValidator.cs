using System.Text.RegularExpressions;

namespace Blog_API.Utilities
{
    public static class ImageUrlValidator
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };
        private static readonly Regex UrlPattern = new Regex(
            @"^https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static bool IsValidImageUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            // Check if it's a valid URL format
            if (!UrlPattern.IsMatch(url))
                return false;

            // Check if it has a valid image extension
            var uri = new Uri(url);
            var path = uri.AbsolutePath.ToLowerInvariant();
            
            return AllowedExtensions.Any(ext => path.EndsWith(ext));
        }

        public static (bool isValid, List<string> invalidUrls) ValidateImageUrls(IEnumerable<string> imageUrls)
        {
            var invalidUrls = new List<string>();
            
            foreach (var url in imageUrls)
            {
                if (!IsValidImageUrl(url))
                {
                    invalidUrls.Add(url);
                }
            }

            return (invalidUrls.Count == 0, invalidUrls);
        }
    }
}