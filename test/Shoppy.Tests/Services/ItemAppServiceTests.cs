using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shoppy.Application.Items;
using Shoppy.Application.Items.Dtos;
using Shoppy.Data.Repositories;
using Xunit;

namespace Shoppy.Tests.Services
{
    public class ItemAppServiceTests : AppTestBase
    {
        private readonly ItemAppService _itemAppService;

        public ItemAppServiceTests()
        {
            _itemAppService = new ItemAppService(new ItemRepository(Context), new ListRepository(Context));
        }

        [Theory]
        [InlineData("  test", "test")]
        [InlineData("  test  ", "test")]
        [InlineData("test  ", "test")]
        public async Task Create_Name_ShouldBeNormalized_Trimmed(string val, string expectedResult)
        {
            var userId1 = (await CreateUser()).Id;
            LoginAs(userId1);
            var listId1 = (await CreateList("liste1")).Id;
            var itemDto = await _itemAppService.Create(new CreateItemDto
            {
                Name = val,
                ListId = listId1
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
            var userId1 = (await CreateUser()).Id;
            LoginAs(userId1);
            var listId = (await CreateList("List")).Id;
            var item = await CreateItem(listId, "item");

            var itemDto = await _itemAppService.Update(new UpdateItemDto
            {
                Id = item.Id,
                Name = val,
                ListId = listId,
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

        [Fact]
        public async Task GetAll_FromSharedListOnly_FilterOnOneList_NotBeingTheListUser()
        {
            var userId1 = (await CreateUser()).Id;
            var currentUser = (await CreateUser(userName: "test@toto.com")).Id;

            LoginAs(userId1);

            var listId1 = (await CreateList("liste1")).Id;
            var itemId = (await CreateItem(listId1, "item1")).Id;

            await CreateShare(listId1, currentUser);

            LoginAs(currentUser);

            var itemDtos = await _itemAppService.GetAll(new GetAllItemsDto { ListId = listId1 });
            Assert.NotNull(itemDtos);
            Assert.NotEmpty(itemDtos);
            Assert.Equal(1, itemDtos.Count);
            Assert.Contains(itemDtos, dto => dto.Id == itemId);
        }

        [Fact]
        public async Task GetAll_FromSharedList_FilterOnOneList_BeingTheListUser()
        {
            var userId1 = (await CreateUser()).Id;
            var currentUser = (await CreateUser(userName: "test@toto.com")).Id;

            LoginAs(userId1);

            var listId1 = (await CreateList("liste1")).Id;
            var itemId = (await CreateItem(listId1, "item1")).Id;

            await CreateShare(listId1, currentUser);

            LoginAs(currentUser);
            var item2Id = (await CreateItem(listId1, "item2")).Id;

            var itemDtos = await _itemAppService.GetAll(new GetAllItemsDto { ListId = listId1 });
            Assert.NotNull(itemDtos);
            Assert.NotEmpty(itemDtos);
            Assert.Equal(2, itemDtos.Count);
            Assert.Contains(itemDtos, dto => dto.Id == itemId);
            Assert.Contains(itemDtos, dto => dto.Id == item2Id);
        }

        [Fact]
        public async Task Create_OnSharedList()
        {
            var userId1 = (await CreateUser()).Id;
            var currentUser = (await CreateUser(userName: "test@toto.com")).Id;

            LoginAs(userId1);

            var listId1 = (await CreateList("liste1")).Id;
            await CreateItem(listId1, "item1");

            await CreateShare(listId1, currentUser);

            LoginAs(currentUser);
            var itemDto = await _itemAppService.Create(new CreateItemDto
            {
                Name = "item2",
                ListId = listId1
            });

            Assert.NotNull(itemDto);
            Assert.NotEqual(Guid.Empty, itemDto.Id);

            await UseDbContextAsync(async context =>
            {
                var list = await context.Lists.IgnoreQueryFilters().Include(l => l.Items).SingleOrDefaultAsync(l => l.Id == listId1);
                Assert.NotNull(list);
                Assert.NotEmpty(list.Items);
                Assert.Equal(2, list.Items.Count);
            });
        }

        [Fact]
        public async Task Create_OnNotSharedList_Exception()
        {
            var userId1 = (await CreateUser()).Id;
            var currentUser = (await CreateUser(userName: "test@toto.com")).Id;

            LoginAs(userId1);

            var listId1 = (await CreateList("liste1")).Id;
            await CreateItem(listId1, "item1");

            //await CreateShare(listId1, currentUser);

            LoginAs(currentUser);
            await Assert.ThrowsAsync<ArgumentException>(() => _itemAppService.Create(new CreateItemDto
            {
                Name = "item2",
                ListId = listId1
            }));
        }

        [Fact]
        public async Task Update_OnItemNotMine_ButOnSharedList()
        {
            var userId1 = (await CreateUser()).Id;
            var currentUser = (await CreateUser(userName: "test@toto.com")).Id;

            LoginAs(userId1);

            var listId1 = (await CreateList("liste1")).Id;
            var itemId = (await CreateItem(listId1, "item1")).Id;

            await CreateShare(listId1, currentUser);

            LoginAs(currentUser);
            var itemDto = await _itemAppService.Update(new UpdateItemDto
            {
                Name = "item2",
                ListId = listId1,
                Index = 20,
                Picked = true,
                Id = itemId
            });

            Assert.NotNull(itemDto);
            Assert.Equal(itemId, itemDto.Id);

            await UseDbContextAsync(async context =>
            {
                var item = await context.Items.IgnoreQueryFilters().SingleOrDefaultAsync(l => l.Id == itemId);
                Assert.NotNull(item);
                Assert.Equal("item2", item.Name);
                Assert.Equal(20, item.Index);
                Assert.True(item.Picked);
                Assert.Equal(userId1, item.UserId);
                Assert.Equal(userId1, item.CreationUserId);
                Assert.Equal(currentUser, item.ModificationUserId);
            });
        }

        [Fact]
        public async Task Update_OnNotSharedList_Exception()
        {
            var userId1 = (await CreateUser()).Id;
            var currentUser = (await CreateUser(userName: "test@toto.com")).Id;

            LoginAs(userId1);

            var listId1 = (await CreateList("liste1")).Id;
            var itemId = (await CreateItem(listId1, "item1")).Id;

            //await CreateShare(listId1, currentUser);

            LoginAs(currentUser);
            await Assert.ThrowsAsync<ArgumentException>(() => _itemAppService.Update(new UpdateItemDto
            {
                Name = "item2",
                ListId = listId1,
                Index = 20,
                Picked = true,
                Id = itemId
            }));
        }

        [Fact]
        public async Task Delete_Cannot_ListNoMine_Exception()
        {
            var userId = (await CreateUser()).Id;
            var currentUserId = (await CreateUser(userName: "test@toto.com")).Id;
            LoginAs(userId);

            var listId = (await CreateList("liste1")).Id;
            var itemId = (await CreateItem(listId, "item1")).Id;

            LoginAs(currentUserId);
            await Assert.ThrowsAsync<ArgumentException>(() => _itemAppService.Delete(itemId));
        }

        [Fact]
        public async Task Delete_Can_ListShareToMe()
        {
            var userId = (await CreateUser()).Id;
            var currentUserId = (await CreateUser(userName: "test@toto.com")).Id;
            LoginAs(userId);

            var listId = (await CreateList("liste1")).Id;
            var itemId = (await CreateItem(listId, "item1")).Id;

            await CreateShare(listId, currentUserId);

            LoginAs(currentUserId);
            await _itemAppService.Delete(itemId);

            await UseDbContextAsync(async context =>
                Assert.False(await context.Items.IgnoreQueryFilters().AnyAsync(i => i.Id == itemId)));
        }

        [Fact]
        public async Task Update_Cannot_NotMine_Exception()
        {
            var userId = (await CreateUser()).Id;
            var currentUserId = (await CreateUser(userName: "test@toto.com")).Id;
            LoginAs(userId);

            var listId = (await CreateList("liste1")).Id;
            var itemId = (await CreateItem(listId, "item1")).Id;

            LoginAs(currentUserId);
            await Assert.ThrowsAsync<ArgumentException>(() => _itemAppService.Update(new UpdateItemDto
            {
                Id = itemId,
                ListId = listId,
                Name = "toto"
            }));
        }

        [Fact]
        public async Task Update_Can_ListShareToMe()
        {
            var userId = (await CreateUser()).Id;
            var currentUserId = (await CreateUser(userName: "test@toto.com")).Id;
            LoginAs(userId);

            var listId = (await CreateList("liste1")).Id;
            var itemId = (await CreateItem(listId, "item1")).Id;

            await CreateShare(listId, currentUserId);

            LoginAs(currentUserId);
            var itemDto = await _itemAppService.Update(new UpdateItemDto
            {
                Id = itemId,
                ListId = listId,
                Name = "toto"
            });
            Assert.NotNull(itemDto);

            await UseDbContextAsync(async context =>
            {
                var item = await context.Items.IgnoreQueryFilters().SingleOrDefaultAsync(c => c.Id == itemId);
                Assert.NotNull(item);
                Assert.Equal("toto", item.Name);
                Assert.Equal(currentUserId, item.ModificationUserId);
            });
        }

        [Fact]
        public async Task Get_Cannot_NotMine_Null()
        {
            var userId = (await CreateUser()).Id;
            var currentUserId = (await CreateUser(userName: "test@toto.com")).Id;
            LoginAs(userId);

            var listId = (await CreateList("liste1")).Id;
            var itemId = (await CreateItem(listId, "item1")).Id;

            LoginAs(currentUserId);
            var itemDto = await _itemAppService.Get(itemId);
            Assert.Null(itemDto);
        }

        [Fact]
        public async Task Get_Can_ShareWithMe()
        {
            var userId = (await CreateUser()).Id;
            var currentUserId = (await CreateUser(userName: "test@toto.com")).Id;
            LoginAs(userId);

            var listId = (await CreateList("liste1")).Id;
            var itemId = (await CreateItem(listId, "item1")).Id;

            await CreateShare(listId, currentUserId);

            LoginAs(currentUserId);
            var itemDto = await _itemAppService.Get(itemId);
            Assert.NotNull(itemDto);
        }

        [Fact]
        public async Task GetAll_OnlyMine()
        {
            var userId = (await CreateUser()).Id;
            var currentUserId = (await CreateUser(userName: "test@toto.com")).Id;
            LoginAs(userId);

            var listId1 = (await CreateList("liste1")).Id;
            await CreateItem(listId1, "item1");

            LoginAs(currentUserId);
            var listId2 = (await CreateList("liste2")).Id;
            var itemId2 = (await CreateItem(listId2, "item1")).Id;

            var itemDtos = await _itemAppService.GetAll();
            Assert.NotNull(itemDtos);
            Assert.Single(itemDtos);
            Assert.Contains(itemDtos, dto => dto.Id == itemId2);
        }

        [Fact]
        public async Task GetAll_ListShared_And_AllLists_All()
        {
            var userId = (await CreateUser()).Id;
            var currentUserId = (await CreateUser(userName: "test@toto.com")).Id;
            LoginAs(userId);

            var listId1 = (await CreateList("liste1")).Id;
            var itemId1 = (await CreateItem(listId1, "item1")).Id;

            await CreateShare(listId1, currentUserId);

            LoginAs(currentUserId);
            var listId2 = (await CreateList("liste2")).Id;
            var itemId2 = (await CreateItem(listId2, "item1")).Id;

            var itemDtos = await _itemAppService.GetAll();
            Assert.NotNull(itemDtos);
            Assert.Equal(2, itemDtos.Count);
            Assert.Contains(itemDtos, dto => dto.Id == itemId1);
            Assert.Contains(itemDtos, dto => dto.Id == itemId2);
        }
    }
}
