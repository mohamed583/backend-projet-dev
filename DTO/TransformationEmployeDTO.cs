using backend_projetdev.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace backend_projetdev.DTOs;
public class TransformationEmployeDTO
{
    public string CandidatId { get; set; }
    public string CandidatureId { get; set; }
    public string Email { get; set; }
    public string Metier { get; set; }
    public decimal Salaire { get; set; }
    public int EquipeId { get; set; }
    public StatutContractuel Contrat { get; set; }
    public bool EstManager { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Required]
    public string ConfirmPassword { get; set; }
}
