using System;
using System.Threading.Tasks;
using Shoppy.Core.Data;

namespace Shoppy.Core.Items
{
    public interface IItemRepository : IRepository<Item, Guid>
    {
        Task DuplicateOnList(Guid originalListId, Guid newListId);
        Task<int> GetMaxIndexForList(Guid listId);
    }
}
