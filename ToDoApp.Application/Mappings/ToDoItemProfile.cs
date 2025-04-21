using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ToDoApp.Application.Dtos;
using ToDoApp.Domain.Entities;
using AutoMapper;
namespace ToDoApp.Application.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // لتحويل ToDoItem إلى ToDoItemDto
            CreateMap<ToDoItem, ToDoItemDto>();
            CreateMap<ToDoItemDto, ToDoItem>();

            // لتحويل ToDoItemCreateDto إلى ToDoItem
            CreateMap<ToDoItemCreateDto, ToDoItemDto>(); 
            CreateMap<ToDoItemCreateDto, ToDoItem>();

            // لتحويل ToDoItemUpdateDto إلى ToDoItem
            CreateMap<ToDoItemUpdateDto, ToDoItemDto>();
            CreateMap<ToDoItemUpdateDto, ToDoItem>();
        }
    }


}
