using ScrumApp.Models;
using System.Collections.Generic;

namespace StudiScrumApp062022.Models
{
    public class ProjetViewModel
    {
        public ProjectModel Projet { get; set; }
        public List<TacheModel> allTaches { get; set; }
    }
}
