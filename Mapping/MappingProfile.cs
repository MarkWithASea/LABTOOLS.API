using AutoMapper;
using LABTOOLS.API.DataTransferObjects;
using LABTOOLS.API.Models;

namespace LABTOOLS.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.Roles!.FirstOrDefault()!.Id))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Roles!.FirstOrDefault()!.Name));

            CreateMap<Role, RoleDTO>();
        }
    }
}