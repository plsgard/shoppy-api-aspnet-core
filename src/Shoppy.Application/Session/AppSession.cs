using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Shoppy.Core.Session;

namespace Shoppy.Application.Session
{
    public class AppSession : IAppSession
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppSession(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? GetCurrentUserId()
        {
            var value = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return !string.IsNullOrEmpty(value) ? Guid.Parse(value) : (Guid?) null;
        }
    }
}
