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
            //return Context.CurrentUserId.HasValue ?
            //    GetAll().Where(l => l.UserId == Context.CurrentUserId.Value)
            //        .Union(
            //            Context.Shares.Include(l => l.User).IgnoreQueryFilters().Where(s => s.UserId == Context.CurrentUserId.Value).Select(s => s.List)
            //        )
            //    : GetAll();
            return Context.CurrentUserId.HasValue ?
                (from mine in GetAll()
                 where mine.UserId == Context.CurrentUserId.Value
                 select mine)
                .Union(
                from shared in Context.Shares.IgnoreQueryFilters().Include(l => l.List).ThenInclude(l => l.User)
                where shared.UserId == Context.CurrentUserId.Value
                select new List
                {
                    Id = shared.List.Id,
                    Name = shared.List.Name,
                    UserId = shared.List.UserId,
                    User = shared.List.User,
                    ModificationUserId = shared.List.ModificationUserId,
                    CreationTime = shared.List.CreationTime,
                    CreationUserId = shared.List.CreationUserId,
                    ModificationTime = shared.List.ModificationTime
                }
                )
                : GetAll();
        }

        public async Task AddShareAsync(Share share)
        {
            if (share == null) throw new ArgumentNullException(nameof(share));

            await Context.Shares.AddAsync(share);
            await Context.SaveChangesAsync();
        }

        public override IQueryable<List> GetAll()
        {
            return GetAllIncluding(l => l.User);
        }
    }
}
