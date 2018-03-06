using System;
using Shoppy.Application.Commons;

namespace Shoppy.Application.Authentication.Dtos
{
    /// <summary>
    /// A JWT token for Bearer authentication.
    /// </summary>
    public class TokenDto : EntityDto<string>
    {
        /// <summary>
        /// The token value.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Expiration date of the token.
        /// </summary>
        public DateTime ExpirationDate { get; set; }
    }
}
