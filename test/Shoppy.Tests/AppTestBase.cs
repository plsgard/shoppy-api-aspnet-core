using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shoppy.Core.Lists;
using Shoppy.Core.Session;
using Shoppy.Core.Users;
using Shoppy.Data;
using Xunit;

namespace Shoppy.Tests
{
    public abstract class AppTestBase : IDisposable
    {
        private Mock<IAppSession> _mockSession;
        private DbContextOptions<ShoppyContext> _dbContextOptions;

        protected AppTestBase()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ShoppyContext>()
                .UseInMemoryDatabase("shoppy-test")
                .Options;

            _mockSession = new Mock<IAppSession>();
        }

        private ShoppyContext GetDbContext()
        {
            return new ShoppyContext(_dbContextOptions, _mockSession.Object);
        }

        protected void LoginAs(Guid userId)
        {
            _mockSession.Setup(c => c.GetCurrentUserId()).Returns(userId);
        }

        protected async Task UseDbContextAsync(Func<ShoppyContext, Task> action)
        {
            using (var context = GetDbContext())
            {
                await action(context);
                await context.SaveChangesAsync();
            }
        }

        protected async Task<List> CreateList(string name)
        {
            var entity = new List
            {
                Name = name
            };

            await UseDbContextAsync(async context =>
            {
                await context.Lists.AddAsync(entity);
                await context.SaveChangesAsync();
            });

            return entity;
        }

        protected async Task<User> CreateUser(string firstName = "John", string lastName = "Doe", string userName = "john.doe@shoppy.com")
        {
            var entity = new User
            {
                Email = userName,
                FirstName = firstName,
                LastName = lastName,
                UserName = userName
            };

            await UseDbContextAsync(async context =>
            {
                await context.Users.AddAsync(entity);
                await context.SaveChangesAsync();
            });

            return entity;
        }

        public void Dispose()
        {
        }
    }
}
