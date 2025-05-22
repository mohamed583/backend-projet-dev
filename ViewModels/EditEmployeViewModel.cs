using backend_projetdev.Models;
namespace backend_projetdev.ViewModels;

public class EditEmployeViewModel
{
    public string Id { get; set; }
    public string Nom { get; set; }
    public string Prenom { get; set; }
    public string Email { get; set; }
    public decimal Salaire { get; set; }
    public string Metier { get; set; }
    public DateTime DateEmbauche { get; set; }
    public StatutContractuel Contrat { get; set; }
    public StatutEmploi Statut { get; set; }
    public int EquipeId { get; set; }
    public List<Equipe> Equipes { get; set; } = new List<Equipe>();
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
    public bool EstManager { get; set; }
}
