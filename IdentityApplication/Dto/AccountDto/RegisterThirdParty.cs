using System.ComponentModel.DataAnnotations;

namespace IdentityApplication.API.Dto.AccountDto
{
    public class RegisterThirdParty
    {

        [Required]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "First Name Must be at leat (3) and maximum (16) charchters")]

        public string  FirstName{ get; set; }
        [Required]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Last Name Must be at leat (3) and maximum (16) charchters")]

        public string  LastName{ get; set; }
        [Required]
        public string  Provider{ get; set; }
        [Required]
        public string  AccessToken{ get; set; }
        [Required]

        public string  UserId{ get; set; }


   
   }
}
