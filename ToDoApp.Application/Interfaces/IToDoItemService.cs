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
        Task<ToDoItem> CreateAsync(ToDoItem toDoItem);
        Task<bool> UpdateAsync(int id, ToDoItem toDoItem);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<ToDoItem>> FilterAsync(string? searchQuery, string? priority, string? category, int pageIndex, int pageSize);

    }
}
