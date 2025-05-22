using backend_projetdev.Models;

namespace backend_projetdev.ViewModels
{
    public class EntretienCreateViewModel
    {
        public string CandidatureId { get; set; }
        public string EmployeId { get; set; }
        public DateTime DateEntretien { get; set; }
        public List<Employe> Employes { get; set; } = new List<Employe>();
    }
}
