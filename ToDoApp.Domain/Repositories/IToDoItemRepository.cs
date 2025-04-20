using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.Domain.Entities;

namespace ToDoApp.Domain.Repositories
{
    public interface IToDoItemRepository
    {
        Task<IEnumerable<ToDoItem>> GetAllAsync();
        Task<ToDoItem> GetByIdAsync(int id);
        Task<ToDoItem> CreateAsync(ToDoItem toDoItem);
        Task<ToDoItem> UpdateAsync(ToDoItem toDoItem);
        Task DeleteAsync(int id);
    }
}
