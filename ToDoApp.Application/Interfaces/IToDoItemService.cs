using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.Domain.Entities;

namespace ToDoApp.Application.Interfaces
{
    public interface IToDoItemService
    {
        Task<IEnumerable<ToDoItem>> GetAllAsync();
        ToDoItem CreateAsync(ToDoItem toDoItem);
        bool UpdateAsync(Guid id, ToDoItem toDoItem);
        bool DeleteAsync(Guid id);
        Task<ActionResult<IEnumerable<ToDoItem>>> FilterAsync(
         string? keyword,
         string? category,
         string? priority,
         bool? isCompleted,
         string? sortBy = "CreatedAt",
         bool isDesc = false,
         int page = 1,
         int pageSize = 10);
        Task<ActionResult<ToDoItem>> GetById(Guid id);
        Task<bool> SetCompletedStatusAsync(Guid id, bool isCompleted);

    }
}
