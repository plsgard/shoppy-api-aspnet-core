using System;

namespace Shoppy.Application.Authentication.Dtos
{
    public class TokenDto
    {
        public string Token { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
