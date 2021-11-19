using CoffeeShop.Models;
using CoffeeShop.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeeShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;

        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _itemService.GetAllAsync());
        }

        [HttpGet("{id:length(24)}")]        
        public async Task<IActionResult> Get(string id)
        {
            var item = await _itemService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }
        [HttpPost]
        public async Task<IActionResult> Create(Item item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            await _itemService.CreateAsync(item);
            return Ok(item.Id);
        }
        [HttpPut("{id:length(24)}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Update(string id, Item booksData)
        {
            var item = await _itemService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            await _itemService.UpdateAsync(id, booksData);
            return NoContent();
        }
        [HttpDelete("{id:length(24)}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Delete(string id)
        {
            var item = await _itemService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            await _itemService.DeleteAsync(item.Id);
            return NoContent();
        }
    }

}
