using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shoppy.Application.Lists;
using Shoppy.Application.Lists.Dtos;

namespace Shoppy.Api.Controllers
{
    [Route("api/[controller]")]
    public class ListsController : Controller
    {
        private readonly IListAppService _listAppService;

        public ListsController(IListAppService listAppService)
        {
            _listAppService = listAppService;
        }

        [HttpGet]
        public async Task<IList<ListDto>> Get()
        {
            return await _listAppService.GetAll();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var listDto = await _listAppService.Get(id);
            if (listDto == null)
                return NotFound();
            return new ObjectResult(listDto);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]CreateListDto value)
        {
            if (value == null)
                return BadRequest();

            var listDto = await _listAppService.Create(value);
            return CreatedAtAction(nameof(Get), new { id = listDto.Id }, listDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody]ListDto value)
        {
            if (value == null || value.Id != id)
                return BadRequest();

            await _listAppService.Update(value);
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _listAppService.Delete(id);
            return new NoContentResult();
        }
    }
}
