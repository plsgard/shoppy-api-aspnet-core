using System;
using System.Threading.Tasks;
using Shoppy.Application.Commons;
using Shoppy.Application.Lists.Dtos;

namespace Shoppy.Application.Lists
{
    public interface IListAppService : IAppService<ListDto, Guid, CreateListDto, UpdateListDto>
    {
        Task<ListDto> Duplicate(DuplicateListDto input);
    }
}
