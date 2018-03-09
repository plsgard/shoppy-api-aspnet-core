using System.Net;
using Microsoft.AspNetCore.Mvc;
using Shoppy.Api.Models;

namespace Shoppy.Api.Controllers
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}")]
    public class ApiController : Controller
    {
        /// <summary>
        /// Retrieves API informations, like version and release date.
        /// </summary>
        /// <returns>An API informations object.</returns>
        /// <response code="200">Returns API informations with version and release date.</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiVersionDto), (int)HttpStatusCode.OK)]
        public ApiVersionDto Get()
        {
            return ApiVersionDto.GetApiVersionDto(HttpContext.GetRequestedApiVersion());
        }
    }
}