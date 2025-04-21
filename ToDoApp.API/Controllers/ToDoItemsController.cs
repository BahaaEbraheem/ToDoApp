using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Application.Interfaces;
using ToDoApp.Domain.Entities;

namespace ToDoApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoItemsController : ControllerBase
    {
        private readonly IToDoItemService _toDoItemService;

        public ToDoItemsController(IToDoItemService toDoItemService)
        {
            _toDoItemService = toDoItemService;
        }

        //// GET: api/ToDoItems
        //[HttpGet]
        //[Authorize(Roles = "Owner,Guest")]  // يمكن الوصول إليه من قبل الجميع (Owner و Guest)
        //public async Task<ActionResult<IEnumerable<ToDoItem>>> Get()
        //{
        //    var items = await _toDoItemService.GetAllAsync();
        //    return Ok(items);
        //}
        [HttpGet]
        [Authorize(Roles = "Owner,Guest")]  // يمكن الوصول إليه من قبل الجميع (Owner و Guest)
        public async Task<ActionResult<IEnumerable<ToDoItem>>> Get([FromQuery] string? searchQuery, [FromQuery] string? priority, [FromQuery] string? category, [FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 10)
        {
            var items = await _toDoItemService.FilterAsync(searchQuery, priority, category, pageIndex, pageSize);
            return Ok(items);
        }

        // POST: api/ToDoItems
        [HttpPost]
        [Authorize(Roles = "Owner")]  // فقط Owner يمكنه إضافة المهام

        public ActionResult<ToDoItem> Post([FromBody] ToDoItem toDoItem)
        {
            var createdItem = _toDoItemService.CreateAsync(toDoItem);
            return CreatedAtAction(nameof(Get), new { id = createdItem.Id }, createdItem);
        }

        // PUT: api/ToDoItems/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Owner")]  // فقط Owner يمكنه إضافة المهام

        public async Task<IActionResult> Put(int id, [FromBody] ToDoItem toDoItem)
        {
            if (await _toDoItemService.UpdateAsync(id, toDoItem))
            {
                return NoContent();
            }
            return NotFound();
        }

        // DELETE: api/ToDoItems/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Owner")]  // فقط Owner يمكنه حذف المهام

        public async Task<IActionResult> DeleteAsync(int id)
        {
            if (await _toDoItemService.DeleteAsync(id))
            {
                return NoContent();
            }
            return NotFound();
        }
    }

}
