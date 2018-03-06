using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shoppy.Application.Users;
using Shoppy.Application.Users.Dtos;

namespace Shoppy.Api.Controllers
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class UsersController : Controller
    {
        private readonly IUserAppService _userAppService;

        public UsersController(IUserAppService userAppService)
        {
            _userAppService = userAppService;
        }

        /// <summary>
        /// Gets a specific user by its unique id.
        /// </summary>
        /// <param name="id">The unique id of the user.</param>
        /// <returns>The user with provided id.</returns>
        /// <response code="200">Returns the user with provided id.</response>
        /// <response code="404">If the user does not exists.</response>            
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(typeof(UserDto), 404)]
        public async Task<IActionResult> Get(Guid id)
        {
            var userDto = await _userAppService.Get(id);
            if (userDto == null)
                return NotFound();
            return Ok(userDto);
        }
    }
}
