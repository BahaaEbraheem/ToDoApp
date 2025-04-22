using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ToDoApp.Application.Dtos;
using ToDoApp.Application.Interfaces;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Repositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace ToDoApp.Application.Services;

public class ToDoItemService : IToDoItemService
{
    private readonly IToDoItemRepository _repository;
    private readonly ILogger<ToDoItemService> _logger;
    private readonly IMapper _mapper;
    public ToDoItemService(IMapper mapper, IToDoItemRepository toDoItemRepository, ILogger<ToDoItemService> logger)
    {
        _repository = toDoItemRepository;
        _logger = logger;
        _mapper=mapper;
    }

    public async Task<IEnumerable<ToDoItemDto>> GetAllAsync()
    {
        _logger.LogInformation("Fetching all ToDo items.");
        try
        {
            var items = await _repository.GetAllAsync();
            _logger.LogInformation($"Fetched {items.Count()} ToDo items.");
            return _mapper.Map<IEnumerable<ToDoItemDto>>(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching ToDo items.");
            throw;
        }
    }
    public ToDoItemDto CreateAsync(ToDoItemCreateDto toDoItemCreteDto)
    {
        //return  _repository.CreateAsync(toDoItem);
        _logger.LogInformation("Creating a new ToDo item.");
        try
        {
            var toDoItem = _mapper.Map<ToDoItem>(toDoItemCreteDto);
            var existingItem =  _repository.GetByTitleAsync(toDoItemCreteDto.Title);
            if (existingItem != null)
            {
                _logger.LogWarning($"ToDo item with title '{toDoItemCreteDto.Title}' already exists.");
                throw new ApplicationException($"A ToDo item with the title '{toDoItemCreteDto.Title}' already exists.");
            }
            _repository.CreateAsync(toDoItem);
   
            _logger.LogInformation($"Created ToDo item with ID: {toDoItem.Id}");
            return _mapper.Map<ToDoItemDto>(toDoItem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating ToDo item.");
             throw ;
        }
    }

    public  bool UpdateAsync(Guid id, ToDoItemUpdateDto toDoItemUpdateDto)
    {
        _logger.LogInformation($"Updating ToDo item with ID: {id}.");
        try
        {
         
            var existingItem =  _repository.GetByIdAsync(id);
            if (existingItem == null)
            {
                _logger.LogWarning($"ToDo item with ID: {id} not found.");
                return false;
            }
            // Check if another item with the same title exists (excluding the current item)
            var itemWithSameTitle =  _repository.GetByTitleAsync(toDoItemUpdateDto.Title);
            if (itemWithSameTitle != null && itemWithSameTitle.Id != id)  // Ensure the IDs don't match
            {
                _logger.LogWarning($"ToDo item with title '{toDoItemUpdateDto.Title}' already exists.");
                throw new ApplicationException($"A ToDo item with the title '{toDoItemUpdateDto.Title}' already exists.");
            }

            // Mapping values
            _mapper.Map(toDoItemUpdateDto, existingItem);

             _repository.UpdateAsync(existingItem);
            _logger.LogInformation($"Updated ToDo item with ID: {id}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating ToDo item.");
            throw ;
        }
    }

    public  bool DeleteAsync(Guid id)
    {
        _logger.LogInformation($"Deleting ToDo item with ID: {id}.");
        try
        {
            var existingItem =  _repository.GetByIdAsync(id);
            if (existingItem == null)
            {
                _logger.LogWarning($"ToDo item with ID: {id} not found.");
                return false;
            }

             _repository.DeleteAsync(id);
            _logger.LogInformation($"Deleted ToDo item with ID: {id}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting ToDo item.");
            throw ;
        }
    }
    public async Task<int> GetTotalCountAsync(string? qyerySearch, string? title, string? category, string? priority, bool? isCompleted)
    {
        var query = _repository.QueryAll();

        if (!string.IsNullOrEmpty(qyerySearch))
        {
            query = query.Where(t =>
                t.Title.Contains(qyerySearch) ||
                t.Description.Contains(qyerySearch) ||
                t.Category.Contains(qyerySearch) ||
                t.Priority.Contains(qyerySearch));
        }
        if (!string.IsNullOrEmpty(title))
            query = query.Where(x => x.Title.Contains(title));

        if (!string.IsNullOrEmpty(category))
            query = query.Where(x => x.Category.Contains(category));

        if (!string.IsNullOrEmpty(priority))
            query = query.Where(x => x.Priority.Contains(priority));

        if (isCompleted.HasValue)
            query = query.Where(x => x.IsCompleted == isCompleted.Value);

        return await query.CountAsync();
    }
    public async Task<PagedResult<ToDoItemDto>> FilterAsync(string? qyerySearch,string? title, string? category, string? priority, bool? isCompleted,
                                                        string sortBy, bool isDesc, int pageNumber, int pageSize)
    {
        var query = _repository.QueryAll();

        if (!string.IsNullOrEmpty(qyerySearch))
        {
            query = query.Where(t =>
                t.Title.Contains(qyerySearch) ||
                t.Description.Contains(qyerySearch) ||
                t.Category.Contains(qyerySearch) ||
                t.Priority.Contains(qyerySearch));
        }
        if (!string.IsNullOrEmpty(title))
            query = query.Where(x => x.Title.Contains(title));

        if (!string.IsNullOrEmpty(category))
            query = query.Where(x => x.Category.Contains(category));

        if (!string.IsNullOrEmpty(priority))
            query = query.Where(x => x.Priority.Contains(priority));

        if (isCompleted.HasValue)
            query = query.Where(x => x.IsCompleted == isCompleted.Value);

        if (sortBy == "CreatedAt")
            query = isDesc ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new ToDoItemDto
            {
                Id = x.Id,
                Title = x.Title,
                Category = x.Category,
                Priority = x.Priority,
                IsCompleted = x.IsCompleted,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        return new PagedResult<ToDoItemDto>
        {
            Items = items,
            TotalCount = await GetTotalCountAsync(qyerySearch,title, category, priority, isCompleted),
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }


    public async Task<bool> SetCompletedStatusAsync(Guid id, bool isCompleted)
    {
        _logger.LogInformation($"Setting completed status for ToDo item with ID: {id}.");
        try
        {
            var item = await _repository.FindAsync(id);
            if (item == null)
            {
                _logger.LogWarning($"ToDo item with ID: {id} not found.");
                return false;
            }

            item.IsCompleted = isCompleted;
            item.CompletedAt = isCompleted ? DateTime.UtcNow : null;
            _repository.UpdateAsync(item);
            _logger.LogInformation($"Updated completed status for ToDo item with ID: {id}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while setting the completed status.");
            throw new ApplicationException("An error occurred while setting the completed status.", ex);
        }
    }
 
    public async Task<ActionResult<ToDoItemDto>> GetById(Guid id)
    {
        _logger.LogInformation($"Fetching ToDo item by ID: {id}.");
        try
        {
            var item = await _repository.FindAsync(id);
            if (item == null)
            {
                _logger.LogWarning($"ToDo item with ID: {id} not found.");
                return new NotFoundResult();
            }

            return _mapper.Map<ToDoItemDto>(item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching ToDo item by ID.");
            throw new ApplicationException("An error occurred while fetching the ToDo item.", ex);
        }
    }
}
