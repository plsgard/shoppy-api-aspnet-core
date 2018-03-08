using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shoppy.Application.Commons
{
    public interface IAppService<TEntityDto, TPrimaryKey, TCreateEntityDto, TUpdateEntityDto> where TEntityDto : IEntityDto<TPrimaryKey> where TUpdateEntityDto : IEntityDto<TPrimaryKey>
    {
        Task<IList<TEntityDto>> GetAll();

        Task<TEntityDto> Get(TPrimaryKey id);

        Task<TEntityDto> Create(TCreateEntityDto input);

        Task<TEntityDto> Update(TUpdateEntityDto input);

        Task Delete(TPrimaryKey id);
    }

    public interface IAppService<TEntityDto, TPrimaryKey, TCreateEntityDto, TUpdateEntityDto, TGetAllDto> : IAppService<TEntityDto, TPrimaryKey, TCreateEntityDto, TUpdateEntityDto> where TEntityDto : IEntityDto<TPrimaryKey> where TUpdateEntityDto : IEntityDto<TPrimaryKey>
    {
        Task<IList<TEntityDto>> GetAll(TGetAllDto input);
    }
}
