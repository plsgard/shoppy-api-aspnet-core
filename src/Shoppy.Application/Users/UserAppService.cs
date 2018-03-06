using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Shoppy.Application.Authentication.Dtos;
using Shoppy.Application.Commons;
using Shoppy.Application.Users.Dtos;
using Shoppy.Core.Users;

namespace Shoppy.Application.Users
{
    public class UserAppService : BaseAppService<User, UserDto>, IUserAppService
    {
        private readonly UserManager<User> _userManager;

        public UserAppService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public Task<UserDto> Register(RegisterDto input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            Normalize(input);
            Validate(input);

            return Create(ObjectMapper.Map<CreateUserDto>(input));
        }

        public async Task<UserDto> Create(CreateUserDto input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            Normalize(input);
            Validate(input);

            var user = ToEntity(input);
            var identityResult = await _userManager.CreateAsync(user, input.Password);
            if (identityResult == IdentityResult.Success)
                return ToDto(user);

            throw new Exception(String.Join(" - ", identityResult.Errors.Select(c => $"{c.Code} : {c.Description}")));
        }

        public async Task<UserDto> Get(Guid id)
        {
            return ToDto(await _userManager.FindByIdAsync(id.ToString()));
        }
    }
}