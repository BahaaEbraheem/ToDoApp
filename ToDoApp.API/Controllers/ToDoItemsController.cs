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

     
        [HttpGet]
        [Authorize(Roles = "Owner,Guest")]  // يمكن الوصول إليه من قبل الجميع (Owner و Guest)
        [Authorize(Policy = "CanViewTasks")]
        public async Task<ActionResult<IEnumerable<ToDoItem>>> Get([FromQuery] string? searchQuery, [FromQuery] string? priority, [FromQuery] string? category, [FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 10)
        {
            var items = await _toDoItemService.FilterAsync(searchQuery, priority, category, pageIndex, pageSize);
            return Ok(items);
        }

        // POST: api/ToDoItems
        [HttpPost]
        [Authorize(Roles = "Owner")]  // فقط Owner يمكنه إضافة المهام
        [Authorize(Policy = "CanCreateTasks")]

        public ActionResult<ToDoItem> Post([FromBody] ToDoItem toDoItem)
        {
            var createdItem = _toDoItemService.CreateAsync(toDoItem);
            return CreatedAtAction(nameof(Get), new { id = createdItem.Id }, createdItem);
        }

        // PUT: api/ToDoItems/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Owner")]  // فقط Owner يمكنه إضافة المهام
        [Authorize(Policy = "CanEditTasks")]

        public async Task<IActionResult> Put(Guid id, [FromBody] ToDoItem toDoItem)
        {
            if ( _toDoItemService.UpdateAsync(id, toDoItem))
            {
                return Ok();
            }
            return NotFound();
        }

        // DELETE: api/ToDoItems/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Owner")]  // فقط Owner يمكنه حذف المهام
        [Authorize(Policy = "CanDeleteTasks")]

        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            if ( _toDoItemService.DeleteAsync(id))
            {
                return Ok();
            }
            return NotFound();
        }
    }

}
