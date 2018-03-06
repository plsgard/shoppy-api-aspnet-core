using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shoppy.Application.Lists;
using Shoppy.Application.Lists.Dtos;

namespace Shoppy.Api.Controllers
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class ListsController : Controller
    {
        private readonly IListAppService _listAppService;

        public ListsController(IListAppService listAppService)
        {
            _listAppService = listAppService;
        }

        /// <summary>
        /// Get all shopping lists.
        /// </summary>
        /// <returns>A list of shopping lists.</returns>
        /// <response code="200">Returns the list of all shopping lists.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IList<ListDto>), (int)HttpStatusCode.OK)]
        public async Task<IList<ListDto>> Get()
        {
            return await _listAppService.GetAll();
        }

        /// <summary>
        /// Gets a specific shopping list by its unique id.
        /// </summary>
        /// <param name="id">The unique id of the list.</param>
        /// <returns>The list with provided id.</returns>
        /// <response code="200">Returns the list with provided id.</response>
        /// <response code="404">If the list does not exists.</response>            
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ListDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get(Guid id)
        {
            var listDto = await _listAppService.Get(id);
            if (listDto == null)
                return NotFound(id);
            return Ok(listDto);
        }

        /// <summary>
        /// Creates a new shopping list.
        /// </summary>
        /// <param name="value">The list to create.</param>
        /// <returns>A newly-created list with its unique id.</returns>
        /// <response code="201">Returns the newly-created list.</response>
        /// <response code="400">If the list is null or not valid.</response>            
        [HttpPost]
        [ProducesResponseType(typeof(ListDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post([FromBody]CreateListDto value)
        {
            if (value == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            var listDto = await _listAppService.Create(value);
            return CreatedAtAction(nameof(Get), new { id = listDto.Id }, listDto);
        }

        /// <summary>
        /// Updates an existing shopping list.
        /// </summary>
        /// <param name="id">ID of the list to update.</param>
        /// <param name="value">An list with the values to update.</param>
        /// <returns>Returns the newly-updated list.</returns>
        /// <response code="200">Returns the newly-updated list.</response>            
        /// <response code="400">If the list is null or if the provided id and the id of the list to update do not match, or if the list to update is not valid.</response>            
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ListDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Put(Guid id, [FromBody]ListDto value)
        {
            if (value == null || value.Id != id || !ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(await _listAppService.Update(value));
        }

        /// <summary>
        /// Deletes a specific shopping list.
        /// </summary>
        /// <param name="id">ID of the list to delete.</param>
        /// <returns>No content result.</returns>
        /// <response code="204">No content result.</response>            
        [HttpDelete("{id:guid}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _listAppService.Delete(id);
            return NoContent();
        }
    }
}
