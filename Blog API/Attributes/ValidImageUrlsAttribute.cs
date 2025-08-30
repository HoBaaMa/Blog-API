using Blog_API.Utilities;
using System.ComponentModel.DataAnnotations;

namespace Blog_API.Attributes
{
    public class ValidImageUrlsAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null)
                return true; // Allow null/empty collections

            if (value is IEnumerable<string> urls)
            {
                var urlList = urls.ToList();
                if (urlList.Count == 0)
                    return true; // Allow empty collections

                var (isValid, invalidUrls) = ImageUrlValidator.ValidateImageUrls(urlList);
                if (!isValid)
                {
                    ErrorMessage = $"Invalid image URLs: {string.Join(", ", invalidUrls)}";
                    return false;
                }
                
                return true;
            }

            return false;
        }
    }
}