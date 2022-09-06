using System.ComponentModel.DataAnnotations;

namespace WebAuth.Models
{
    public class UserLoginModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

    }
}
