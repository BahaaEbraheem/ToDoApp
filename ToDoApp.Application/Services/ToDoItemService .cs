using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Application.Interfaces;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Repositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ToDoApp.Application.Services;

public class ToDoItemService : IToDoItemService
{
    private readonly IToDoItemRepository _repository;

    public ToDoItemService(IToDoItemRepository toDoItemRepository)
    {
        _repository = toDoItemRepository;
    }

    public async Task<IEnumerable<ToDoItem>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }
    public ToDoItem CreateAsync(ToDoItem toDoItem)
    {
        return  _repository.CreateAsync(toDoItem);
    }

    public  bool UpdateAsync(Guid id, ToDoItem toDoItem)
    {
        var existingItem =  _repository.GetByIdAsync(id);
        if (existingItem != null)
        {
            existingItem.Title = toDoItem.Title;
            existingItem.Description = toDoItem.Description;
            existingItem.Priority = toDoItem.Priority;
            existingItem.Category = toDoItem.Category;
            existingItem.IsCompleted = toDoItem.IsCompleted;
            existingItem.CompletedAt = toDoItem.CompletedAt;
            _repository.UpdateAsync(existingItem);
            return true;
        }
        return false;
    }

    public  bool DeleteAsync(Guid id)
    {
        var existingItem =  _repository.GetByIdAsync(id);
        if (existingItem != null)
        {
             _repository.DeleteAsync(id);
            return true;
        }
        return false;
    }
    public async Task<ActionResult<IEnumerable<ToDoItem>>> FilterAsync(
    string? keyword,
    string? category,
    string? priority,
    bool? isCompleted,
    string? sortBy = "CreatedAt",
    bool isDesc = false,
    int page = 1,
    int pageSize = 10)
    {
        // خذ جميع البيانات من الريبو
        var allItems = await _repository.GetAllAsync();

        // حولها لـ IQueryable حتى تقدر تعمل فلترة وترتيب
        var query = allItems.AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
            query = query.Where(t => t.Title.Contains(keyword) || t.Description.Contains(keyword));
        if (!string.IsNullOrEmpty(category))
            query = query.Where(t => t.Category == category);
        if (!string.IsNullOrEmpty(priority))
            query = query.Where(t => t.Priority == priority);
        if (isCompleted.HasValue)
            query = query.Where(t => t.IsCompleted == isCompleted);

        query = sortBy switch
        {
            "Title" => isDesc ? query.OrderByDescending(t => t.Title) : query.OrderBy(t => t.Title),
            "Priority" => isDesc ? query.OrderByDescending(t => t.Priority) : query.OrderBy(t => t.Priority),
            _ => isDesc ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt)
        };

        return await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
    }
    public async Task<bool> SetCompletedStatusAsync(Guid id, bool isCompleted)
    {
        var item = await _repository.FindAsync(id);
        if (item == null) return false;

        item.IsCompleted = isCompleted;
        item.CompletedAt = isCompleted ? DateTime.UtcNow : null;
        _repository.UpdateAsync(item);

        return true;
    }
 
    public async Task<ActionResult<ToDoItem>> GetById(Guid id)
    {
        return await _repository.FindAsync(id);
    }
}
