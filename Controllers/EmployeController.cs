using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend_projetdev.Models;
using backend_projetdev.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
namespace backend_projetdev.Controllers;
[Route("employe")]
[ApiController]
public class EmployeController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<Personne> _userManager;

    public EmployeController(ApplicationDbContext context, UserManager<Personne> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // Lister les employés d'une équipe donnée
    [HttpGet("equipe/{equipeId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetEmployesByEquipe(int equipeId)
    {
        var equipe = await _context.Equipes
            .Include(e => e.Employes)
            .FirstOrDefaultAsync(e => e.Id == equipeId);

        if (equipe == null) return NotFound(new { message = "Équipe non trouvée." });

        return Ok(equipe.Employes);
    }

    // Modifier un employé (PUT)
    [HttpPut("{id}/edit")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditEmploye(string id, [FromBody] EditEmployeViewModel model)
    {
        if (id != model.Id) return BadRequest(new { message = "L'ID ne correspond pas." });

        var employe = await _context.Employes.FindAsync(id);
        if (employe == null) return NotFound(new { message = "Employé non trouvé." });

        employe.Nom = model.Nom;
        employe.Prenom = model.Prenom;
        employe.Salaire = model.Salaire;
        employe.Metier = model.Metier;
        employe.DateEmbauche = model.DateEmbauche;
        employe.Contrat = model.Contrat;
        employe.Statut = model.Statut;
        employe.EquipeId = model.EquipeId;

        _context.Employes.Update(employe);
        await _context.SaveChangesAsync();

        // Vérification et modification du rôle Manager
        var user = await _userManager.FindByIdAsync(id);
        if (model.EstManager)
        {
            if (!await _userManager.IsInRoleAsync(user, "Manager"))
            {
                await _userManager.AddToRoleAsync(user, "Manager");
            }
        }
        else
        {
            if (await _userManager.IsInRoleAsync(user, "Manager"))
            {
                var result = await _userManager.RemoveFromRoleAsync(user, "Manager");
                if (!result.Succeeded)
                {
                    return BadRequest(new { message = "Impossible de retirer le rôle Manager." });
                }
            }
        }

        await _userManager.UpdateSecurityStampAsync(user);
        return Ok(new { message = "Employé modifié avec succès." });
    }

    // Supprimer un employé
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteEmploye(string id)
    {
        var employe = await _context.Employes.FindAsync(id);
        if (employe == null) return NotFound(new { message = "Employé non trouvé." });

        var equipeId = employe.EquipeId;
        _context.Employes.Remove(employe);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Employé supprimé avec succès." });
    }

    // Afficher les détails d'un employé
    [HttpGet("details/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetEmployeDetails(string id)
    {
        var employe = await _context.Employes
            .Include(e => e.Equipe)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employe == null) return NotFound(new { message = "Employé non trouvé." });

        return Ok(employe);
    }

    // Modifier les informations de connexion (email, mot de passe)
    [HttpPut("change-login-info/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ChangeLoginInfo(string id, [FromBody] ChangeLoginInfoViewModel model)
    {
        if (string.IsNullOrEmpty(model.NewEmail) && string.IsNullOrEmpty(model.NewPassword))
        {
            return BadRequest(new { message = "Aucune information à mettre à jour." });
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { message = "Utilisateur non trouvé." });
        }

        // Modifier l'email
        if (!string.IsNullOrEmpty(model.NewEmail) && model.NewEmail != user.Email)
        {
            var emailChangeResult = await _userManager.SetEmailAsync(user, model.NewEmail);
            if (!emailChangeResult.Succeeded)
            {
                return BadRequest(new { message = "Erreur lors de la modification de l'email.", errors = emailChangeResult.Errors });
            }

            var usernameChangeResult = await _userManager.SetUserNameAsync(user, model.NewEmail);
            if (!usernameChangeResult.Succeeded)
            {
                return BadRequest(new { message = "Erreur lors de la modification du nom d'utilisateur.", errors = usernameChangeResult.Errors });
            }
        }

        // Modifier le mot de passe
        if (!string.IsNullOrEmpty(model.NewPassword))
        {
            var passwordChangeResult = await _userManager.RemovePasswordAsync(user);
            if (!passwordChangeResult.Succeeded)
            {
                return BadRequest(new { message = "Erreur lors de la suppression du mot de passe.", errors = passwordChangeResult.Errors });
            }

            passwordChangeResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!passwordChangeResult.Succeeded)
            {
                return BadRequest(new { message = "Erreur lors de l'ajout du nouveau mot de passe.", errors = passwordChangeResult.Errors });
            }
        }

        await _userManager.UpdateSecurityStampAsync(user);

        return Ok(new { message = "Informations de connexion mises à jour avec succès." });
    }

    // Modifier le statut d'un employé
    [HttpPut("changer-statut")]
    [Authorize(Roles = "Employe")]
    public async Task<IActionResult> ChangerStatut([FromBody] string statut)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser != null)
        {
            if (Enum.TryParse(statut, out StatutEmploi statutEmploi))
            {
                var employe = await _context.Employes
                    .FirstOrDefaultAsync(e => e.Id == currentUser.Id);

                if (employe != null)
                {
                    employe.Statut = statutEmploi;
                    _context.Update(employe);
                    await _context.SaveChangesAsync();
                    return Ok(new { success = true });
                }
                return NotFound(new { message = "Employé non trouvé." });
            }
            return BadRequest(new { message = "Statut invalide." });
        }

        return Unauthorized(new { message = "Utilisateur non authentifié." });
    }

    // Liste des employés dans la même équipe
    [HttpGet("liste-equipe")]
    [Authorize(Roles = "Employe")]
    public async Task<IActionResult> ListeEquipe()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return Unauthorized(new { message = "Utilisateur non authentifié." });
        }

        var employeConnecte = await _context.Employes
            .Include(e => e.Equipe)
            .FirstOrDefaultAsync(e => e.Id == currentUser.Id);

        if (employeConnecte == null)
        {
            return NotFound(new { message = "Employé non trouvé." });
        }

        var employesEquipe = await _context.Employes
            .Where(e => e.EquipeId == employeConnecte.EquipeId)
            .ToListAsync();

        return Ok(employesEquipe);
    }
}
