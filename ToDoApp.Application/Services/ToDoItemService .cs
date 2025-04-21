using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Application.Interfaces;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Repositories;

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
    public async Task<IEnumerable<ToDoItem>> FilterAsync(string? searchQuery, string? priority, string? category, int pageIndex, int pageSize)
    {
        var items = await _repository.GetAllAsync();

        if (!string.IsNullOrEmpty(searchQuery))
        {
            items = items.Where(x => x.Title.Contains(searchQuery) || x.Description.Contains(searchQuery));
        }

        if (!string.IsNullOrEmpty(priority))
        {
            items = items.Where(x => x.Priority.ToString() == priority);
        }

        if (!string.IsNullOrEmpty(category))
        {
            items = items.Where(x => x.Category == category);
        }

        return items.Skip(pageIndex * pageSize).Take(pageSize).ToList();
    }

    public async Task<ActionResult<ToDoItem>> GetById(Guid id)
    {
        return await _repository.FindAsync(id);
    }
}
