using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shoppy.Core.Commons;
using Shoppy.Core.Data;
using Shoppy.Utils.Enumerable;

namespace Shoppy.Application.Commons
{
    public class AppService<TEntity, TEntityDto, TPrimaryKey, TCreateEntityDto, TUpdateEntityDto> : BaseAppService, IAppService<TEntityDto, TPrimaryKey, TCreateEntityDto, TUpdateEntityDto> where TEntity : IEntity<TPrimaryKey>
    {
        protected IRepository<TEntity, TPrimaryKey> Repository { get; }

        public AppService(IRepository<TEntity, TPrimaryKey> repository)
        {
            Repository = repository;
        }

        protected virtual TEntityDto ToDto(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return ObjectMapper.Map<TEntityDto>(entity);
        }

        protected virtual TEntity ToEntity<TDto>(TDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return ObjectMapper.Map<TEntity>(dto);
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

    public class AppService<TEntity, TEntityDto, TPrimaryKey, TCreateEntityDto, TUpdateEntityDto, TGetAllDto> : AppService<TEntity, TEntityDto, TPrimaryKey, TCreateEntityDto, TUpdateEntityDto>, IAppService<TEntityDto, TPrimaryKey, TCreateEntityDto, TUpdateEntityDto, TGetAllDto> where TEntity : IEntity<TPrimaryKey>
    {
        public AppService(IRepository<TEntity, TPrimaryKey> repository) : base(repository)
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
            if (input is ISorted sorted) return sorted.Sorting.EndsWith("DESC") ? query.OrderByDescending(sorted.Sorting.Replace("DESC", string.Empty).Trim()) : query.OrderBy(sorted.Sorting.Replace("ASC", string.Empty).Trim());
            return query.OrderBy(c => c.Id);
        }

        protected virtual IQueryable<TEntity> CreateFilteredQuery(TGetAllDto input)
        {
            return Repository.GetAll();
        }
    }
}