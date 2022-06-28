using System.ComponentModel.DataAnnotations;

namespace StudiScrumApp062022.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage ="Le login est obligatoire")]
        public string Login { get; set; }
        [Required(ErrorMessage ="Le mot de passe est obligatoire")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}
