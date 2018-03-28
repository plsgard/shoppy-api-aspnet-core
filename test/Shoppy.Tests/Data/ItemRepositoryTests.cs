using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shoppy.Data.Repositories;
using Xunit;

namespace Shoppy.Tests.Data
{
    public class ItemRepositoryTests : AppTestBase
    {
        private readonly ItemRepository _itemRepository;

        public ItemRepositoryTests()
        {
            _itemRepository = new ItemRepository(Context);
        }

        [Fact]
        public async Task Duplicate_WithItems_AllExistingAndNotPicked()
        {
            var userId = (await CreateRandomUser()).Id;
            LoginAs(userId);
            var id = (await CreateList("liste1")).Id;
            var item1 = await CreateItem(id, "item1", picked: true);
            var item2 = await CreateItem(id, "item2");
            var item3 = await CreateItem(id, "item3", picked: true);

            var newListId = (await CreateList("liste2")).Id;
            await _itemRepository.DuplicateOnList(id, newListId);

            await UseDbContextAsync(async context =>
            {
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
        public async Task Duplicate_WithoutItems_NoError()
        {
            var userId = (await CreateRandomUser()).Id;
            LoginAs(userId);
            var id = (await CreateList("liste1")).Id;

            var newListId = (await CreateList("liste2")).Id;
            await _itemRepository.DuplicateOnList(id, newListId);

            await UseDbContextAsync(async context =>
            {
                var newItems = await context.Items.Where(i => i.ListId == newListId).ToListAsync();
                Assert.NotNull(newItems);
                Assert.Empty(newItems);
            });
        }

        [Fact]
        public async Task GetAllIncludingShares_FromSharedListOnly()
        {
            var userId1 = (await CreateRandomUser()).Id;
            var currentUser = (await CreateRandomUser()).Id;

            LoginAs(userId1);

            var listId1 = (await CreateList("liste1")).Id;
            var itemId = (await CreateItem(listId1, "item1")).Id;

            await CreateShare(listId1, currentUser);

            LoginAs(currentUser);

            var itemDtos = await _itemRepository.GetAll().ToListAsync();
            Assert.NotNull(itemDtos);
            Assert.NotEmpty(itemDtos);
            Assert.Single(itemDtos);
            Assert.Contains(itemDtos, dto => dto.Id == itemId);
        }

        [Fact]
        public async Task GetAllIncludingShares_FromMine_And_SharedList_OnOneList_NotBeingTheListUser()
        {
            var userId1 = (await CreateRandomUser()).Id;
            var currentUser = (await CreateRandomUser()).Id;

            LoginAs(userId1);

            var listId1 = (await CreateList("liste1")).Id;
            var itemId = (await CreateItem(listId1, "item1")).Id;

            await CreateShare(listId1, currentUser);

            LoginAs(currentUser);
            var item2Id = (await CreateItem(listId1, "item2")).Id;

            var itemDtos = await _itemRepository.GetAll().ToListAsync();
            Assert.NotNull(itemDtos);
            Assert.NotEmpty(itemDtos);
            Assert.Equal(2, itemDtos.Count);
            Assert.Contains(itemDtos, dto => dto.Id == itemId);
            Assert.Contains(itemDtos, dto => dto.Id == item2Id);
        }

        [Fact]
        public async Task GetAllIncludingShares_FromMine_And_SharedList_OnOneList_BeingTheListUser()
        {
            var userId1 = (await CreateRandomUser()).Id;
            var currentUser = (await CreateRandomUser()).Id;

            LoginAs(userId1);

            var listId1 = (await CreateList("liste1")).Id;
            var itemId = (await CreateItem(listId1, "item1")).Id;

            await CreateShare(listId1, currentUser);

            LoginAs(currentUser);
            var item2Id = (await CreateItem(listId1, "item2")).Id;

            LoginAs(userId1);
            var itemDtos = await _itemRepository.GetAll().ToListAsync();
            Assert.NotNull(itemDtos);
            Assert.NotEmpty(itemDtos);
            Assert.Equal(2, itemDtos.Count);
            Assert.Contains(itemDtos, dto => dto.Id == itemId);
            Assert.Contains(itemDtos, dto => dto.Id == item2Id);
        }
    }
}
