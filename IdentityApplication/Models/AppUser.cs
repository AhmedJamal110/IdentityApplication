using Microsoft.AspNetCore.Identity;
using System;

namespace IdentityApplication.API.Models
{
    public class AppUser : IdentityUser
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;







    }






}
