using System.ComponentModel.DataAnnotations;

namespace StudiScrumApp062022.Models
{
    public class LoginModel
    {
        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}
