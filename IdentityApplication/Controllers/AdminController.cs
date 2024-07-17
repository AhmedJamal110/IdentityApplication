using IdentityApplication.API.Dto.AccountDto;
using IdentityApplication.API.Dto.Admin;
using IdentityApplication.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityApplication.API.Controllers
{
    [Authorize(Roles ="Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<AppUser> userManager , RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet("get-members")]
        public async Task<ActionResult<MemberViewDto>> GetMembers()
        {
            var members = await _userManager.Users
                                                                   .Where(x => x.UserName != "admin@gmail.com")
                                                                   .ToListAsync();

            var memberDtos = new List<MemberViewDto>();

            foreach (var member in members)
            {
                var memberDto = new MemberViewDto
                {
                    Id = member.Id,
                    FirstName = member.FirstName,
                    LastName = member.LastName,
                    UserName = member.UserName,
                    DateCreated = member.DateCreated,
                    IsLooked = await _userManager.IsLockedOutAsync(member),
                    Roles = await _userManager.GetRolesAsync(member)

                };
                     memberDtos.Add(memberDto);

            }
            return Ok(memberDtos);
        }

        [HttpGet("get-member/{id}")]
        public async Task<ActionResult<MemberAddAndEditDto>> GetMemberById(string id)
        {
           var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return NotFound();

            if (user.UserName.Equals(Sd.AdminUserName))
            {
                return BadRequest();
            }
            var userDto = new MemberAddAndEditDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Password = "",
                Roles =  string.Join("," , await _userManager.GetRolesAsync(user))
            };

            return Ok(userDto);
        }

        [HttpPut("lock-member/{id}")]
        public async Task<ActionResult> LockMember(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return NotFound();

            if( _userManager.FindByIdAsync(id).GetAwaiter().GetResult().UserName.Equals(Sd.AdminUserName))
            {
                return BadRequest("Suber Admin Cant Changed");
            }
          await  _userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddDays(5));
            return NoContent();
        }

        [HttpPut("unlock-member/{id}")]
        public async Task<ActionResult> UnLockMember(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return NotFound();

            if (_userManager.FindByIdAsync(id).GetAwaiter().GetResult().UserName.Equals(Sd.AdminUserName))
            {
                return BadRequest("Suber Admin Cant Changed");
            }
            await _userManager.SetLockoutEndDateAsync(user, null);
            return NoContent();
        }

        [HttpDelete("Delete-member/{id}")]
        public async Task<ActionResult> DeleteMember(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return NotFound();

            if (_userManager.FindByIdAsync(id).GetAwaiter().GetResult().UserName.Equals(Sd.AdminUserName))
            {
                return BadRequest("Suber Admin Cant Changed");
            }
            await _userManager.DeleteAsync(user);
            return NoContent();
        }

        [HttpGet("get-application-roles")]
        public async Task<ActionResult<string[]>> GetApplicationRoles()
        {
            var roles = await _roleManager.Roles.Select(role => role.Name).ToListAsync();
            return Ok(roles);
         }

        [HttpPost("add-edit-member")]
        public async Task<ActionResult> AddEditMember(MemberAddAndEditDto model)
        {

            var user = new AppUser();
            if (string.IsNullOrEmpty(model.Id))
            {
                //add
                if(string.IsNullOrEmpty(model.Password) && model.Password.Length < 6)
                {
                    return BadRequest("Password should be more than 6 charchter");
                }
                user = new AppUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.UserName,
                    EmailConfirmed = true
                };

              var result =   await _userManager.CreateAsync(user, model.Password);
                if (! result.Succeeded)
                    return BadRequest();
            }
            else
            {
                user = await _userManager.FindByIdAsync(model.Id);
            }
            var userRole = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRoleAsync(user, userRole);
            foreach (var role in model.Roles.Split("," ).ToArray())
            {
                var roleToAdd = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Name == role);
                if(roleToAdd != null)
                {
                    await _userManager.AddToRoleAsync(user, role);
                }

            }
            return Ok();
        }
    
    
    }
}
