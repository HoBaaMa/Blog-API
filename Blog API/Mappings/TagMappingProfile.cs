using AutoMapper;
using Blog_API.Models;
using Blog_API.DTOs;

namespace Blog_API.MappingProfiles
{
    public class TagMappingProfile : Profile
    {
        public TagMappingProfile()
        {
            CreateMap<Tag, TagDTO>();
        }
    }
}
