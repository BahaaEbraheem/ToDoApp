using ToDoApp.Application.Interfaces;
using ToDoApp.Domain.Entities;

namespace ToDoApp.Application.Services;

public class ToDoItemService : IToDoItemService
{
    private readonly List<ToDoItem> _toDoItems = new();

    public IEnumerable<ToDoItem> GetAll()
    {
        return _toDoItems;
    }

    public ToDoItem Create(ToDoItem toDoItem)
    {
        toDoItem.Id = _toDoItems.Count + 1;
        _toDoItems.Add(toDoItem);
        return toDoItem;
    }

    public bool Update(int id, ToDoItem toDoItem)
    {
        var item = _toDoItems.Find(i => i.Id == id);
        if (item != null)
        {
            item.Title = toDoItem.Title;
            item.IsCompleted = toDoItem.IsCompleted;
            return true;
        }
        return false;
    }

    public bool Delete(int id)
    {
        var item = _toDoItems.Find(i => i.Id == id);
        if (item != null)
        {
            _toDoItems.Remove(item);
            return true;
        }
        return false;
    }
}
