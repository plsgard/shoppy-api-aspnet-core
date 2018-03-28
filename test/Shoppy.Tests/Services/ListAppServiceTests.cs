using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shoppy.Application.Lists;
using Shoppy.Application.Lists.Dtos;
using Shoppy.Core.Lists;
using Shoppy.Data.Repositories;
using Xunit;

namespace Shoppy.Tests.Services
{
    public class ListAppServiceTests : AppTestBase
    {
        private readonly ListAppService _listAppService;

        public ListAppServiceTests()
        {
            _listAppService = new ListAppService(new Repository<List, Guid>(Context), new ItemRepository(Context));
        }

        [Theory]
        [InlineData("  test", "test")]
        [InlineData("  test  ", "test")]
        [InlineData("test  ", "test")]
        public async Task Create_Name_ShouldBeNormalized_Trimmed(string val, string expectedResult)
        {
            var listDto = await _listAppService.Create(new CreateListDto
            {
                Name = val
            });
            Assert.Equal(expectedResult, listDto.Name);

            await UseDbContextAsync(async context =>
                Assert.Equal(expectedResult, (await context.Lists.IgnoreQueryFilters().FirstAsync(l => l.Id == listDto.Id)).Name));
        }

        [Theory]
        [InlineData("  test", "test")]
        [InlineData("  test  ", "test")]
        [InlineData("test  ", "test")]
        public async Task Update_Name_ShouldBeNormalized_Trimmed(string val, string expectedResult)
        {
            var listId = (await CreateList("toto")).Id;
            var listDto = await _listAppService.Update(new UpdateListDto
            {
                Id = listId,
                Name = val
            });
            Assert.Equal(expectedResult, listDto.Name);

            await UseDbContextAsync(async context =>
                Assert.Equal(expectedResult, (await context.Lists.IgnoreQueryFilters().FirstAsync(l => l.Id == listDto.Id)).Name));
        }

        [Fact]
        public async Task Create_Name_CanCreateTwoListWithSameName_OnSameUser()
        {
            var userId = (await CreateUser()).Id;
            LoginAs(userId);

            var listName = "list1";
            var list = await CreateList(listName);

            await UseDbContextAsync(async context =>
                Assert.Equal(1, await context.Lists.CountAsync(i => i.Name == listName)));

            var listDto = await _listAppService.Create(new CreateListDto
            {
                Name = listName
            });
            Assert.NotEqual(list.Id, listDto.Id);
            Assert.Equal(list.Name, listDto.Name);

            await UseDbContextAsync(async context =>
                Assert.Equal(2, await context.Lists.CountAsync(i => i.Name == listName)));
        }

        [Fact]
        public async Task Duplicate_WithItems_AllExistingAndNotPicked()
        {
            var userId = (await CreateUser()).Id;
            LoginAs(userId);
            var id = (await CreateList("liste1")).Id;
            var item1 = await CreateItem(id, "item1", picked: true);
            var item2 = await CreateItem(id, "item2");
            var item3 = await CreateItem(id, "item3", picked: true);

            var newListId = (await _listAppService.Duplicate(new DuplicateListDto
            {
                Name = "liste2",
                ExistingListId = id
            })).Id;

            await UseDbContextAsync(async context =>
            {
                var list = await context.Lists.SingleOrDefaultAsync(c=>c.Id == newListId);
                Assert.NotNull(list);
                Assert.NotEqual(id, list.Id);
                Assert.Equal("liste2", list.Name);

                var newItems = await context.Items.Where(i => i.ListId == newListId).ToListAsync();
                Assert.NotNull(newItems);
                Assert.Equal(3, newItems.Count);

                foreach (var newItem in newItems)
                {
                    var existingItem = new[] { item1, item2, item3 }.FirstOrDefault(i => i.Name == newItem.Name);
                    Assert.NotNull(existingItem);
                    Assert.False(newItem.Picked);
                    Assert.NotNull(newItem.CreationUserId);
                    Assert.NotEqual(DateTimeOffset.MinValue, newItem.CreationTime);
                    Assert.Equal(userId, newItem.UserId);
                    Assert.Equal(existingItem.Index, newItem.Index);
                    Assert.Equal(newListId, newItem.ListId);
                    Assert.NotEqual(existingItem.Id, newItem.Id);
                }
            });
        }
    }
}
