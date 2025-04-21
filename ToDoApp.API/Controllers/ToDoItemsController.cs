using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
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
        public async Task<ActionResult<IEnumerable<ToDoItem>>> Get(
                [FromQuery] string? keyword,
    [FromQuery] string? category,
    [FromQuery] string? priority,
    [FromQuery] bool? isCompleted,
    [FromQuery] string? sortBy = "CreatedAt",
    [FromQuery] bool isDesc = false,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
        {
            return await _toDoItemService.FilterAsync(keyword, category, priority, isCompleted, sortBy, isDesc, page, pageSize);
        }
        // GET: api/ToDoItems/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ToDoItem>> GetToDoItem(Guid id)
        {
            var item = await _toDoItemService.GetById(id);
            if (item == null) return NotFound();
            return item;
        }

        [HttpPatch("{id}/complete")]
        public async Task<IActionResult> SetCompletionStatus(Guid id, [FromQuery] bool isCompleted)
        {
            var result = await _toDoItemService.SetCompletedStatusAsync(id, isCompleted);
            if (!result) return NotFound();
            return Ok();
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
