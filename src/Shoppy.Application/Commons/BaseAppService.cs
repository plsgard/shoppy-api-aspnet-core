using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Shoppy.Core.Exceptions;
using Shoppy.Core.Validation;

namespace Shoppy.Application.Commons
{
    public abstract class BaseAppService<TEntity, TEntityDto>
    {
        protected IMapper ObjectMapper => Mapper.Instance;

        protected virtual TEntityDto ToDto(TEntity entity)
        {
            if (entity == null) return default(TEntityDto);

            return ObjectMapper.Map<TEntityDto>(entity);
        }

        protected virtual TEntity ToEntity<TDto>(TDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return ObjectMapper.Map<TEntity>(dto);
        }

        protected void Normalize(object input)
        {
            if (input == null)
                return;

            if (input is IShouldNormalize normalize)
                normalize.Normalize();
        }

        protected virtual void Validate(object input)
        {
            var context = new System.ComponentModel.DataAnnotations.ValidationContext(input);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(input, context, results);
            if (!isValid)
                throw new AppValidationException(results);
        }
    }
}
