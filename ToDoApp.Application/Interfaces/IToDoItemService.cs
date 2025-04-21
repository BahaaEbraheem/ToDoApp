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
        Task<IEnumerable<ToDoItem>> FilterAsync(string? searchQuery, string? priority, string? category, int pageIndex, int pageSize);

    }
}
