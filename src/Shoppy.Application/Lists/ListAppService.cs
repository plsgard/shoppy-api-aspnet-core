using System;
using Shoppy.Application.Commons;
using Shoppy.Application.Lists.Dtos;
using Shoppy.Core.Data;
using Shoppy.Core.Lists;

namespace Shoppy.Application.Lists
{
    public class ListAppService : AppService<List, ListDto, Guid, CreateListDto, UpdateListDto>, IListAppService
    {
        public ListAppService(IRepository<List, Guid> repository) : base(repository)
        {
        }
    }
}