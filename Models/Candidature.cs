namespace projetERP.Models
{
    public class Candidature
    {
        public string Id { get; set; }
        public string CandidatId { get; set; }
        public Candidat Candidat { get; set; }
        public int PosteId { get; set; }
        public Poste Poste { get; set; }
        public Status Status { get; set; }
        public string CVPath { get; set; }
        public ICollection<Entretien> Entretiens { get; set; }

    }
    public enum Status
    {
        EnCours,
        Approuve,
        Rejete
    }
}
