using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shoppy.Core.Items;
using Shoppy.Core.Lists;
using Shoppy.Core.Session;
using Shoppy.Core.Shares;
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

        private readonly Random _random = new Random();

        protected string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
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

        protected async Task<Item> CreateItem(Guid listId, string name, int index = 0, bool picked = false)
        {
            var entity = new Item
            {
                ListId = listId,
                Name = name,
                Index = index,
                Picked = picked
            };

            await UseDbContextAsync(async context =>
            {
                await context.Items.AddAsync(entity);
                await context.SaveChangesAsync();
            });

            return entity;
        }

        protected async Task<Share> CreateShare(Guid listId, Guid userId)
        {
            var entity = new Share
            {
                ListId = listId,
                UserId = userId
            };

            await UseDbContextAsync(async context =>
            {
                await context.Shares.AddAsync(entity);
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

        protected async Task<User> CreateRandomUser()
        {
            return await CreateUser(userName: $"{RandomString(6)}@test.com");
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
