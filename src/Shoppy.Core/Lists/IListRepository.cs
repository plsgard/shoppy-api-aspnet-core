using System;
using System.Linq;
using Shoppy.Core.Data;

namespace Shoppy.Core.Lists
{
    public interface IListRepository : IRepository<List, Guid>
    {
        IQueryable<List> GetAllIncludingShares();
    }
}
