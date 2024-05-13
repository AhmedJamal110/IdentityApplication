using IdentityApplication.API.Dto.AccountDto;
using IdentityApplication.API.Models;
using IdentityApplication.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityApplication.API.Controllers
{
    [Route("api/[controller]")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private readonly JwtServices _jwtServices;
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;

		public AccountController( JwtServices jwtServices ,UserManager<AppUser> userManager , SignInManager<AppUser> signInManager )
        {
			_jwtServices = jwtServices;
			_userManager = userManager;
			_signInManager = signInManager;
		}

		[HttpPost("login")]
		public async Task<ActionResult<UserDto>> Login( LoginDto model)
		{
			var user = await _userManager.FindByEmailAsync(model.Email);
			if (user is null)
				return Unauthorized("Invaild Email OR Password");

			if (!user.EmailConfirmed)
				return Unauthorized("Please Confirm your Email");

			var Result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

			if (!Result.Succeeded)
				return Unauthorized("Invalid Email or Password");

			return CtreateApplicationUserDto(user);

		}
		[HttpPost("register")]	
        public async Task<ActionResult<UserDto>> Register( RegisterDto model)
		{
			if(await CheckEmailExit(model.Email))
			{
				return BadRequest($"this email {model.Email} is Already in using, Please try with another Email ");
			}

			var user = new AppUser
			{
				FirstName = model.FirstName.ToLower(),
				LastName = model.LastName.ToLower(),
				UserName = model.Email.ToLower(),
				EmailConfirmed = true,
				Email = model.Email.ToLower(),
			};

			var Result = await _userManager.CreateAsync(user , model.Paswword);

			if (!Result.Succeeded)
				return BadRequest(Result.Errors);

			return Ok(new JsonResult(new {title = "Account Created" , message="your account has been created , you can login" }  ));
			//return Ok("your account has been created , you can login now ");

		}

		[Authorize]
		[HttpGet("refresh-user-token")]
		public async Task<ActionResult<UserDto>> RefrshUserToken()
		{

			var email = User.FindFirstValue(ClaimTypes.Email);
			var user = await _userManager.FindByEmailAsync(email);
			
			//var UserName = User.FindFirst(ClaimTypes.Name)?.Value;
			//var user =  await _userManager.FindByNameAsync(UserName);
			return CtreateApplicationUserDto(user);

		}





		private async Task<bool> CheckEmailExit(string email)
		{
			return await _userManager.Users.AnyAsync(U => U.Email == email.ToLower());

		}
	
		
		private UserDto CtreateApplicationUserDto(AppUser user)
		{
			return new UserDto
			{
				FirstName = user.FirstName,
				LastName = user.LastName,
				Token = _jwtServices.CreateJwt(user)
			};
		}	
	
	
	}
}
