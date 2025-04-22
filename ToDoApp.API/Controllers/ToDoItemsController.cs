using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using ToDoApp.Application.Dtos;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.Validators;
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
        private readonly IMapper _mapper;
        private readonly CreateToDoItemDtoValidator _createValidator;
        private readonly UpdateToDoItemDtoValidator _updateValidator;
        public ToDoItemsController(IToDoItemService toDoItemService, ILogger<ToDoItemsController> logger,
            IMapper mapper, CreateToDoItemDtoValidator createValidator , UpdateToDoItemDtoValidator updateValidator)
        {
            _updateValidator = updateValidator;
            _logger = logger;
            _toDoItemService = toDoItemService;
            _mapper= mapper;
            _createValidator = createValidator;
        }


        [HttpGet]
        [Authorize(Roles = "Owner,Guest")]
        [Authorize(Policy = "CanViewTasks")]
        public async Task<ActionResult<PagedResult<ToDoItemDto>>> Get([FromQuery] GetToDoItemListFilter filter)
        {
            try
            {
                _logger.LogInformation("API call to fetch ToDo items.");

                // Ensure valid PageSize and PageNumber
                if (filter.PageSize <= 0)
                {
                    filter.PageSize = 10;  // Default to 10 items per page
                }

                if (filter.PageNumber <= 0)
                {
                    filter.PageNumber = 1; // Default to first page if PageNumber is invalid
                }

                // Fetch the total count first (this is typically done in your service)
                var totalCount = await _toDoItemService.GetTotalCountAsync(
                    filter.qyerySearch,
                    filter.Title,
                    filter.Category,
                    filter.Priority,
                    filter.IsCompleted
                );
                Console.WriteLine("totalCount "+totalCount);
                // Calculate total number of pages
                int totalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize);
                Console.WriteLine("totalPages " + totalPages);
                Console.WriteLine("filter.PageNumber " + filter.PageNumber);

                // If the page number exceeds the total number of pages, set it to the last page
                if (filter.PageNumber != totalPages && totalPages > 0)
                {
                    filter.PageNumber = totalPages;
                }

                // Now fetch the data for the current page
                var pagedResult = await _toDoItemService.FilterAsync(
                    filter.qyerySearch,
                    filter.Title,
                    filter.Category,
                    filter.Priority,
                    filter.IsCompleted,
                    filter.SortBy,
                    filter.IsDesc,
                    filter.PageNumber,
                    filter.PageSize
                );

                // Return a PagedResult including the total count and page info
                return Ok(new PagedResult<ToDoItemDto>
                {
                    Items = pagedResult.Items,
                    TotalCount = totalCount,
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error fetching ToDo items with filters: Title = {Title}, Category = {Category}, Priority = {Priority}, IsCompleted = {IsCompleted}",
                    filter.Title, filter.Category, filter.Priority, filter.IsCompleted);
                return StatusCode(500, "Internal server error");
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

        public async Task<ActionResult<ToDoItem>> Post([FromBody] ToDoItemCreateDto createToDoItemDto)
        {
            try
            {
                // Validate the DTO asynchronously
                var validationResult =await  _createValidator.ValidateAsync(createToDoItemDto);

                if (!validationResult.IsValid)
                {
                    // Return validation errors if not valid
                    return BadRequest(validationResult.Errors);
                }
                var createdItem = _toDoItemService.CreateAsync(createToDoItemDto);
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

        public async Task<IActionResult> Put(Guid id, [FromBody] ToDoItemUpdateDto toDoItem)
        {
            try
            {
                // Validate the DTO asynchronously
                var validationResult = await _updateValidator.ValidateAsync(toDoItem);

                if (!validationResult.IsValid)
                {
                    // Return validation errors if not valid
                    return BadRequest(validationResult.Errors);
                }
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
