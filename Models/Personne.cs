using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;


namespace projetERP.Models
{
    public class Personne : IdentityUser
    {
        public string Nom { get; set; }
        public string Prenom { get; set; }
        [JsonIgnore]
        public ICollection<Paie> Paies { get; set; } = new List<Paie>();
    }
}
