using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApp.Application.Dtos
{
    public class ToDoItemDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public string Priority { get; set; }
        public string Category { get; set; }


        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

    }
}
