using System;
using System.Threading.Tasks;
using Shoppy.Application.Users.Dtos;

namespace Shoppy.Application.Users
{
    public interface IUserAppService
    {
        Task<UserDto> Create(CreateUserDto input);
        Task<UserDto> Get(Guid id);
    }
}
