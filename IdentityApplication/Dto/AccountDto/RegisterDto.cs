using System.ComponentModel.DataAnnotations;

namespace IdentityApplication.API.Dto.AccountDto
{
	public class RegisterDto
	{
        [Required]
		[StringLength(15 , MinimumLength = 3 , ErrorMessage = "First Name Must be at leat (3) and maximum (16) charchters")]
        public string FirstName { get; set; }
		[Required]
		[StringLength(15, MinimumLength = 3, ErrorMessage = "Last Name Must be at leat (3) and maximum (16) charchters")]

		public string LastName { get; set; }
		[Required]
		[RegularExpression("^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$", ErrorMessage ="Invaild Email")]
		public string Email { get; set; }
		[Required]
		[StringLength(15, MinimumLength = 6 , ErrorMessage = "Password Must be at leat (6) and maximum (16) charchters")]

		public string Paswword { get; set; }



    }
}
