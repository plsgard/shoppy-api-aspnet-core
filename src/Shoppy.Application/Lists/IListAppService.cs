using System;
using Shoppy.Application.Commons;
using Shoppy.Application.Lists.Dtos;

namespace Shoppy.Application.Lists
{
    public interface IListAppService : IAppService<ListDto, Guid, CreateListDto, ListDto>
    {
    }
}
