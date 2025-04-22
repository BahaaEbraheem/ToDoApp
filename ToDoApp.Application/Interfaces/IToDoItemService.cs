using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.Application.Dtos;
using ToDoApp.Domain.Entities;

namespace ToDoApp.Application.Interfaces
{
    public interface IToDoItemService
    {
        Task<IEnumerable<ToDoItemDto>> GetAllAsync();
        ToDoItemDto CreateAsync(ToDoItemCreateDto toDoItem);
        bool UpdateAsync(Guid id, ToDoItemUpdateDto toDoItem);
        bool DeleteAsync(Guid id);
        Task<PagedResult<ToDoItemDto>> FilterAsync(
         string? querySearch,
         string? title,
         string? category,
         string? priority,
         bool? isCompleted,
         string? sortBy = "CreatedAt",
         bool isDesc = false,
         int page = 1,
         int pageSize = 10);

        Task<int> GetTotalCountAsync(string? querySearch, string? title, string? category, string? priority, bool? isCompleted);

        Task<ActionResult<ToDoItemDto>> GetById(Guid id);
        Task<bool> SetCompletedStatusAsync(Guid id, bool isCompleted);

    }
}
