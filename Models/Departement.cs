using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace projetERP.Models
{
    public class Departement
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        [ValidateNever]
        public ICollection<Equipe> Equipes { get; set; }
    }
}
