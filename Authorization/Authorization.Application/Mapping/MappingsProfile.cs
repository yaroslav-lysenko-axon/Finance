using Authorization.Application.DTO;
using Authorization.Domain.Models;
using AutoMapper;

namespace Authorization.Application.Mapping
{
    public class MappingsProfile : Profile
    {
        public MappingsProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(role => role.Role.Name));
        }
    }
}
