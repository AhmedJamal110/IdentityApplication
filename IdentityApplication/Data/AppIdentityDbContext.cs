using IdentityApplication.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityApplication.API.Data
{
	public class AppIdentityDbContext : IdentityDbContext<AppUser>
	{

        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options ): base(options) 
        {
            
        }






    }
}
