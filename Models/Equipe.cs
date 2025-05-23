using System.Text.Json.Serialization;

namespace backend_projetdev.Models
{
    public class Equipe
    {
        public int Id { get; set; }
        public int DepartementId { get; set; }
        [JsonIgnore]
        public Departement Departement { get; set; }
        public string Nom { get; set; }
        public ICollection<Employe> Employes { get; set; } = new List<Employe>();
    }
}
