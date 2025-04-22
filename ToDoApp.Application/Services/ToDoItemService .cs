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
            _repository.CreateAsync(toDoItem);
   
            _logger.LogInformation($"Created ToDo item with ID: {toDoItem.Id}");
            return _mapper.Map<ToDoItemDto>(toDoItem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating ToDo item.");
            throw new ApplicationException("An error occurred while creating the ToDo item.", ex);
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

            // Mapping values
            _mapper.Map(toDoItemUpdateDto, existingItem);

             _repository.UpdateAsync(existingItem);
            _logger.LogInformation($"Updated ToDo item with ID: {id}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating ToDo item.");
            throw new ApplicationException($"An error occurred while updating the ToDo item with ID: {id}.", ex);
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
            throw new ApplicationException($"An error occurred while deleting the ToDo item with ID: {id}.", ex);
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
    //public async Task<PagedResult<ToDoItemDto>> FilterAsync(
    //    string? keyword,
    //    string? category,
    //    string? priority,
    //    bool? isCompleted,
    //    string? sortBy = "CreatedAt",
    //    bool isDesc = false,
    //    int page = 1,
    //    int pageSize = 10)
    //{
    //    _logger.LogInformation("Filtering ToDo items.");
    //    try
    //    {
    //        var query = _repository.QueryAll();

    //        if (!string.IsNullOrEmpty(keyword))
    //        {
    //            query = query.Where(t =>
    //                t.Title.Contains(keyword) ||
    //                t.Description.Contains(keyword) ||
    //                t.Category.Contains(keyword) ||
    //                t.Priority.Contains(keyword));
    //        }

    //        if (!string.IsNullOrEmpty(category))
    //            query = query.Where(t => t.Category == category);

    //        if (!string.IsNullOrEmpty(priority))
    //            query = query.Where(t => t.Priority == priority);

    //        if (isCompleted.HasValue)
    //            query = query.Where(t => t.IsCompleted == isCompleted);

    //        var totalCount = await query.CountAsync(); // العدد الكلي قبل التقطيع

    //        query = sortBy switch
    //        {
    //            "Title" => isDesc ? query.OrderByDescending(t => t.Title) : query.OrderBy(t => t.Title),
    //            "Priority" => isDesc ? query.OrderByDescending(t => t.Priority) : query.OrderBy(t => t.Priority),
    //            _ => isDesc ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt)
    //        };

    //        var result = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
    //        var dtoList = _mapper.Map<IEnumerable<ToDoItemDto>>(result);

    //        return new PagedResult<ToDoItemDto>
    //        {
    //            Items = dtoList,
    //            TotalCount = totalCount,
    //            PageNumber = page,
    //            PageSize = pageSize
    //        };
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error occurred while filtering ToDo items.");
    //        throw new ApplicationException("An error occurred while filtering the ToDo items.", ex);
    //    }
    //}


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
