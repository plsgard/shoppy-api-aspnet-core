using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Shoppy.Core.Exceptions
{
    public class AppIdentityResultException : Exception
    {
        private readonly IEnumerable<IdentityError> _errors;

        public AppIdentityResultException(IEnumerable<IdentityError> errors)
        {
            _errors = errors;
        }

        public override string ToString()
        {
            return String.Join(" - ", _errors.Select(c => $"{c.Code} : {c.Description}"));
        }
    }
}
