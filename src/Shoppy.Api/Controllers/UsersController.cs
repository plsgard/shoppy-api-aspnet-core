using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shoppy.Application.Users;
using Shoppy.Application.Users.Dtos;
using Shoppy.Core;

namespace Shoppy.Api.Controllers
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(AppConsts.Policies.UserManager)]
    public class UsersController : Controller
    {
        private readonly IUserAppService _userAppService;

        public UsersController(IUserAppService userAppService)
        {
            _userAppService = userAppService;
        }

        /// <summary>
        /// Get all users.
        /// </summary>
        /// <returns>A list of users.</returns>
        /// <response code="200">Returns the list of all users.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IList<UserDto>), (int)HttpStatusCode.OK)]
        public async Task<IList<UserDto>> Get()
        {
            return await _userAppService.GetAll();
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

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="id">ID of the user to update.</param>
        /// <param name="value">An user with the values to update.</param>
        /// <returns>Returns the newly-updated user.</returns>
        /// <response code="200">Returns newly-updated user.</response>            
        /// <response code="400">If the user is null or if the provided id and the id of the user to update do not match, or if the user to update is not valid.</response>            
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Put(Guid id, [FromBody]UpdateUserDto value)
        {
            if (value == null || value.Id != id || !ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(await _userAppService.Update(value));
        }

        /// <summary>
        /// Deletes a specific user.
        /// </summary>
        /// <param name="id">ID of the user to delete.</param>
        /// <returns>No content result.</returns>
        /// <response code="204">No content result.</response>            
        [HttpDelete("{id:guid}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _userAppService.Delete(id);
            return NoContent();
        }
    }
}
