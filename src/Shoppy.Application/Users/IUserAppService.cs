using System;
using System.Threading.Tasks;
using Shoppy.Application.Authentication.Dtos;
using Shoppy.Application.Commons;
using Shoppy.Application.Users.Dtos;

namespace Shoppy.Application.Users
{
    public interface IUserAppService : IAppService<UserDto, Guid, CreateUserDto, UpdateUserDto>
    {
        Task<UserDto> Register(RegisterDto input);
    }
}
