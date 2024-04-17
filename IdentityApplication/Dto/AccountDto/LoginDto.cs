using System.ComponentModel.DataAnnotations;

namespace IdentityApplication.API.Dto.AccountDto
{
	public class LoginDto
	{
        [Required]
        public string Email { get; set; }
		[Required]

		public string Password { get; set; }


    }
}
