using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppy.Core.Data
{
    public interface IRepository<TEntity, TPrimaryKey>
    {
        Task<TEntity> GetByIdAsync(TPrimaryKey key);
        IQueryable<TEntity> GetAll();
        Task<IList<TEntity>> GetAllListAsync();
        Task<TEntity> AddAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task DeleteAsync(TPrimaryKey key);
    }
}
