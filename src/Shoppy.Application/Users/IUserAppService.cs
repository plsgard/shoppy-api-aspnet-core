using System;
using System.Threading.Tasks;
using Shoppy.Application.Authentication.Dtos;
using Shoppy.Application.Users.Dtos;

namespace Shoppy.Application.Users
{
    public interface IUserAppService
    {
        Task<UserDto> Register(RegisterDto input);
        Task<UserDto> Create(CreateUserDto input);
        Task<UserDto> Get(Guid id);
    }
}
