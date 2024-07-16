using System.ComponentModel.DataAnnotations;

namespace IdentityApplication.API.Dto.AccountDto
{
    public class LoginWithExternal
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Provider { get; set; }

    }
}
