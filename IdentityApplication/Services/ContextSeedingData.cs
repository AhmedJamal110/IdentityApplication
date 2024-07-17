using IdentityApplication.API.Data;
using IdentityApplication.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityApplication.API.Services
{
    public class ContextSeedingData
    {
        private readonly AppIdentityDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public ContextSeedingData(AppIdentityDbContext context , 
            RoleManager<IdentityRole> roleManager , UserManager<AppUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public async Task InitializeContextAsync()
        {
            if(  _context.Database.GetPendingMigrationsAsync().GetAwaiter().GetResult().Count() > 0)
            {
                await _context.Database.MigrateAsync();
            }
       
            if(! _roleManager.Roles.Any())
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = Sd.Admin });
                await _roleManager.CreateAsync(new IdentityRole { Name = Sd.Manger });
                await _roleManager.CreateAsync(new IdentityRole { Name = Sd.PlayerRole });
            }

            if (!_userManager.Users.AnyAsync().GetAwaiter().GetResult()  )
            {
                // admin
                var adminUser = new AppUser
                {
                    FirstName = "admin",
                    LastName = "ahmed",
                    Email = "admin@gmail.com",
                    UserName = "admin@gmail.com",
                    EmailConfirmed = true
                };

                 await _userManager.CreateAsync(adminUser , "Pa$$w0rd");
                await _userManager.AddToRolesAsync(adminUser, new[] { Sd.Admin , Sd.Manger , Sd.PlayerRole });
                await _userManager.AddClaimsAsync(adminUser, new[]
                {
                    new Claim(ClaimTypes.Email , adminUser.Email),
                    new Claim(ClaimTypes.Surname , adminUser.LastName)
                });

                // manger
                var mangerUser = new AppUser
                {
                    FirstName = "manger",
                    LastName = "ahmed",
                    Email = "manger@gmail.com",
                    UserName = "manger@gmail.com",
                    EmailConfirmed = true
                };

                await _userManager.CreateAsync(mangerUser, "Pa$$w0rd");
                await _userManager.AddToRoleAsync(mangerUser, Sd.Manger );
                await _userManager.AddClaimsAsync(mangerUser, new[]
                {
                    new Claim(ClaimTypes.Email , mangerUser.Email),
                    new Claim(ClaimTypes.Surname , mangerUser.LastName)
                });

                // player

                var PlayerUser = new AppUser
                {
                    FirstName = "player",
                    LastName = "ahmed",
                    Email = "player@gmail.com",
                    UserName = "player@gmail.com",
                    EmailConfirmed = true
                };

                await _userManager.CreateAsync(PlayerUser, "Pa$$w0rd");
                await _userManager.AddToRoleAsync(PlayerUser, Sd.PlayerRole );
                await _userManager.AddClaimsAsync(PlayerUser, new[]
                {
                    new Claim(ClaimTypes.Email , PlayerUser.Email),
                    new Claim(ClaimTypes.Surname , PlayerUser.LastName)
                });


            }
        
        

        }
    }
}
