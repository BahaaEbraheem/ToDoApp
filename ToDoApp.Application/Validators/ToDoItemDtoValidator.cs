using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.Application.Dtos;

namespace ToDoApp.Application.Validators
{
    public class ToDoItemDtoValidator : AbstractValidator<ToDoItemDto>
    {
        public ToDoItemDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("العنوان مطلوب")
                .MaximumLength(100);

            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("التصنيف مطلوب");

            RuleFor(x => x.Priority)
                .Must(p => new[] { "Low", "Medium", "High" }.Contains(p))
                .WithMessage("القيمة يجب أن تكون Low أو Medium أو High");
        }
    }
}
