using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Shoppy.Application.Authentication.Dtos;
using Shoppy.Application.Commons;
using Shoppy.Application.Users.Dtos;
using Shoppy.Core.Exceptions;
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

            throw new AppIdentityResultException(identityResult.Errors);
        }

        public async Task<UserDto> Update(UpdateUserDto input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            Normalize(input);
            Validate(input);

            var entity = ToEntity(input);
            var identityResult = await _userManager.UpdateAsync(entity);
            if (identityResult.Succeeded)
                return ToDto(entity);

            throw new AppIdentityResultException(identityResult.Errors);
        }

        public async Task Delete(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                throw new ArgumentException($"No user with id '{id}'.", nameof(id));

            var identityResult = await _userManager.DeleteAsync(user);
            if (!identityResult.Succeeded)
                throw new AppIdentityResultException(identityResult.Errors);
        }

        public Task<IList<UserDto>> GetAll()
        {
            return Task.FromResult(ObjectMapper.Map<IList<UserDto>>(_userManager.Users.ToList()));
        }

        public async Task<UserDto> Get(Guid id)
        {
            return ToDto(await _userManager.FindByIdAsync(id.ToString()));
        }
    }
}