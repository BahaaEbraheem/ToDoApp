using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.Application.Dtos;

namespace ToDoApp.Application.Validators
{
    public class UpdateToDoItemDtoValidator : AbstractValidator<ToDoItemUpdateDto>
    {
        public UpdateToDoItemDtoValidator()
        {

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("العنوان مطلوب")
                .MaximumLength(100).WithMessage("العنوان يجب أن لا يتجاوز 100 حرف");

            RuleFor(x => x.Priority)
                .NotEmpty().WithMessage("الأولوية مطلوبة")
                    .Must(p => new[] { "منخفضة", "متوسطة", "عالية" }.Contains(p))
                .WithMessage("القيمة يجب أن تكون منخفضة أو متوسطة أو عالية");

            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("التصنيف مطلوب");

            RuleFor(x => x.CompletedAt)
                .GreaterThan(x => x.CreatedAt).WithMessage("تاريخ الانتهاء يجب أن يكون بعد تاريخ الإنشاء");
        }
    }
}
