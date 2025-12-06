using AutoMapper;
using Blog_API.Models.DTOs;
using Blog_API.Models.Entities;

namespace Blog_API.MappingProfiles
{
    public class BlogPostMappingProfile : Profile
    {
        public BlogPostMappingProfile()
        {
            CreateMap<BlogPost, BlogPostDTO>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User!.UserName))
                .ForMember(dest => dest.LikeCount, opt => opt.MapFrom(src => src.Likes.Count))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.BlogCategory != null ? src.BlogCategory.Name : string.Empty))
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments.Where(c => c.ParentCommentId == null)))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ImageUrls));

            CreateMap<CreateBlogPostDTO, BlogPost>()
                .ForMember(dest => dest.Tags, opt => opt.Ignore())
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ImageUrls))
                .ForPath(dest => dest.BlogCategory.Name, opt => opt.MapFrom(src => src.CategoryName));
        }
    }
}