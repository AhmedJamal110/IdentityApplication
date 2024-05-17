using System.ComponentModel.DataAnnotations;

namespace IdentityApplication.API.Dto.AccountDto
{
    public class ResetPasswordDto
    {
        [Required]
        public string Token { get; set; }
        [Required]
        [RegularExpression("^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$", ErrorMessage = "Invaild Email")]

        public string Eamil { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 6, ErrorMessage = "Password Must be at leat (6) and maximum (16) charchters")]

        public string NewPassword { get; set; }


    }
}
