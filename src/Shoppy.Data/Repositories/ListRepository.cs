using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shoppy.Core.Lists;
using Shoppy.Core.Shares;

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
            return Context.CurrentUserId.HasValue ?
                GetAll().Where(l => l.UserId == Context.CurrentUserId.Value)
                    .Union(
                        Context.Shares.IgnoreQueryFilters().Where(s => s.UserId == Context.CurrentUserId.Value).Select(s => s.List)
                    ).Include(l => l.User)
                : GetAll();
        }

        public async Task AddShareAsync(Share share)
        {
            if (share == null) throw new ArgumentNullException(nameof(share));

            await Context.Shares.AddAsync(share);
            await Context.SaveChangesAsync();
        }
    }
}
