namespace projetERP.Models
{
    public class Inscription
    {
        public int Id { get; set; }
        public string EmployeId { get; set; }
        public int FormationId { get; set; }
        public DateTime DateInscription { get; set; }
        public StatusInscription StatusInscription { get; set; }
        public Employe Employe { get; set; }
        public Formation Formation { get; set; }
    }
    public enum StatusInscription
    {
        EnCours,
        Approuve,
        Rejete
    }
}
