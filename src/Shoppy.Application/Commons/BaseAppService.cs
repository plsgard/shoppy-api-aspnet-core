using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Shoppy.Core.Exceptions;
using Shoppy.Core.Validation;

namespace Shoppy.Application.Commons
{
    public abstract class BaseAppService
    {
        protected IMapper ObjectMapper => Mapper.Instance;

        protected void Normalize(object input)
        {
            if (input == null)
                return;

            if (input is IShouldNormalize normalize)
                normalize.Normalize();
        }

        protected void Validate(object input)
        {
            var context = new System.ComponentModel.DataAnnotations.ValidationContext(input);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(input, context, results);
            if (!isValid)
                throw new AppValidationException(results);
        }
    }
}
