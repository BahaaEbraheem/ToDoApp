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
        ToDoItem GetByIdAsync(Guid id);
        ToDoItem CreateAsync(ToDoItem toDoItem);
        ToDoItem UpdateAsync(ToDoItem toDoItem);
        Task DeleteAsync(Guid id);
        Task<ToDoItem?> FindAsync(Guid id);

    }
}
