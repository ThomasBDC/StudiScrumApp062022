using System;
using System.ComponentModel.DataAnnotations;

namespace ScrumApp.Models
{
    public class UserModel
    {
        public int IdUser { get; set; }

        [Display(Name ="Nom")]
        [Required(ErrorMessage ="Le nom est obligatoire")]
        public string Surname { get; set; }

        [Required(ErrorMessage ="Le prénom est obligatoire")]
        [Display(Name = "Prenom")]
        public string Forename { get; set; }

        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+", ErrorMessage ="L'email n'est pas valide")]
        [Required(ErrorMessage ="Le mail est obligatoire")]
        public string Mail { get; set; }
        
        [Required(ErrorMessage ="Le mot de passe est obligatoire")]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; }

        [Display(Name = "N° de téléphone")]
        public string Phone { get; set; }

        [Display(Name = "Date d'embauche")]
        public string DateEmbauche { get; set; }
        public string DateRenvoi { get; set; }
        public string SubjectId { get; set; }
        public string PasswordKey { get; set; }
    }
}
