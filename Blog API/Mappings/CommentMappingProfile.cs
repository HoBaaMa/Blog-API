using AutoMapper;
using Blog_API.Models.DTOs;
using Blog_API.Models.Entities;

namespace Blog_API.MappingProfiles
{
    public class CommentMappingProfile : Profile
    {
        public CommentMappingProfile()
        {
            CreateMap<Comment, CommentDTO>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User!.UserName))
                .ForMember(dest => dest.LikeCount, opt => opt.MapFrom(src => src.Likes.Count));

            CreateMap<CreateCommentDTO, Comment>();
        }

    }
}
