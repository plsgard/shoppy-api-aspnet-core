using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shoppy.Application.Items;
using Shoppy.Application.Items.Dtos;

namespace Shoppy.Api.Controllers
{
    [Route("api/[controller]")]
    public class ItemsController : Controller
    {
        private readonly IItemAppService _itemAppService;

        public ItemsController(IItemAppService itemAppService)
        {
            _itemAppService = itemAppService;
        }

        [HttpGet]
        public async Task<IList<ItemDto>> Get()
        {
            return await _itemAppService.GetAll();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var itemDto = await _itemAppService.Get(id);
            if (itemDto == null)
                return NotFound();
            return new ObjectResult(itemDto);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]CreateItemDto value)
        {
            if (value == null)
                return BadRequest();

            var itemDto = await _itemAppService.Create(value);
            return CreatedAtAction(nameof(Get), new { id = itemDto.Id }, itemDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody]ItemDto value)
        {
            if (value == null || value.Id != id)
                return BadRequest();

            await _itemAppService.Update(value);
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _itemAppService.Delete(id);
            return new NoContentResult();
        }
    }
}
