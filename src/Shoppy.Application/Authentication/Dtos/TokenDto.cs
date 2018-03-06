using System;
using Shoppy.Application.Commons;

namespace Shoppy.Application.Authentication.Dtos
{
    public class TokenDto : EntityDto<string>
    {
        public string Token { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
