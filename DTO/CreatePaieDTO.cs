using backend_projetdev.Models;

namespace backend_projetdev.DTOs
{
    public class CreatePaieDTO
    {
        public string PersonneId { get; set; } // L'ID de la personne (employé ou formateur)
        public List<Employe> Employes { get; set; } // Liste des employés pour afficher dans le select
        public List<Formateur> Formateurs { get; set; } // Liste des formateurs pour afficher dans le select
        public DateTime DatePaie { get; set; }
        public decimal Montant { get; set; }
        public string Description { get; set; }
        public string Avantages { get; set; }
        public string Retenues { get; set; }
    }
}
