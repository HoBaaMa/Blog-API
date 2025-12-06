using AutoMapper;
using Blog_API.Models.DTOs;
using Blog_API.Models.Entities;

namespace Blog_API.Mappings
{
    public class BlogCategoryMappingProfile : Profile
    {
        public BlogCategoryMappingProfile()
        {
            CreateMap<BlogCategory, BlogCategoryDTO>().ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
            CreateMap<CreateBlogCategoryDTO, BlogCategory>();
        }
    }
}
