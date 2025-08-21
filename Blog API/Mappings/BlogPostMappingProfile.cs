using Blog_API.Models;
using Blog_API.DTOs;
using AutoMapper;

namespace Blog_API.MappingProfiles
{
    public class BlogPostMappingProfile : Profile
    {
        public BlogPostMappingProfile()
        {
            CreateMap<BlogPost, BlogPostDTO>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User!.UserName))
                .ForMember(dest => dest.LikeCount, opt => opt.MapFrom(src => src.Likes.Count))
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments.Where(c => c.ParentCommentId == null)));

            CreateMap<CreateBlogPostDTO, BlogPost>()
                .ForMember(dest => dest.Tags, opt => opt.Ignore());
            //CreateMap<CreateBlogPostDTO, BlogPost>()
            //    .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => new Tag { Name = t })));

        }
    }
}