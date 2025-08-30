using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Blog_API.Models.DTOs
{
    public class CreateTagDTO
    {
        [DisplayName("Tag name")]
        [MaxLength(25, ErrorMessage = "{0} has maximum length 25 characters.")]
        [Required(ErrorMessage = "{0} cannot be empty.")]
        public required string Name { get; set; }
    }
}
