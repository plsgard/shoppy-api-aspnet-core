using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            var userId = (await CreateUser()).Id;
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
    }
}
