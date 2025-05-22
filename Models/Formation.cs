namespace backend_projetdev.Models
{
    public class Formation
    {
        public int Id { get; set; }
        public string FormateurId { get; set; }
        public Formateur Formateur { get; set; }
        public string Titre { get; set; }
        public string Description { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public string Cout { get; set; }
        public ICollection<Inscription> Inscriptions { get; set; } = new List<Inscription>();
    }
}
