using System;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Shoppy.Api.Controllers
{
    public class BaseAppController : Controller
    {
        protected IMapper ObjectMapper => Mapper.Instance;

        protected virtual Guid? GetCurrentUserId()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
                return null;
            var value = HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return !string.IsNullOrWhiteSpace(value) ? Guid.Parse(value) : (Guid?)null;
        }
    }
}