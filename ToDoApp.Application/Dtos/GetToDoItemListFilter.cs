using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApp.Application.Dtos
{
    public class GetToDoItemListFilter
    {
        public string? qyerySearch { get; set; }
        public string? Title { get; set; }  // الفلترة حسب العنوان
        public string? Priority { get; set; }  // الفلترة حسب الأولوية
        public string? Category { get; set; }  // الفلترة حسب الفئة
        public string? Description { get; set; }
        public bool? IsCompleted { get; set; }  // الفلترة حسب حالة المهمة (مكتملة أو لا)
        public DateTime? CreatedAfter { get; set; }  // الفلترة حسب تاريخ الإنشاء بعد تاريخ معين
        public DateTime? CreatedBefore { get; set; }  // الفلترة حسب تاريخ الإنشاء قبل تاريخ معين
        public string SortBy { get; set; } = "CreatedAt";  // Default sort by CreatedAt
        public bool IsDesc { get; set; } = false;
        public int PageNumber { get; set; } = 1;  // الترقيم
        public int PageSize { get; set; } = 10;  // الترقيم
    }
}
