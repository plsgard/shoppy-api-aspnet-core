using System;
using System.Linq;
using Shoppy.Application.Commons;
using Shoppy.Application.Items.Dtos;
using Shoppy.Core.Data;
using Shoppy.Core.Items;

namespace Shoppy.Application.Items
{
    public class ItemAppService : AppService<Item, ItemDto, Guid, CreateItemDto, UpdateItemDto, GetAllItemsDto>, IItemAppService
    {
        public ItemAppService(IRepository<Item, Guid> repository) : base(repository)
        {
        }

        protected override IQueryable<Item> CreateFilteredQuery(GetAllItemsDto input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            var filteredQuery = base.CreateFilteredQuery(input);
            return input.ListId.HasValue ? filteredQuery.Where(i => i.ListId == input.ListId.Value) : filteredQuery;
        }
    }
}