﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shoppy.Core.Commons;
using Shoppy.Core.Data;
using Shoppy.Utils.Enumerable;

namespace Shoppy.Application.Commons
{
    public abstract class AppService<TEntity, TEntityDto, TPrimaryKey, TCreateEntityDto, TUpdateEntityDto> : BaseAppService<TEntity, TEntityDto>, IAppService<TEntityDto, TPrimaryKey, TCreateEntityDto, TUpdateEntityDto> where TEntity : IEntity<TPrimaryKey>
    {
        protected IRepository<TEntity, TPrimaryKey> Repository { get; }

        protected AppService(IRepository<TEntity, TPrimaryKey> repository)
        {
            Repository = repository;
        }
        
        public virtual async Task<IList<TEntityDto>> GetAll()
        {
            return ObjectMapper.Map<List<TEntityDto>>(await Repository.GetAllListAsync());
        }

        public virtual async Task<TEntityDto> Get(TPrimaryKey id)
        {
            return ToDto(await Repository.GetByIdAsync(id));
        }

        public virtual async Task<TEntityDto> Create(TCreateEntityDto input)
        {
            Normalize(input);
            Validate(input);

            TEntity entity = ToEntity(input);
            return ToDto(await Repository.AddAsync(entity));
        }

        public virtual async Task<TEntityDto> Update(TUpdateEntityDto input)
        {
            Normalize(input);
            Validate(input);

            TEntity entity = ToEntity(input);
            return ToDto(await Repository.UpdateAsync(entity));
        }

        public virtual async Task Delete(TPrimaryKey id)
        {
            await Repository.DeleteAsync(id);
        }
    }

    public abstract class AppService<TEntity, TEntityDto, TPrimaryKey, TCreateEntityDto, TUpdateEntityDto, TGetAllDto> : AppService<TEntity, TEntityDto, TPrimaryKey, TCreateEntityDto, TUpdateEntityDto>, IAppService<TEntityDto, TPrimaryKey, TCreateEntityDto, TUpdateEntityDto, TGetAllDto> where TEntity : IEntity<TPrimaryKey>
    {
        protected AppService(IRepository<TEntity, TPrimaryKey> repository) : base(repository)
        {
        }

        public virtual Task<IList<TEntityDto>> GetAll(TGetAllDto input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            Normalize(input);
            Validate(input);

            var query = CreateFilteredQuery(input);
            query = ApplySorting(query, input);

            return Task.FromResult<IList<TEntityDto>>(ObjectMapper.Map<List<TEntityDto>>(query.ToList()));
        }

        private IOrderedQueryable<TEntity> ApplySorting(IQueryable<TEntity> query, TGetAllDto input)
        {
            if (input is ISorted sorted) return sorted.Sorting.EndsWith("DESC", StringComparison.InvariantCultureIgnoreCase) ? query.OrderByDescending(sorted.Sorting.Replace("DESC", string.Empty, StringComparison.InvariantCultureIgnoreCase).Trim()) : query.OrderBy(sorted.Sorting.Replace("ASC", string.Empty, StringComparison.InvariantCultureIgnoreCase).Trim());
            return query.OrderBy(c => c.Id);
        }

        protected virtual IQueryable<TEntity> CreateFilteredQuery(TGetAllDto input)
        {
            return Repository.GetAll();
        }
    }
}