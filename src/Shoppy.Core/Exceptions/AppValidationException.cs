using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Shoppy.Core.Exceptions
{
    public class AppValidationException : ValidationException
    {
        public List<ValidationResult> ValidationResults { get; }

        public AppValidationException(List<ValidationResult> validationResults)
        {
            ValidationResults = validationResults;
        }

        public override string ToString()
        {
            return "Model is not valid : " + string.Join(", ", ValidationResults.Select(s => s.ErrorMessage).ToArray());
        }
    }
}
