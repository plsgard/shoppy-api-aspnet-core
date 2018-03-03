using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shoppy.Application.Items;
using Shoppy.Application.Items.Dtos;

namespace Shoppy.Api.Controllers
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ItemsController : Controller
    {
        private readonly IItemAppService _itemAppService;

        public ItemsController(IItemAppService itemAppService)
        {
            _itemAppService = itemAppService;
        }

        /// <summary>
        /// Get all shopping items.
        /// </summary>
        /// <returns>A list of shopping items.</returns>
        /// <response code="200">Returns the list of all items.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IList<ItemDto>), 200)]
        public async Task<IList<ItemDto>> Get()
        {
            return await _itemAppService.GetAll();
        }

        /// <summary>
        /// Gets a specific shopping item by its unique id.
        /// </summary>
        /// <param name="id">The unique id of the item.</param>
        /// <returns>The item with provided id.</returns>
        /// <response code="200">Returns the item with provided id.</response>
        /// <response code="404">If the item does not exists.</response>            
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ItemDto), 200)]
        [ProducesResponseType(typeof(ItemDto), 404)]
        public async Task<IActionResult> Get(Guid id)
        {
            var itemDto = await _itemAppService.Get(id);
            if (itemDto == null)
                return NotFound();
            return Ok(itemDto);
        }

        /// <summary>
        /// Creates a new shopping item.
        /// </summary>
        /// <param name="value">The item to create.</param>
        /// <returns>A newly-created item with its unique id.</returns>
        /// <response code="201">Returns the newly-created item.</response>
        /// <response code="400">If the item is null or not valid.</response>            
        [HttpPost]
        [ProducesResponseType(typeof(ItemDto), 201)]
        [ProducesResponseType(typeof(CreateItemDto), 400)]
        public async Task<IActionResult> Post([FromBody]CreateItemDto value)
        {
            if (value == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            var itemDto = await _itemAppService.Create(value);
            return CreatedAtAction(nameof(Get), new { id = itemDto.Id }, itemDto);
        }

        /// <summary>
        /// Updates an existing shopping item.
        /// </summary>
        /// <param name="id">ID of the item to update.</param>
        /// <param name="value">An item with the values to update.</param>
        /// <returns>No content result.</returns>
        /// <response code="204">No content result.</response>            
        /// <response code="400">If the item is null or if the provided id and the id of the item to update do not match, or if the item to update is not valid.</response>            
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ItemDto), 204)]
        [ProducesResponseType(typeof(ItemDto), 400)]
        public async Task<IActionResult> Put(Guid id, [FromBody]ItemDto value)
        {
            if (value == null || value.Id != id || !ModelState.IsValid)
                return BadRequest(ModelState);

            await _itemAppService.Update(value);
            return NoContent();
        }

        /// <summary>
        /// Deletes a specific shopping item.
        /// </summary>
        /// <param name="id">ID of the item to delete.</param>
        /// <returns>No content result.</returns>
        /// <response code="204">No content result.</response>            
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ItemDto), 204)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _itemAppService.Delete(id);
            return NoContent();
        }
    }
}
