using System.Text.Json.Serialization;

namespace backend_projetdev.Models
{
    public class Candidat: Personne
    {
        public string Description { get; set; }
        [JsonIgnore]
        public ICollection<Candidature> Candidatures { get; set; }
    }
}
