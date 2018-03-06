using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shoppy.Application.Authentication.Dtos;
using Shoppy.Application.Users;
using Shoppy.Application.Users.Dtos;

namespace Shoppy.Api.Controllers
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AccountsController : Controller
    {
        private readonly IUserAppService _userAppService;

        public AccountsController(IUserAppService userAppService)
        {
            _userAppService = userAppService;
        }

        /// <summary>
        /// Creates a new user with new account.
        /// </summary>
        /// <param name="model">The user to create.</param>
        /// <returns>A newly-created user with its unique id.</returns>
        /// <response code="201">Returns the newly-created user.</response>
        /// <response code="400">If the user is null or not valid.</response>            
        [HttpPost]
        [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (model == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            var userDto = await _userAppService.Register(model);
            return CreatedAtAction("Get", "Users", new { id = userDto.Id }, userDto);
        }
    }
}
