using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public async Task<int> GetMaxIndexForList(Guid listId)
        {
            return await DbSet.AnyAsync(l => l.ListId == listId) ? await DbSet.Where(l => l.ListId == listId).MaxAsync(l => l.Index) : 0;
        }

        public override IQueryable<Item> GetAll()
        {
            // need to refilter userid on getAll because must use IgnoreQueryFilter on Share who disable for all queryable, even GetAll()
            return Context.CurrentUserId.HasValue
                ? base.GetAll().Where(l => l.List.UserId == Context.CurrentUserId.Value)
                    .Union(
                        from item in DbSet.IgnoreQueryFilters()
                        join share in Context.Shares.IgnoreQueryFilters()
                            on item.ListId equals share.ListId
                        where share.UserId == Context.CurrentUserId.Value
                        select item)
                : base.GetAll();
        }
    }
}
