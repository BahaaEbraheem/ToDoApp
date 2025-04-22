using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApp.Application.Dtos
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
        public int TotalCount { get; set; }           // عدد كل العناصر بعد الفلترة
        public int PageNumber { get; set; }           // الصفحة الحالية
        public int PageSize { get; set; }             // عدد العناصر في كل صفحة
    }

}
