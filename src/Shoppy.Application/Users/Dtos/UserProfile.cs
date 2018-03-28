using AutoMapper;
using Shoppy.Application.Authentication.Dtos;
using Shoppy.Core.Users;

namespace Shoppy.Application.Users.Dtos
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, UserInfoDto>();
            CreateMap<CreateUserDto, User>();
            CreateMap<UpdateUserDto, User>();
            CreateMap<RegisterDto, CreateUserDto>().ForMember(c => c.UserName, opts => opts.MapFrom(v => v.Email));
        }
    }
}