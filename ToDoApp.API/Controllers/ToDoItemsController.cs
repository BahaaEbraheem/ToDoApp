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

        // GET: api/ToDoItems
        [HttpGet]
        [Authorize(Roles = "Owner,Guest")]  // يمكن الوصول إليه من قبل الجميع (Owner و Guest)
        public ActionResult<IEnumerable<ToDoItem>> Get()
        {
            var items = _toDoItemService.GetAll();
            return Ok(items);
        }

        // POST: api/ToDoItems
        [HttpPost]
        [Authorize(Roles = "Owner")]  // فقط Owner يمكنه إضافة المهام

        public ActionResult<ToDoItem> Post([FromBody] ToDoItem toDoItem)
        {
            var createdItem = _toDoItemService.Create(toDoItem);
            return CreatedAtAction(nameof(Get), new { id = createdItem.Id }, createdItem);
        }

        // PUT: api/ToDoItems/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Owner")]  // فقط Owner يمكنه إضافة المهام

        public IActionResult Put(int id, [FromBody] ToDoItem toDoItem)
        {
            if (_toDoItemService.Update(id, toDoItem))
            {
                return NoContent();
            }
            return NotFound();
        }

        // DELETE: api/ToDoItems/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Owner")]  // فقط Owner يمكنه حذف المهام

        public IActionResult Delete(int id)
        {
            if (_toDoItemService.Delete(id))
            {
                return NoContent();
            }
            return NotFound();
        }
    }

}
