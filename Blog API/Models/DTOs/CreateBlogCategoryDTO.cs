using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Blog_API.Models.DTOs
{
    public class CreateBlogCategoryDTO
    {
        [DisplayName("Category Name")]
        [Required(ErrorMessage = "{0} is required.")]
        public required string Name { get; set; }
    }
}
