using System;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Configuration;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shoppy.Core.Items;
using Shoppy.Core.Lists;
using Shoppy.Core.Session;
using Shoppy.Core.Users;
using Shoppy.Data;
using Xunit;

namespace Shoppy.Tests
{
    [Collection("Services")]
    public abstract class AppTestBase : IDisposable
    {
        private readonly Mock<IAppSession> _mockSession;
        private readonly DbContextOptions<ShoppyContext> _dbContextOptions;
        protected ShoppyContext Context { get; }

        protected AppTestBase()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ShoppyContext>()
                .UseInMemoryDatabase("shoppy-test")
                .Options;

            _mockSession = new Mock<IAppSession>();

            Context = GetDbContext();
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
            await action(Context);
            await Context.SaveChangesAsync();
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

        protected async Task<Item> CreateItem(Guid listId, string name)
        {
            var entity = new Item
            {
                ListId = listId,
                Name = name
            };

            await UseDbContextAsync(async context =>
            {
                await context.Items.AddAsync(entity);
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
            Context?.Dispose();
        }
    }

    public class AppTestsBase : IDisposable
    {
        public AppTestsBase()
        {
            InitializeMappings();
        }

        private void InitializeMappings()
        {
            Mapper.Initialize(cfg => cfg.AddProfiles(Assembly.Load("Shoppy.Application")));
        }

        public void Dispose()
        {
            Mapper.Reset();
        }
    }

    [CollectionDefinition("Services")]
    public class ServicesCollection : ICollectionFixture<AppTestsBase>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
