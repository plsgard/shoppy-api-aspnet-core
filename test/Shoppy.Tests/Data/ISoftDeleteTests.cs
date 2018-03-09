using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Shoppy.Tests.Data
{
    public class ISoftDeleteTests : AppTestBase
    {
        [Fact]
        public async Task DoNotDelete()
        {
            var entity = await CreateUser();
            await UseDbContextAsync(async context =>
            {
                var user = await context.Users.SingleOrDefaultAsync(c => c.Id == entity.Id);
                Assert.NotNull(user);
                Assert.False(user.IsDeleted);

                context.Users.Remove(user);
                await context.SaveChangesAsync();
            });

            await UseDbContextAsync(async context =>
            {
                var user = await context.Users.IgnoreQueryFilters().SingleOrDefaultAsync(c => c.Id == entity.Id);
                Assert.NotNull(user);
                Assert.True(user.IsDeleted);
            });
        }

        [Fact]
        public async Task FilterDeletedEntity()
        {
            var entity = await CreateUser();
            await UseDbContextAsync(async context =>
            {
                var user = await context.Users.SingleOrDefaultAsync(c => c.Id == entity.Id);
                Assert.NotNull(user);

                context.Users.Remove(user);
                await context.SaveChangesAsync();
            });

            await UseDbContextAsync(async context =>
            {
                var user = await context.Users.SingleOrDefaultAsync(c => c.Id == entity.Id);
                Assert.Null(user);
            });
        }
    }
}
