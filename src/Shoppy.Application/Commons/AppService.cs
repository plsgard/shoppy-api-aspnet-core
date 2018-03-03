using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Shoppy.Core.Data;

namespace Shoppy.Application.Commons
{
    public class AppService<TEntity, TEntityDto, TPrimaryKey, TCreateEntityDto, TUpdateEntityDto> : IAppService<TEntityDto, TPrimaryKey, TCreateEntityDto, TUpdateEntityDto>
    {
        protected IMapper ObjectMapper => Mapper.Instance;
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

        public async Task<IList<TEntityDto>> GetAll()
        {
            return ObjectMapper.Map<List<TEntityDto>>(await Repository.GetAllListAsync());
        }

        public async Task<TEntityDto> Get(TPrimaryKey id)
        {
            return ToDto(await Repository.GetByIdAsync(id));
        }

        public async Task<TEntityDto> Create(TCreateEntityDto input)
        {
            TEntity entity = ToEntity(input);
            return ToDto(await Repository.AddAsync(entity));
        }

        public async Task<TEntityDto> Update(TUpdateEntityDto input)
        {
            TEntity entity = ToEntity(input);
            return ToDto(await Repository.UpdateAsync(entity));
        }

        public async Task Delete(TPrimaryKey id)
        {
            await Repository.DeleteAsync(id);
        }
    }
}