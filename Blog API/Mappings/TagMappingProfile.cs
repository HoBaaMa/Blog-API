using AutoMapper;
using Blog_API.Models.DTOs;
using Blog_API.Models.Entities;

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
