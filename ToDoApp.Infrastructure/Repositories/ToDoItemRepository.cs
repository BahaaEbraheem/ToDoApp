using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Repositories;
using ToDoApp.Infrastructure.Data;

namespace ToDoApp.Infrastructure.Repositories
{
    public class ToDoItemRepository : IToDoItemRepository
    {
        private readonly ApplicationDbContext _context;

        public ToDoItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public  IQueryable<ToDoItem> QueryAll()
        {
            return  _context.ToDoItems.AsQueryable();
        }
        public async Task<IEnumerable<ToDoItem>> GetAllAsync()
        {
            return await _context.ToDoItems.ToListAsync();
        }
        public ToDoItem GetByTitleAsync(string title)
        {
            return  _context.ToDoItems
                                   .Where(item => item.Title == title)
                                   .FirstOrDefault();
        }
        public ToDoItem GetByIdAsync(Guid id)
        {
            return _context.ToDoItems.Find(id);
        }

        public  ToDoItem CreateAsync(ToDoItem toDoItem)
        {
            _context.ToDoItems.Add(toDoItem);
            _context.SaveChanges();
            return toDoItem;
        }

        public ToDoItem UpdateAsync(ToDoItem toDoItem)
        {

            _context.ToDoItems.Update(toDoItem);
            _context.SaveChangesAsync();
            return toDoItem;
        }

        public async Task DeleteAsync(Guid id)
        {
            var toDoItem = await _context.ToDoItems.FindAsync(id);
            if (toDoItem != null)
            {
                _context.ToDoItems.Remove(toDoItem);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<ToDoItem?> FindAsync(Guid id)
        { 
            var workItem = await _context.ToDoItems.FindAsync(id);
            return workItem;
        }
    }

}
