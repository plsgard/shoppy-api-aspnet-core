using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shoppy.Application.Items;
using Shoppy.Application.Items.Dtos;
using Shoppy.Core.Items;
using Shoppy.Data.Repositories;
using Xunit;

namespace Shoppy.Tests.Services
{
    public class ItemAppServiceTests : AppTestBase
    {
        private readonly ItemAppService _itemAppService;

        public ItemAppServiceTests()
        {
            _itemAppService = new ItemAppService(new Repository<Item, Guid>(Context));
        }

        [Theory]
        [InlineData("  test", "test")]
        [InlineData("  test  ", "test")]
        [InlineData("test  ", "test")]
        public async Task Create_Name_ShouldBeNormalized_Trimmed(string val, string expectedResult)
        {
            var itemDto = await _itemAppService.Create(new CreateItemDto
            {
                Name = val
            });
            Assert.Equal(expectedResult, itemDto.Name);

            await UseDbContextAsync(async context =>
                Assert.Equal(expectedResult, (await context.Items.IgnoreQueryFilters().FirstAsync(l => l.Id == itemDto.Id)).Name));
        }

        [Theory]
        [InlineData("  test", "test")]
        [InlineData("  test  ", "test")]
        [InlineData("test  ", "test")]
        public async Task Update_Name_ShouldBeNormalized_Trimmed(string val, string expectedResult)
        {
            var listId = (await CreateList("List")).Id;
            var item = await CreateItem(listId, "item");

            var itemDto = await _itemAppService.Update(new UpdateItemDto
            {
                Id = item.Id,
                Name = val,
                ListId = listId
            });
            Assert.Equal(expectedResult, itemDto.Name);

            await UseDbContextAsync(async context =>
                Assert.Equal(expectedResult, (await context.Items.IgnoreQueryFilters().FirstAsync(l => l.Id == itemDto.Id)).Name));
        }

        [Fact]
        public async Task Create_Name_CanCreateTwoItemWithSameName_OnSameUser_OnSameList()
        {
            var userId = (await CreateUser()).Id;
            LoginAs(userId);

            var listId = (await CreateList("list")).Id;
            var itemName = "item1";
            var item = await CreateItem(listId, itemName);

            await UseDbContextAsync(async context =>
                Assert.Equal(1, await context.Items.CountAsync(i => i.Name == itemName)));

            var itemDto = await _itemAppService.Create(new CreateItemDto
            {
                Name = itemName,
                ListId = listId
            });
            Assert.NotEqual(item.Id, itemDto.Id);
            Assert.Equal(item.Name, itemDto.Name);

            await UseDbContextAsync(async context =>
                Assert.Equal(2, await context.Items.CountAsync(i => i.Name == itemName)));
        }

        [Fact]
        public async Task Create_Name_CanCreateTwoItemWithSameName_OnSameUser_DifferentList()
        {
            var userId = (await CreateUser()).Id;
            LoginAs(userId);

            var listId = (await CreateList("list")).Id;
            var listId2 = (await CreateList("list2")).Id;
            var itemName = "item1";
            var item = await CreateItem(listId, itemName);

            await UseDbContextAsync(async context =>
                Assert.Equal(1, await context.Items.CountAsync(i => i.Name == itemName)));

            var itemDto = await _itemAppService.Create(new CreateItemDto
            {
                Name = itemName,
                ListId = listId2
            });
            Assert.NotEqual(item.Id, itemDto.Id);
            Assert.Equal(item.Name, itemDto.Name);

            await UseDbContextAsync(async context =>
                Assert.Equal(2, await context.Items.CountAsync(i => i.Name == itemName)));
        }

        [Fact]
        public async Task Create_Index_GreaterThanExistingMax_Plus10()
        {
            var userId = (await CreateUser()).Id;
            LoginAs(userId);

            var listId = (await CreateList("list")).Id;
            var itemName = "item1";
            await CreateItem(listId, itemName);
            await CreateItem(listId, itemName, 10);

            var itemDto = await _itemAppService.Create(new CreateItemDto
            {
                Name = "item2",
                ListId = listId
            });
            Assert.Equal(20, itemDto.Index);

            await UseDbContextAsync(async context =>
                Assert.Equal(20, (await context.Items.FirstAsync(i => i.Id == itemDto.Id)).Index));
        }

        [Fact]
        public async Task Create_Index_0ForFirst()
        {
            var userId = (await CreateUser()).Id;
            LoginAs(userId);

            var listId = (await CreateList("list")).Id;

            var itemDto = await _itemAppService.Create(new CreateItemDto
            {
                Name = "item2",
                ListId = listId
            });
            Assert.Equal(0, itemDto.Index);

            await UseDbContextAsync(async context =>
                Assert.Equal(0, (await context.Items.FirstAsync(i => i.Id == itemDto.Id)).Index));
        }
    }
}
