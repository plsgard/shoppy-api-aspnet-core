using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        /// Gets a specific user account by its unique id.
        /// </summary>
        /// <param name="id">The unique id of the user.</param>
        /// <returns>The user with provided id.</returns>
        /// <response code="200">Returns the user with provided id.</response>
        /// <response code="404">If the user does not exists.</response>            
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(typeof(UserDto), 404)]
        public async Task<IActionResult> Get(Guid id)
        {
            var userDto = await _userAppService.Get(id);
            if (userDto == null)
                return NotFound();
            return Ok(userDto);
        }

        /// <summary>
        /// Creates a new user with new account.
        /// </summary>
        /// <param name="model">The user to create.</param>
        /// <returns>A newly-created user with its unique id.</returns>
        /// <response code="201">Returns the newly-created user.</response>
        /// <response code="400">If the user is null or not valid.</response>            
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] CreateUserDto model)
        {
            if (model == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            var userDto = await _userAppService.Create(model);
            return CreatedAtAction(nameof(Get), new { id = userDto.Id }, userDto);
        }
    }
}
