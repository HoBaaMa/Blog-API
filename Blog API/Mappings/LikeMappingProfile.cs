using AutoMapper;
using Blog_API.Models.DTOs;
using Blog_API.Models.Entities;

namespace Blog_API.Mappings
{
    public class LikeMappingProfile : Profile
    {
        public LikeMappingProfile()
        {
            CreateMap<Like, LikeDTO>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User!.UserName));
        }
    }
}
