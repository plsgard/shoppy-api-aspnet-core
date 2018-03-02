using System;
using AutoMapper;
using Shoppy.Application.Commons;
using Shoppy.Application.Lists.Dtos;
using Shoppy.Core.Data;
using Shoppy.Core.Lists;

namespace Shoppy.Application.Lists
{
    public class ListAppService : AppService<List, ListDto, Guid, CreateListDto, ListDto>, IListAppService
    {
        public ListAppService(IRepository<List, Guid> repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}