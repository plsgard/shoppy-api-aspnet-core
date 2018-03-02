﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shoppy.Core.Data;

namespace Shoppy.Data.Repositories
{
    public class Repository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey> where TEntity : class
    {
        protected ShoppyContext Context { get; }

        protected DbSet<TEntity> DbSet { get; }

        public Repository(ShoppyContext context)
        {
            Context = context;
            DbSet = Context.Set<TEntity>();
        }

        public virtual async Task<TEntity> GetByIdAsync(TPrimaryKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            return await DbSet.FindAsync(key);
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            return DbSet;
        }

        public virtual async Task<IList<TEntity>> GetAllListAsync()
        {
            return await GetAll().ToListAsync();
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            await DbSet.AddAsync(entity);
            await Context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            DbSet.Update(entity);
            await Context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task DeleteAsync(TPrimaryKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var entity = await DbSet.FindAsync(key);
            if (entity == null)
                throw new ArgumentException($"No entity with key '{key}'.", nameof(key));
            DbSet.Remove(entity);
            await Context.SaveChangesAsync();
        }
    }
}
