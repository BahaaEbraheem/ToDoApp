using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.Domain.Entities;

namespace ToDoApp.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<ApplicationUser> AuthenticateAsync(string username, string password);
    }
}
