using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shoppy.Core.Commons;
using Shoppy.Core.Data;

namespace Shoppy.Data.Repositories
{
    public class Repository<TEntity, TPrimaryKey> : BaseRepository, IRepository<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
    {
        protected ShoppyContext Context { get; }

        protected DbSet<TEntity> DbSet { get; }

        protected Repository(ShoppyContext context)
        {
            Context = context;
            DbSet = Context.Set<TEntity>();
        }

        public virtual async Task<TEntity> GetByIdAsync(TPrimaryKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            return await GetAll().SingleOrDefaultAsync(s => s.Id.Equals(key));
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            return DbSet;
        }

        public IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] includes)
        {
            if (includes == null || !includes.Any())
                return GetAll();

            var returns = GetAll();
            foreach (var expression in includes)
            {
                returns = returns.Include(expression);
            }

            return returns;
        }

        public virtual async Task<IList<TEntity>> GetAllListAsync()
        {
            return await GetAll().ToListAsync();
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            Normalize(entity);

            await DbSet.AddAsync(entity);
            await Context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            Normalize(entity);

            DbSet.Update(entity);
            await Context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task DeleteAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            DbSet.Remove(entity);
            await Context.SaveChangesAsync();
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().AnyAsync(predicate);
        }

        public bool Any(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Any(predicate);
        }

        public void Dispose()
        {
            Context?.Dispose();
        }
    }
}
