using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApp.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string Role { get; set; }  // Can be 'Owner' or 'Guest'
    }
}
