using System.ComponentModel.DataAnnotations;

namespace Blog_API.Validators
{
    public class BlogCategoryValidator : ValidationAttribute
    {

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {

            return base.IsValid(value, validationContext);
        }
    }
}
