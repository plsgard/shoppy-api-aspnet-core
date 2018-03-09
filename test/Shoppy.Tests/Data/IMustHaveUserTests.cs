using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shoppy.Core.Lists;
using Xunit;

namespace Shoppy.Tests.Data
{
    public class IMustHaveUserTests : AppTestBase
    {
        [Fact]
        public async Task SetCurrentUser()
        {
            var currentUserId = Guid.NewGuid();

            LoginAs(currentUserId);

            var entity = await CreateList("Toto");
            Assert.Equal(currentUserId, entity.UserId);

            await UseDbContextAsync(async context =>
            {
                var list = await context.Lists.SingleOrDefaultAsync(c => c.Id == entity.Id);
                Assert.NotNull(list);
                Assert.Equal(currentUserId, list.UserId);
            });
        }

        [Fact]
        public async Task FilterOnlyDateForCurrentUser()
        {
            var currentUserId = Guid.NewGuid();

            LoginAs(currentUserId);

            var entity = await CreateList("Toto");
            Assert.Equal(currentUserId, entity.UserId);

            await UseDbContextAsync(async context =>
            {                
                var list = await context.Lists.SingleOrDefaultAsync(c => c.Id == entity.Id);
                Assert.NotNull(list);
                Assert.Equal(currentUserId, list.UserId);
            });

            LoginAs(Guid.NewGuid());
            await UseDbContextAsync(async context =>
            {
                var list = await context.Lists.SingleOrDefaultAsync(c => c.Id == entity.Id);
                Assert.Null(list);

                list = await context.Lists.IgnoreQueryFilters().SingleOrDefaultAsync(c => c.Id == entity.Id);
                Assert.NotNull(list);
            });
        }
    }
}
