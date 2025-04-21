using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApp.Application.Dtos
{
    public class ToDoItemCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public string Category { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // التاريخ سيتم تحديده في الخلفية بناءً على الوقت الحالي عند الإنشاء
    }
}
