using System;
using System.Linq;
using System.Threading.Tasks;
using Shoppy.Core.Items;

namespace Shoppy.Data.Repositories
{
    public class ItemRepository : Repository<Item, Guid>, IItemRepository
    {
        public ItemRepository(ShoppyContext context) : base(context)
        {
        }

        public async Task DuplicateOnList(Guid originalListId, Guid newListId)
        {
            // maybe not the cleanest way, but it works and it can be easily tested
            await Context.AddRangeAsync(GetAll().Where(c => c.ListId == originalListId).Select(i => new Item
            {
                Name = i.Name,
                Index = i.Index,
                ListId = newListId,
                Picked = false
            }));
            await Context.SaveChangesAsync();
            //var itemsTable = $"dbo.{nameof(Context.Items)}";
            //var currentUserIdText = Context.CurrentUserId.HasValue
            //    ? string.Format("'{0}'", Context.CurrentUserId.Value)
            //    : "NULL";
            //var rawSqlString = $"INSERT INTO {itemsTable} ({nameof(Item.Id)}, {nameof(Item.ListId)}, {nameof(Item.Index)}, {nameof(Item.Picked)}, {nameof(Item.CreationTime)}, {nameof(Item.CreationUserId)}, {nameof(Item.UserId)}) " +
            //                   $"SELECT NEWID(), '{newListId}', {nameof(Item.Index)}, 0, '{DateTimeOffset.UtcNow}', {currentUserIdText}, {currentUserIdText} " +
            //                   $"FROM {itemsTable} " +
            //                   $"WHERE {nameof(Item.ListId)} = '{originalListId}'";
            //await Context.Database.ExecuteSqlCommandAsync(rawSqlString);
        }
    }
}
