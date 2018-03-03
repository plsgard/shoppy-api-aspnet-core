using System;
using Shoppy.Application.Commons;
using Shoppy.Application.Items.Dtos;
using Shoppy.Core.Data;
using Shoppy.Core.Items;

namespace Shoppy.Application.Items
{
    public class ItemAppService : AppService<Item, ItemDto, Guid, CreateItemDto, ItemDto>, IItemAppService
    {
        public ItemAppService(IRepository<Item, Guid> repository) : base(repository)
        {
        }
    }
}