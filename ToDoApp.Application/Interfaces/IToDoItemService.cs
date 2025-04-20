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
        IEnumerable<ToDoItem> GetAll();
        ToDoItem Create(ToDoItem toDoItem);
        bool Update(int id, ToDoItem toDoItem);
        bool Delete(int id);
    }
}
