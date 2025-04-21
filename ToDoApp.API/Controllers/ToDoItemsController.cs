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
        private readonly ILogger<ToDoItemsController> _logger;
        public ToDoItemsController(IToDoItemService toDoItemService, ILogger<ToDoItemsController> logger)
        {
            _logger= logger;
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
            try
            {
                _logger.LogInformation("API call to fetch ToDo items.");
                return await _toDoItemService.FilterAsync(keyword, category, priority, isCompleted, sortBy, isDesc, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching ToDo items with filters: Keyword = {Keyword}, Category = {Category}, Priority = {Priority}, IsCompleted = {IsCompleted}", keyword, category, priority, isCompleted);
                throw; // إعادة رمي الاستثناء ليتم التعامل معه في Middleware
            }
        }
        // GET: api/ToDoItems/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Owner,Guest")]  // يمكن الوصول إليه من قبل الجميع (Owner و Guest)
        [Authorize(Policy = "CanViewTasks")]
        public async Task<ActionResult<ToDoItem>> GetToDoItem(Guid id)
        {
            try
            {
                var item = await _toDoItemService.GetById(id);

                if (item.Value == null)
                {
                    // توليد استثناء إذا لم يتم العثور على العنصر
                    throw new KeyNotFoundException("The ToDo item with the specified ID was not found.");
                }

                return Ok(item);
            }
            catch (Exception ex)
            {
                // هنا يتم إرسال الاستثناء إلى الـ Middleware للتعامل معه
                _logger.LogError(ex, "Error fetching ToDo item with id: {Id}", id);
                throw; // يتم إعادة رمي الاستثناء للتعامل معه في الـ Middleware
            }
            //var item = await _toDoItemService.GetById(id);
            //if (item == null) return NotFound();
            //return item;
        }

        [HttpPatch("{id}/complete")]
        [Authorize(Roles = "Owner")]
        [Authorize(Policy = "CanEditTasks")]
        public async Task<IActionResult> SetCompletionStatus(Guid id, [FromQuery] bool isCompleted)
        {
            //var result = await _toDoItemService.SetCompletedStatusAsync(id, isCompleted);
            //if (!result) return NotFound();
            //return Ok();

            try
            {
                var result = await _toDoItemService.SetCompletedStatusAsync(id, isCompleted);
                if (!result) return NotFound();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting completion status for ToDo item with id: {Id}", id);
                throw; // إعادة رمي الاستثناء ليتم التعامل معه في Middleware
            }
        }

        // POST: api/ToDoItems
        [HttpPost]
        [Authorize(Roles = "Owner")]  // فقط Owner يمكنه إضافة المهام
        [Authorize(Policy = "CanCreateTasks")]

        public ActionResult<ToDoItem> Post([FromBody] ToDoItem toDoItem)
        {
            try
            {
                var createdItem = _toDoItemService.CreateAsync(toDoItem);
                return CreatedAtAction(nameof(Get), new { id = createdItem.Id }, createdItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new ToDo item.");
                throw; // إعادة رمي الاستثناء ليتم التعامل معه في Middleware
            }
            //var createdItem = _toDoItemService.CreateAsync(toDoItem);
            //return CreatedAtAction(nameof(Get), new { id = createdItem.Id }, createdItem);
        }

        // PUT: api/ToDoItems/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Owner")]  // فقط Owner يمكنه إضافة المهام
        [Authorize(Policy = "CanEditTasks")]

        public async Task<IActionResult> Put(Guid id, [FromBody] ToDoItem toDoItem)
        {
            try
            {
                var result = _toDoItemService.UpdateAsync(id, toDoItem);
                if (!result) return NotFound();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating ToDo item with id: {Id}", id);
                throw; // إعادة رمي الاستثناء ليتم التعامل معه في Middleware
            }
        }

        // DELETE: api/ToDoItems/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Owner")]  // فقط Owner يمكنه حذف المهام
        [Authorize(Policy = "CanDeleteTasks")]

        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            try
            {
                var result =  _toDoItemService.DeleteAsync(id);
                if (!result) return NotFound();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting ToDo item with id: {Id}", id);
                throw; // إعادة رمي الاستثناء ليتم التعامل معه في Middleware
            }
        }
    }

}
