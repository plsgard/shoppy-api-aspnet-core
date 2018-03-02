using System;
using Shoppy.Application.Commons;
using Shoppy.Application.Items.Dtos;

namespace Shoppy.Application.Items
{
    public interface IItemAppService : IAppService<ItemDto, Guid, CreateItemDto, ItemDto>
    {
    }
}
