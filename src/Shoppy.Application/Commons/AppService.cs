using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shoppy.Core.Commons;
using Shoppy.Core.Data;
using Shoppy.Utils.Enumerable;

namespace Shoppy.Application.Commons
{
    public abstract class AppService<TEntity, TEntityDto, TPrimaryKey, TCreateEntityDto, TUpdateEntityDto> : BaseAppService<TEntity, TEntityDto>, IAppService<TEntityDto, TPrimaryKey, TCreateEntityDto, TUpdateEntityDto> where TEntity : IEntity<TPrimaryKey> where TEntityDto : IEntityDto<TPrimaryKey> where TUpdateEntityDto : IEntityDto<TPrimaryKey>
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

            TEntity entity = await Repository.GetByIdAsync(input.Id);
            if (entity == null)
                throw new ArgumentException($"No entity with id '{input.Id}'.", nameof(input.Id));
            TEntity entityToUpdate = ObjectMapper.Map(input, entity); //ToEntity(input);
            return ToDto(await Repository.UpdateAsync(entityToUpdate));
        }

        public virtual async Task Delete(TPrimaryKey id)
        {
            await Repository.DeleteAsync(id);
        }
    }

    public abstract class AppService<TEntity, TEntityDto, TPrimaryKey, TCreateEntityDto, TUpdateEntityDto, TGetAllDto> : AppService<TEntity, TEntityDto, TPrimaryKey, TCreateEntityDto, TUpdateEntityDto>, IAppService<TEntityDto, TPrimaryKey, TCreateEntityDto, TUpdateEntityDto, TGetAllDto> where TEntity : IEntity<TPrimaryKey> where TEntityDto : IEntityDto<TPrimaryKey> where TUpdateEntityDto : IEntityDto<TPrimaryKey>
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
            if (input is ISorted sorted) return sorted.SortType == SortType.DESC ? query.OrderByDescending(sorted.SortProperty) : query.OrderBy(sorted.SortProperty);
            return query.OrderBy(c => c.Id);
        }

        protected virtual IQueryable<TEntity> CreateFilteredQuery(TGetAllDto input)
        {
            return Repository.GetAll();
        }
    }
}