namespace backend_projetdev.Models
{
    public class Evaluation
    {
        public int Id { get; set; }
        public string EmployeId { get; set; }
        public string ResponsableId { get; set; }
        public DateTime DateEvaluation { get; set; }
        public int Note { get; set; }
        public string Description { get; set; }
        public string CommentairesEmploye { get; set; }
        public string CommentairesResponsable { get; set; }
        public string Objectifs { get; set; }
        public Employe Employe { get; set; }
        public Employe Responsable { get; set; }
        public Boolean FinaliseParEmploye { get; set; }
        public Boolean FinaliseParManager { get; set; }
        public EstApprouve EstApprouve { get; set; }
    }
    public enum EstApprouve
    {
        EnCours,
        Oui,
        Non
    }
}
