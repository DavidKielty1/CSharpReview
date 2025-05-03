using AutoMapper;
using GeekMeet.DTOs;
using GeekMeet.Models;

namespace GeekMeet.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<CreateUserDto, User>();
        CreateMap<UpdateUserDto, User>();
    }
} 