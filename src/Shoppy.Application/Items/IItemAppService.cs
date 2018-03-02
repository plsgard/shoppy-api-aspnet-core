using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shoppy.Application.Items
{
    public interface IItemAppService<TEntityDto, TPrimaryKey, TCreateEntityDto, TUpdateEntityDto>
    {
        Task<IList<TEntityDto>> GetAll();

        Task<TEntityDto> Get(TPrimaryKey id);

        Task<TEntityDto> Create(TCreateEntityDto input);

        Task<TEntityDto> Update(TUpdateEntityDto input);
    }
}
