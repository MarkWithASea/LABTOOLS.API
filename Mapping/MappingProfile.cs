using AutoMapper;
using LABTOOLS.API.DataTransferObjects;
using LABTOOLS.API.Models;

namespace LABTOOLS.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDTO>();
        }
    }
}