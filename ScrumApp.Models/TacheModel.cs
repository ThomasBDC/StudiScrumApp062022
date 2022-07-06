using System;
using System.Collections.Generic;
using System.Text;

namespace ScrumApp.Models
{
    public class TacheModel
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Description { get; set; }
        public StatusTache Status { get; set; }
    }

    public enum StatusTache
    {
        Todo = 1, 
        InProgress = 2,
        Done = 3
    }
}
