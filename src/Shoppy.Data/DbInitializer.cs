using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shoppy.Core;
using Shoppy.Core.Exceptions;
using Shoppy.Core.Roles;
using Shoppy.Core.Users;

namespace Shoppy.Data
{
    public class DbInitializer
    {
        public static async Task Initialize(ShoppyContext context, UserManager<User> userManager,
            RoleManager<Role> roleManager, ILogger<DbInitializer> logger, IConfiguration configuration)
        {
            context.Database.EnsureCreated();

            // Look for any users.
            if (context.Users.Any())
            {
                return; // DB has been seeded
            }

            await CreateDefaultUserAndRoleForApplication(userManager, roleManager, logger, configuration);
        }

        private static async Task CreateDefaultUserAndRoleForApplication(UserManager<User> userManager,
            RoleManager<Role> roleManager, ILogger<DbInitializer> logger, IConfiguration configuration)
        {
            string administratorRole = AppConsts.Roles.Administrator;
            string email = configuration["Data:Default:Admin:UserName"];

            await CreateDefaultAdministratorRole(roleManager, logger, administratorRole);
            var user = await CreateDefaultUser(userManager, logger, email);
            await SetPasswordForDefaultUser(userManager, logger, email, user, configuration);
            await AddDefaultRoleToDefaultUser(userManager, logger, email, administratorRole, user);
        }

        private static async Task CreateDefaultAdministratorRole(RoleManager<Role> roleManager, ILogger<DbInitializer> logger, string administratorRole)
        {
            logger.LogInformation($"Create the role `{administratorRole}` for application");
            var identityResult = await roleManager.CreateAsync(new Role(administratorRole));
            if (identityResult.Succeeded)
            {
                logger.LogDebug($"Created the role `{administratorRole}` successfully");
            }
            else
            {
                var exception = new ApplicationException($"Default role `{administratorRole}` cannot be created");
                logger.LogError(exception, new AppIdentityResultException(identityResult.Errors).Message);
                throw exception;
            }
        }

        private static async Task<User> CreateDefaultUser(UserManager<User> userManager, ILogger<DbInitializer> logger, string email)
        {
            logger.LogInformation($"Create default user with email `{email}` for application");
            var user = new User(email, "Shoppy", "Administrator");

            var identityResult = await userManager.CreateAsync(user);
            if (identityResult.Succeeded)
            {
                logger.LogDebug($"Created default user `{email}` successfully");
            }
            else
            {
                var exception = new ApplicationException($"Default user `{email}` cannot be created");
                logger.LogError(exception, new AppIdentityResultException(identityResult.Errors).Message);
                throw exception;
            }

            var createdUser = await userManager.FindByEmailAsync(email);
            return createdUser;
        }

        private static async Task SetPasswordForDefaultUser(UserManager<User> userManager, ILogger<DbInitializer> logger, string email, User user, IConfiguration configuration)
        {
            logger.LogInformation($"Set password for default user `{email}`");
            string password = configuration["Data:Default:Admin:Password"];
            var identityResult = await userManager.AddPasswordAsync(user, password);
            if (identityResult.Succeeded)
            {
                logger.LogTrace($"Set password `{password}` for default user `{email}` successfully");
            }
            else
            {
                var exception = new ApplicationException($"Password for the user `{email}` cannot be set");
                logger.LogError(exception, new AppIdentityResultException(identityResult.Errors).Message);
                throw exception;
            }
        }

        private static async Task AddDefaultRoleToDefaultUser(UserManager<User> userManager, ILogger<DbInitializer> logger, string email, string administratorRole, User user)
        {
            logger.LogInformation($"Add default user `{email}` to role '{administratorRole}'");
            var identityResult = await userManager.AddToRoleAsync(user, administratorRole);
            if (identityResult.Succeeded)
            {
                logger.LogDebug($"Added the role '{administratorRole}' to default user `{email}` successfully");
            }
            else
            {
                var exception = new ApplicationException($"The role `{administratorRole}` cannot be set for the user `{email}`");
                logger.LogError(exception, new AppIdentityResultException(identityResult.Errors).Message);
                throw exception;
            }
        }
    }
}