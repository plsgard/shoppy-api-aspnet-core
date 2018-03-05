using AutoMapper;
using Shoppy.Core.Users;

namespace Shoppy.Application.Users.Dtos
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<CreateUserDto, User>();
        }
    }
}