using System;
using System.Linq;
using System.Threading.Tasks;
using Shoppy.Core.Data;
using Shoppy.Core.Shares;

namespace Shoppy.Core.Lists
{
    public interface IListRepository : IRepository<List, Guid>
    {
        IQueryable<List> GetAllIncludingShares();
        Task AddShareAsync(Share share);
    }
}
