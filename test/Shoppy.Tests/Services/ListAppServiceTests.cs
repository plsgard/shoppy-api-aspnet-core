using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Shoppy.Application.Lists;
using Shoppy.Application.Lists.Dtos;
using Shoppy.Core.Roles;
using Shoppy.Core.Users;
using Shoppy.Data;
using Shoppy.Data.Repositories;
using Xunit;

namespace Shoppy.Tests.Services
{
    public class ListAppServiceTests : AppTestBase
    {
        private readonly ListAppService _listAppService;
        private Mock<IUserEmailStore<User>> _mockUserStore;

        public ListAppServiceTests()
        {
            _mockUserStore = new Mock<IUserEmailStore<User>>();
            _listAppService = new ListAppService(new ListRepository(Context), new ItemRepository(Context), new UserManager<User>(_mockUserStore.Object, null, null, null, null, null, null, null, null));
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
            var userId = (await CreateUser()).Id;
            LoginAs(userId);
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
                var list = await context.Lists.SingleOrDefaultAsync(c => c.Id == newListId);
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

        [Fact]
        public async Task Duplicate_WithoutItems_CreateOnlyNewList()
        {
            var userId = (await CreateUser()).Id;
            LoginAs(userId);
            var id = (await CreateList("liste1")).Id;

            var newListId = (await _listAppService.Duplicate(new DuplicateListDto
            {
                Name = "liste2",
                ExistingListId = id
            })).Id;

            await UseDbContextAsync(async context =>
            {
                var list = await context.Lists.SingleOrDefaultAsync(c => c.Id == newListId);
                Assert.NotNull(list);
                Assert.NotEqual(id, list.Id);
                Assert.Equal("liste2", list.Name);

                var newItems = await context.Items.Where(i => i.ListId == newListId).ToListAsync();
                Assert.NotNull(newItems);
                Assert.Empty(newItems);
            });
        }

        [Fact]
        public async Task Duplicate_ListDoesNotExists_Exception()
        {
            var userId = (await CreateUser()).Id;
            LoginAs(userId);
            var id = Guid.NewGuid();//(await CreateList("liste1")).Id;

            await Assert.ThrowsAsync<ArgumentException>(() => _listAppService.Duplicate(new DuplicateListDto
            {
                Name = "liste2",
                ExistingListId = id
            }));
        }

        [Fact]
        public async Task Duplicate_Can_NotMineButShared()
        {
            var userId = (await CreateRandomUser()).Id;
            var currentUserId = (await CreateRandomUser()).Id;

            LoginAs(userId);
            var listId = (await CreateList("liste1")).Id;
            await CreateItem(listId, "item1");

            await CreateShare(listId, currentUserId);

            LoginAs(currentUserId);
            var listDto = await _listAppService.Duplicate(new DuplicateListDto
            {
                Name = "new liste",
                ExistingListId = listId
            });
            Assert.NotNull(listDto);
            Assert.NotEqual(listId, listDto.Id);
            Assert.Equal(currentUserId, listDto.UserId);

            await UseDbContextAsync(async context =>
            {
                var list = await context.Lists.IgnoreQueryFilters().Include(l => l.Items).SingleOrDefaultAsync(l => l.Id == listDto.Id);
                Assert.NotNull(list);
                Assert.NotNull(list.Items);
                Assert.NotEmpty(list.Items);
                Assert.Equal(1, list.Items.Count);
            });
        }

        [Fact]
        public async Task Duplicate_Cannot_NotMineNotShared()
        {
            var userId = (await CreateRandomUser()).Id;
            var currentUserId = (await CreateRandomUser()).Id;

            LoginAs(userId);
            var listId = (await CreateList("liste1")).Id;
            await CreateItem(listId, "item1");

            //await CreateShare(listId, currentUserId);

            LoginAs(currentUserId);
            await Assert.ThrowsAnyAsync<Exception>(() => _listAppService.Duplicate(new DuplicateListDto
            {
                Name = "new liste",
                ExistingListId = listId
            }));
        }

        [Fact]
        public async Task Delete_Cannot_ListNoMine_Exception()
        {
            var userId = (await CreateUser()).Id;
            var currentUserId = (await CreateUser(userName: "test@toto.com")).Id;
            LoginAs(userId);

            var listId = (await CreateList("liste1")).Id;

            LoginAs(currentUserId);
            await Assert.ThrowsAsync<ArgumentException>(() => _listAppService.Delete(listId));
        }

        [Fact]
        public async Task Delete_Cannot_EvenListShareToMe_Exception()
        {
            var userId = (await CreateUser()).Id;
            var currentUserId = (await CreateUser(userName: "test@toto.com")).Id;
            LoginAs(userId);

            var listId = (await CreateList("liste1")).Id;

            await CreateShare(listId, currentUserId);

            LoginAs(currentUserId);
            await Assert.ThrowsAsync<ArgumentException>(() => _listAppService.Delete(listId));
        }

        [Fact]
        public async Task Update_Cannot_NotMine_Exception()
        {
            var userId = (await CreateUser()).Id;
            var currentUserId = (await CreateUser(userName: "test@toto.com")).Id;
            LoginAs(userId);

            var listId = (await CreateList("liste1")).Id;

            LoginAs(currentUserId);
            await Assert.ThrowsAsync<ArgumentException>(() => _listAppService.Update(new UpdateListDto
            {
                Id = listId,
                Name = "toto"
            }));
        }

        [Fact]
        public async Task Update_Cannot_EvenListShareToMe_Exception()
        {
            var userId = (await CreateUser()).Id;
            var currentUserId = (await CreateUser(userName: "test@toto.com")).Id;
            LoginAs(userId);

            var listId = (await CreateList("liste1")).Id;

            await CreateShare(listId, currentUserId);

            LoginAs(currentUserId);
            await Assert.ThrowsAsync<ArgumentException>(() => _listAppService.Update(new UpdateListDto
            {
                Id = listId,
                Name = "toto"
            }));
        }

        [Fact]
        public async Task Get_Cannot_NotMine_Null()
        {
            var userId = (await CreateUser()).Id;
            var currentUserId = (await CreateUser(userName: "test@toto.com")).Id;
            LoginAs(userId);

            var listId = (await CreateList("liste1")).Id;

            LoginAs(currentUserId);
            var listDto = await _listAppService.Get(listId);
            Assert.Null(listDto);
        }

        [Fact]
        public async Task Get_Can_ShareWithMe()
        {
            var userId = (await CreateUser()).Id;
            var currentUserId = (await CreateUser(userName: "test@toto.com")).Id;
            LoginAs(userId);

            var listId = (await CreateList("liste1")).Id;

            await CreateShare(listId, currentUserId);

            LoginAs(currentUserId);
            var listDto = await _listAppService.Get(listId);
            Assert.NotNull(listDto);
        }

        [Fact]
        public async Task Get_WithUserInfo()
        {
            var userId = (await CreateUser()).Id;
            LoginAs(userId);

            var listId = (await CreateList("liste1")).Id;

            var listDto = await _listAppService.Get(listId);
            Assert.NotNull(listDto);
            Assert.NotNull(listDto.User);
            Assert.Equal(userId, listDto.User.Id);
        }

        [Fact]
        public async Task Get_SharedList_WithUserInfo()
        {
            var userId = (await CreateUser()).Id;
            var currentUserId = (await CreateUser(userName: "test@toto.com")).Id;
            LoginAs(userId);

            var listId = (await CreateList("liste1")).Id;

            await CreateShare(listId, currentUserId);

            LoginAs(currentUserId);
            var listDto = await _listAppService.Get(listId);
            Assert.NotNull(listDto);
            Assert.NotNull(listDto.User);
            Assert.Equal(userId, listDto.User.Id);
        }

        [Fact]
        public async Task GetAll_OnlyMine()
        {
            var userId = (await CreateUser()).Id;
            var currentUserId = (await CreateUser(userName: "test@toto.com")).Id;
            LoginAs(userId);

            await CreateList("liste1");

            LoginAs(currentUserId);
            var listId2 = (await CreateList("liste2")).Id;

            var listDto = await _listAppService.GetAll();
            Assert.NotNull(listDto);
            Assert.Single(listDto);
            Assert.Contains(listDto, dto => dto.Id == listId2);
        }

        [Fact]
        public async Task GetAll_SharedLists_WithInfos()
        {
            var userId = (await CreateUser()).Id;
            var currentUserId = (await CreateUser(userName: "test@toto.com")).Id;
            LoginAs(userId);

            var listId = (await CreateList("liste1")).Id;

            await CreateShare(listId, currentUserId);

            LoginAs(currentUserId);
            var listId2 = (await CreateList("liste2")).Id;

            var listDto = await _listAppService.GetAll(new GetAllListsDto { LoadShares = true });
            Assert.NotNull(listDto);
            Assert.Equal(2, listDto.Count);
            Assert.Contains(listDto, dto => dto.Id == listId2);
            Assert.Contains(listDto, dto => dto.Id == listId);
            Assert.True(listDto.All(l => l.User != null));
            Assert.True(listDto.Count(l => l.Id == listId && l.User.Id == userId) == 1);
            Assert.True(listDto.Count(l => l.Id == listId2 && l.User.Id == currentUserId) == 1);
        }

        [Fact]
        public async Task GetAll_WithInfos()
        {
            var userId = (await CreateUser()).Id;
            LoginAs(userId);

            await CreateList("liste1");
            await CreateList("liste2");

            var listDto = await _listAppService.GetAll();
            Assert.NotNull(listDto);
            Assert.Equal(2, listDto.Count);
            Assert.True(listDto.All(l => l.User != null && l.User.Id == userId));
        }

        [Fact]
        public async Task Share_Cannot_ListNotMine()
        {
            var userId = (await CreateRandomUser()).Id;
            var currentUserId = (await CreateRandomUser()).Id;
            var externalUserId = (await CreateRandomUser()).Id;

            LoginAs(userId);
            var listId = (await CreateList("liste1")).Id;

            LoginAs(currentUserId);
            await Assert.ThrowsAsync<ArgumentException>(() => _listAppService.Share(new ShareListDto
            {
                ListId = listId,
                UserName = ""
            }));
        }

        [Fact]
        public async Task Share_Cannot_UserDoesNotExists()
        {
            var userId = (await CreateRandomUser()).Id;
            _mockUserStore.Setup(c => c.FindByEmailAsync(It.IsAny<string>(), CancellationToken.None)).ReturnsAsync(() => null);

            LoginAs(userId);
            var listId = (await CreateList("liste1")).Id;

            await Assert.ThrowsAsync<ArgumentException>(() => _listAppService.Share(new ShareListDto
            {
                ListId = listId,
                UserName = "tet@test.com"
            }));
        }

        [Fact]
        public async Task Share_Can_ListIsMine()
        {
            var userId = (await CreateRandomUser()).Id;
            var user = await CreateRandomUser();
            var currentUserId = user.Id;
            _mockUserStore.Setup(c => c.FindByEmailAsync(user.Email, CancellationToken.None)).ReturnsAsync(() => user);

            LoginAs(userId);
            var listId = (await CreateList("liste1")).Id;

            await _listAppService.Share(new ShareListDto
            {
                ListId = listId,
                UserName = user.UserName
            });

            await UseDbContextAsync(async context => Assert.True(await context.Shares.IgnoreQueryFilters()
                .AnyAsync(c => c.ListId == listId && c.UserId == currentUserId)));
        }
    }
}
