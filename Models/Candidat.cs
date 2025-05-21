namespace projetERP.Models
{
    public class Candidat: Personne
    {
        public string Description { get; set; }
        public ICollection<Candidature> Candidatures { get; set; }
    }
}
