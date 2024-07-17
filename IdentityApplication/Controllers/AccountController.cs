using Azure.Core;
using IdentityApplication.API.Dto.AccountDto;
using IdentityApplication.API.Models;
using IdentityApplication.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
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
        private readonly IConfiguration _config;
       private readonly EmailServices _emailServices;
       private readonly HttpClient _facebookClient ;

        public AccountController( JwtServices jwtServices ,UserManager<AppUser> userManager , 
			SignInManager<AppUser> signInManager , IConfiguration config , EmailServices emailServices)
        {
			_jwtServices = jwtServices;
			_userManager = userManager;
			_signInManager = signInManager;
            _config = config;
           _emailServices = emailServices;
			_facebookClient = new HttpClient()
			{
				BaseAddress = new Uri("https://graph.facebook.com")
			};
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

			//if (Result.IsLockedOut)
			//	return Unauthorized( string.Format("Your Account had been locked , wait {0}   to able to login"));

			if (!Result.Succeeded)
				return Unauthorized("Invalid Email or Password");

			return await CtreateApplicationUserDto(user);

		}


		[HttpPost("login-with-third-party")]
		public async Task<ActionResult<UserDto>> loginWithExternal(LoginWithExternal model)
		{
			if (model.Provider.Equals(Sd.Facebook))
			{
                try
                {
                    if (!FacebookValidatAsync(model.AccessToken, model.UserId).GetAwaiter().GetResult())
                    {
                        return Unauthorized("Unable To login With Facebook");
                    }
                }
                catch (Exception)
                {
                    return Unauthorized("Unable To login With Facebook");

                }
            }
			else if (model.Provider.Equals(Sd.Google))
			{
				return Ok();
			}
			else
			{
				return BadRequest("Invalid Provider");
			}

			var user = await _userManager.Users.
				FirstOrDefaultAsync(x => x.UserName == model.UserId && x.Provider == model.Provider);

			if (user is null)
				return Unauthorized("Account Not Found");

			return await CtreateApplicationUserDto(user);
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
				Email = model.Email.ToLower(),
			};

			var Result = await _userManager.CreateAsync(user , model.Paswword);

			if (!Result.Succeeded)
				return BadRequest(Result.Errors);

			try
			{
				if (await sendConfairmEmailAsync(user))
				{
					return Ok(new JsonResult(new { title = "Account Created", message = "your Account has been created , please confirm your email " }));
				}

				return BadRequest("faild to send email , please contact admin");

			}
			catch (Exception)
			{
				return BadRequest("faild to send email , please contact admin");
			}




			//return Ok("your account has been created , you can login now ");

        }


		[HttpPost("register-with-third-party")]
		public async Task<ActionResult<UserDto>> RegisterWithThiredParty(RegisterThirdParty model )
		{
			if (model.Provider.Equals(Sd.Facebook))
			{
				try
				{
                    if (!FacebookValidatAsync(model.AccessToken, model.UserId).GetAwaiter().GetResult())
                    {
                        return Unauthorized("Unable To Register With Facebook");
                    }
                }
				catch (Exception)
				{
                    return Unauthorized("Unable To Register With Facebook");

                }
		   }
            else if (model.Provider.Equals(Sd.Google))
			{
				return Ok();
			}
			else
			{
				return BadRequest("Invalid Provider");
			}


			var user = await _userManager.FindByNameAsync(model.UserId);
			if (user is not  null)
				return BadRequest( string.Format("you have an account already please login with your {0}", model.Provider));

			var userToAdd = new AppUser
			{
				FirstName = model.FirstName.ToLower(),
				LastName = model.LastName.ToLower(),
				UserName = model.UserId,
				Provider = model.Provider
			};

			var result = await _userManager.CreateAsync(userToAdd);
			if (!result.Succeeded)
				return BadRequest(result.Errors);

			return await CtreateApplicationUserDto(userToAdd);

        }

        [Authorize]
        [HttpGet("refresh-user-token")]
        public async Task<ActionResult<UserDto>> RefrshUserToken()
        {


            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);
            return await CtreateApplicationUserDto(user);

        }


        [HttpPut("confirm-email")]
		public async Task<ActionResult> ConfirmEmail(ConfirmEmailDto model)
		{

			var user = await _userManager.FindByEmailAsync(model.Email);

			if (user is null)
				return Unauthorized("this email address has not been registred yet");

			if (user.EmailConfirmed)
				return BadRequest("your email address was confirmed before , please login now");

			try
			{

				var decodedTokemByts = WebEncoders.Base64UrlDecode(model.Token);
				var decodedToken = Encoding.UTF8.GetString(decodedTokemByts);
				var result = await _userManager.ConfirmEmailAsync(user , decodedToken);

				if (result.Succeeded)
					return Ok(new JsonResult(new { title = "confirm email", message = "your email address confirmed , please login now" }));

				return BadRequest("invalid token , please try again");
			}
			catch (Exception)
			{
				
				return BadRequest("invalid token , please try again");

            }


        }



		[HttpPost("resend-email-confirmation-link/{email}")]
		public async Task<ActionResult> ResendConfiramationEmailLink(string email)
		{
			if (string.IsNullOrEmpty(email))
				return BadRequest("email not found");

			var user = await _userManager.FindByEmailAsync(email);
			if (user is null)
				return Unauthorized("this email address has not been registered");
			if (user.EmailConfirmed)
				return BadRequest("ypur email address was confirmed before , please login ");

			try
			{
				if (await sendConfairmEmailAsync(user))
					return Ok(new JsonResult(new { titl = "Confirmation link sent", message = "please confirm your email address" }));


				return BadRequest("Filed to send email , please context again");
			
			}


			catch (Exception)
			{
                return BadRequest("Filed to send email , please context again");
            }



        }



		[HttpPost("forgot-email-or-password/{email}")]
		public async Task<ActionResult> ForgotEmailOrPassword(string email)
		{
			if (string.IsNullOrEmpty(email))
				return BadRequest("Invalid Email");

			var user = await _userManager.FindByEmailAsync(email);
			if (email is null)
				return Unauthorized("this email address has not been regisrewd yet");
			if (!user.EmailConfirmed)
				return BadRequest("you must confirmed your email first");

			try
			{
				if (await sendForgotEmailOrPassword(user))
					return Ok(new JsonResult(new { title = "Forgot Email or Password", message = "please check your email" }));

				return BadRequest("Filed to send email , please context again");	
			}
			catch (Exception)
			{

                return BadRequest("Filed to send email , please context again");
            }
		
		}

		



		[HttpPut("reset-password")]
		public async Task<ActionResult> ResetPassword(ResetPasswordDto model)
		{
			var user = await _userManager.FindByEmailAsync(model.Eamil);
			if (user is null)
				return Unauthorized("this email address has not been regisrewd yet");

			if (!user.EmailConfirmed)
				return BadRequest("you must confirmed your email first");

			try
			{
				var decodedTokenBytes = WebEncoders.Base64UrlDecode(model.Token);
				var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);
				var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);
				if (result.Succeeded)
					return Ok(new JsonResult(new { title = "Reset Password", message = "Your Password has been reset" }));

				return BadRequest("Invalid Token , please try again");
			}
			catch (Exception)
			{
                return BadRequest("Invalid Token , please try again");


            }


        }
			 




		private async Task<bool> CheckEmailExit(string email)
		{
			return await _userManager.Users.AnyAsync(U => U.Email == email.ToLower());

		}
	
		
		private async Task<UserDto> CtreateApplicationUserDto(AppUser user)
		{
			return new UserDto
			{
				FirstName = user.FirstName,
				LastName = user.LastName,
				Token = await  _jwtServices.CreateJwt(user)
			};
		}	
	
	

		private async Task<bool> sendConfairmEmailAsync(AppUser user)
		{
			var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

			var url = $"{_config["Token:ClientUrl"]}/{_config["Email:ConfirmEmailPath"]}?token={token}&email={user.Email}";

			var body = $"<p> Hello {user.FirstName} {user.LastName}</p>" +
						"<p> Please Confirm Your Email Address by Clicking on the Link </p>" +
						$"<p> <a href=\"{url}\"> Click Here </a> </p>" +
						"<p> Thank You</p>" +
						$"<br> {_config["Email:ApplicationName"]}";

			var email = new EmailSendDto(user.Email, "Confirm your Email", body);

				return await _emailServices.sendEmailAsync(email);
		
		}

	
		private async Task<bool> sendForgotEmailOrPassword(AppUser user)
		{
			var token = await _userManager.GeneratePasswordResetTokenAsync(user);
			token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

			var url = $"{_config["Token:ClientUrl"]}/{_config["Email:ResetPasswordPath"]}?token{token}&email={user.Email}";


            var body = $"<p> Hello {user.FirstName} {user.LastName}</p>" +
						$"<p> email {user.Email}</p>"+
						"<p> in order to reset your password .please click in the following link</p>"+
                        "<p> Please Confirm Your Email Address by Clicking on the Link </p>" +
                        $"<p> <a href=\"{url}\"> Click Here </a> </p>" +
                        "<p> Thank You</p>" +
                        $"<br> {_config["Email:ApplicationName"]}";


			var email = new EmailSendDto(user.Email, "Forgot EmailOrPassword", body);
			return await _emailServices.sendEmailAsync(email);
		}
			
		private async Task<bool> FacebookValidatAsync(string accessToken , string userId)
		{
			var facebookKey = _config["Facebook:AppID"] + "|" + _config["Facebook:AppSecret"];
            var fbResult = await _facebookClient.GetFromJsonAsync<FacebookDto>
				($"debug_token?input_token={accessToken}& access_token={facebookKey}");
		
			if(fbResult == null || fbResult.Data.Is_Valid == false  || !fbResult.Data.User_Id.Equals(userId))
			{
				return false;
			}
			return true;
		}

	}
}
