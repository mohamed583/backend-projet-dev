using System.Text.Json.Serialization;
namespace backend_projetdev.Models
{
    public class Poste
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Description { get; set; }
        public StatutPoste StatutPoste { get; set; }
        [JsonIgnore]
        public ICollection<Candidature> Candidatures { get; set; } = new List<Candidature>();

    }
    public enum StatutPoste
    {
        Ouvert,
        Ferme
    }
}
