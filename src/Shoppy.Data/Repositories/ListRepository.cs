using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shoppy.Core.Lists;

namespace Shoppy.Data.Repositories
{
    public class ListRepository : Repository<List, Guid>, IListRepository
    {
        public ListRepository(ShoppyContext context) : base(context)
        {
        }

        public IQueryable<List> GetAllIncludingShares()
        {
            // need to refilter userid on getAll because must use IgnoreQueryFilter on Share who disable for all queryable, even GetAll()
            return Context.CurrentUserId.HasValue ? GetAll().Where(l => l.UserId == Context.CurrentUserId.Value).Union(Context.Shares.IgnoreQueryFilters()
                .Where(s => s.UserId == Context.CurrentUserId.Value).Select(s => s.List)).Include(l => l.User) : GetAll();
        }
    }
}
