using System;
using System.Collections.Generic;
using System.Text;

namespace ScrumApp.Models
{
    public class ProjectModel
    {
        public int IdProjet { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public UserModel Proprietaire { get; set; }
    }

}
