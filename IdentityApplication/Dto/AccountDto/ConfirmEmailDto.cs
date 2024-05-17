using System.ComponentModel.DataAnnotations;

namespace IdentityApplication.API.Dto.AccountDto
{
    public class ConfirmEmailDto
    {
        [Required]
        public string Token { get; set; }
        [RegularExpression("^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$", ErrorMessage = "Invaild Email")]

        public string Email { get; set; }



    }
}
