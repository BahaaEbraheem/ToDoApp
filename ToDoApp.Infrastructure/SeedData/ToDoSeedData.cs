using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ToDoApp.Application.Dtos;
using ToDoApp.Application.Interfaces;
using ToDoApp.Domain.Entities;
using ToDoApp.Infrastructure.Data;

namespace ToDoApp.Infrastructure.SeedData;

public class ToDoSeedData
{

    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure that roles exist
            string[] roles = { "Owner", "Guest" };
            foreach (var role in roles)
            {
                var roleExist = await roleManager.RoleExistsAsync(role);
                if (!roleExist)
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Create default Owner user
            var owner = await userManager.FindByEmailAsync("owner@domain.com");
            if (owner == null)
            {
                owner = new ApplicationUser
                {
                    UserName = "owner@domain.com",
                    Email = "owner@domain.com",
                    Role = "Owner"
                };
                var result = await userManager.CreateAsync(owner, "OwnerPassword123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(owner, "Owner");
                }
            }

            // Create default Guest user
            var guest = await userManager.FindByEmailAsync("guest@domain.com");
            if (guest == null)
            {
                guest = new ApplicationUser
                {
                    UserName = "guest@domain.com",
                    Email = "guest@domain.com",
                    Role = "Guest"
                };
                var result = await userManager.CreateAsync(guest, "GuestPassword123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(guest, "Guest");
                }
            }

            // إضافة مهام تجريبية للمستخدمين
            var todoService = serviceProvider.GetRequiredService<IToDoItemService>();

            if (!todoService.GetAllAsync().Result.Any())
            {
                var defaultItems = new List<ToDoItemCreateDto>
                {
                    new ToDoItemCreateDto { Title = "Test Task 1", Description = "Description for test task 1",  CreatedAt = DateTime.Now , Priority = "عالية", Category = "إدارية"  },
                    new ToDoItemCreateDto { Title = "Test Task 2", Description = "Description for test task 2",  CreatedAt = DateTime.Now ,Priority = "متوسطة", Category = "مالية" },
                    new ToDoItemCreateDto { Title = "Test Task 3", Description = "Description for test task 3",  CreatedAt = DateTime.Now ,Priority = "منخفضة", Category = "حاسوبية" }
                };

                foreach (var item in defaultItems)
                {
                   todoService.CreateAsync(item);  // استخدام خدمة المهام لإنشاء المهام في قاعدة البيانات
                }
            
            }
            

        }
    }
}
