using System.ComponentModel.DataAnnotations;

namespace IdentityApplication.API.Dto.Admin
{
    public class MemberAddAndEditDto
    {
        public string Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string UserName { get; set; }
        
        public string Password { get; set; }
        [Required]
        public  string Roles { get; set; }
    }
}
