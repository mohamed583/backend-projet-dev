using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend_projetdev.Models;
using backend_projetdev.ViewModels;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace backend_projetdev.Controllers
{
    [Authorize]
    [Route("candidature")]
    [ApiController]
    public class CandidatureController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<Personne> _userManager;

        public CandidatureController(ApplicationDbContext dbContext, UserManager<Personne> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        // Afficher les candidatures associées à un poste (Admin uniquement)
        [Authorize(Roles = "Admin")]
        [HttpGet("candidatures/{id}")]
        public async Task<ActionResult> GetCandidatures(int id)
        {
            var poste = await _dbContext.Postes.FirstOrDefaultAsync(p => p.Id == id);
            if (poste == null)
                return NotFound(new { message = "Le poste n'existe pas." });

            var candidatures = await _dbContext.Candidatures
                .Include(c => c.Candidat)
                .Where(c => c.PosteId == id)
                .ToListAsync();

            return Ok(candidatures);
        }

        // Lister les candidatures d'un candidat (uniquement pour le candidat connecté)
        [Authorize(Roles = "Candidat")]
        [HttpGet("mes-candidatures")]
        public async Task<ActionResult> GetMesCandidatures()
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Utilisateur non authentifié." });

            var candidatures = await _dbContext.Candidatures
                .Include(c => c.Poste)
                .Where(c => c.CandidatId == userId)
                .ToListAsync();

            return Ok(candidatures);
        }

        // Détails d'une candidature (Admin ou le candidat concerné)
        [HttpGet("details/{id}")]
        public async Task<ActionResult> GetDetails(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool isAdmin = User.IsInRole("Admin");

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Utilisateur non authentifié." });

            var candidature = await _dbContext.Candidatures
                .Include(c => c.Poste)
                .Include(c => c.Candidat)
                .FirstOrDefaultAsync(c => c.Id == id && (isAdmin || c.CandidatId == userId));

            if (candidature == null)
                return NotFound(new { message = "Candidature non trouvée." });

            return Ok(candidature);
        }

        // Modifier le statut d'une candidature (uniquement pour l'admin)
        [Authorize(Roles = "Admin")]
        [HttpPost("modifier-statut/{id}")]
        public async Task<ActionResult> ModifierStatut(string id, Status status)
        {
            var candidature = await _dbContext.Candidatures.FirstOrDefaultAsync(c => c.Id == id);
            if (candidature == null)
                return NotFound(new { message = "Candidature non trouvée." });

            candidature.Status = status;
            _dbContext.Candidatures.Update(candidature);
            await _dbContext.SaveChangesAsync();

            if (status == Status.Approuve)
            {
                return Ok(new { message = "Candidature approuvée, redirection pour compléter les informations." });
            }

            return Ok(new { message = "Le statut de la candidature a été modifié." });
        }

        // Supprimer une candidature (uniquement pour le candidat)
        [Authorize(Roles = "Candidat")]
        [HttpDelete("supprimer-candidature/{id}")]
        public async Task<ActionResult> SupprimerCandidature(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Utilisateur non authentifié." });

            var candidature = await _dbContext.Candidatures
                .FirstOrDefaultAsync(c => c.Id == id && c.CandidatId == userId);

            if (candidature == null)
                return NotFound(new { message = "Candidature non trouvée." });

            _dbContext.Candidatures.Remove(candidature);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Votre candidature a été supprimée avec succès." });
        }

        // Télécharger le CV d'une candidature
        [HttpGet("download-cv/{id}")]
        public async Task<ActionResult> DownloadCv(string id)
        {
            var candidature = await _dbContext.Candidatures
                .FirstOrDefaultAsync(c => c.Id == id);

            if (candidature == null || string.IsNullOrEmpty(candidature.CVPath))
                return NotFound(new { message = "Candidature ou CV introuvable." });

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", candidature.CVPath.TrimStart('/'));
            if (!System.IO.File.Exists(filePath))
                return NotFound(new { message = "Le fichier n'existe pas." });

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var fileName = Path.GetFileName(filePath);
            return File(fileBytes, "application/octet-stream", fileName);
        }

        // Transformer une candidature en employé (uniquement pour l'admin)
        [Authorize(Roles = "Admin")]
        [HttpPost("transformation-employe")]
        public async Task<ActionResult> TransformationEmploye(TransformationEmployeViewModel model)
        {
            var candidature = await _dbContext.Candidatures
                .FirstOrDefaultAsync(c => c.Id == model.CandidatureId);
            var candidat = await _dbContext.Personnes.OfType<Candidat>()
                .FirstOrDefaultAsync(c => c.Id == model.CandidatId);

            if (candidat == null)
                return NotFound(new { message = "Candidat non trouvé." });

            var employe = new Employe
            {
                Id = Guid.NewGuid().ToString(),
                Nom = candidat.Nom,
                Prenom = candidat.Prenom,
                Email = model.Email,
                UserName = model.Email,
                Metier = model.Metier,
                Salaire = model.Salaire,
                EquipeId = model.EquipeId,
                Contrat = model.Contrat,
                Statut = StatutEmploi.Actif
            };

            try
            {
                var result = await _userManager.CreateAsync(employe, model.Password);
                if (!result.Succeeded)
                    return BadRequest(new { message = "Erreur lors de la création de l'employé." });

                await _userManager.AddToRoleAsync(employe, "Employe");

                if (model.EstManager)
                {
                    await _userManager.AddToRoleAsync(employe, "Manager");
                }

                return Ok(new { message = "Le candidat a été transformé en employé avec succès." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Erreur interne du serveur." });
            }
        }
    }
}
