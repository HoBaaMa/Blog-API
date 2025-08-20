using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Blog_API.DTOs
{
    public class TagDTO
    {
        public required string Name { get; set; }
    }
}
