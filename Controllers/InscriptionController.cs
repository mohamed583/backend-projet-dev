using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projetERP.Models;
using System.Security.Claims;

namespace projetERP.Controllers
{
    [ApiController]
    [Route("inscription")]
    public class InscriptionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InscriptionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Admin : Lister les inscriptions d'une formation
        [HttpGet("formation/{formationId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Inscription>>> GetInscriptionsByFormation(int formationId)
        {
            var inscriptions = await _context.Inscriptions
                .Include(i => i.Employe)
                .Include(i => i.Formation)
                .Where(i => i.FormationId == formationId)
                .ToListAsync();

            return Ok(inscriptions);
        }

        // Admin : Approuver une inscription
        [HttpPost("{id:int}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveInscription(int id)
        {
            var inscription = await _context.Inscriptions.FindAsync(id);
            if (inscription == null)
                return NotFound(new { message = "Inscription non trouvée." });

            inscription.StatusInscription = StatusInscription.Approuve;
            await _context.SaveChangesAsync();

            return NoContent(); // 204
        }

        // Admin : Rejeter une inscription
        [HttpPost("{id:int}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectInscription(int id)
        {
            var inscription = await _context.Inscriptions.FindAsync(id);
            if (inscription == null)
                return NotFound(new { message = "Inscription non trouvée." });

            inscription.StatusInscription = StatusInscription.Rejete;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Employé : Voir toutes les formations disponibles (non inscrites)
        [HttpGet("formations-disponibles")]
        [Authorize(Roles = "Employe")]
        public async Task<ActionResult<IEnumerable<Formation>>> GetFormationsDisponibles()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Utilisateur non authentifié." });

            var formations = await _context.Formations
                .Include(f => f.Inscriptions)
                .Where(f => !f.Inscriptions.Any(i => i.EmployeId == userId))
                .ToListAsync();

            return Ok(formations);
        }

        // Employé : Postuler à une formation
        [HttpPost("postuler/{formationId:int}")]
        [Authorize(Roles = "Employe")]
        public async Task<IActionResult> Postuler(int formationId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Utilisateur non authentifié." });

            // Vérifie si déjà inscrit
            bool dejaInscrit = await _context.Inscriptions.AnyAsync(i => i.FormationId == formationId && i.EmployeId == userId);
            if (dejaInscrit)
                return BadRequest(new { message = "Vous êtes déjà inscrit à cette formation." });

            var inscription = new Inscription
            {
                EmployeId = userId,
                FormationId = formationId,
                DateInscription = DateTime.Now,
                StatusInscription = StatusInscription.EnCours
            };

            _context.Inscriptions.Add(inscription);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMesInscriptions), null, inscription);
        }

        // Employé : Voir ses inscriptions
        [HttpGet("mes-inscriptions")]
        [Authorize(Roles = "Employe")]
        public async Task<ActionResult<IEnumerable<Inscription>>> GetMesInscriptions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Utilisateur non authentifié." });

            var inscriptions = await _context.Inscriptions
                .Include(i => i.Formation)
                .Where(i => i.EmployeId == userId)
                .ToListAsync();

            return Ok(inscriptions);
        }

        // Employé : Supprimer son inscription
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Employe")]
        public async Task<IActionResult> SupprimerInscription(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Utilisateur non authentifié." });

            var inscription = await _context.Inscriptions.FindAsync(id);
            if (inscription == null)
                return NotFound(new { message = "Inscription non trouvée." });

            if (inscription.EmployeId != userId)
                return Forbid();

            _context.Inscriptions.Remove(inscription);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Formateur : Lister ses formations avec inscriptions et employés
        [HttpGet("mes-formations")]
        [Authorize(Roles = "Formateur")]
        public async Task<ActionResult<IEnumerable<Formation>>> GetFormationsEtInscriptions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Utilisateur non authentifié." });

            var formations = await _context.Formations
                .Include(f => f.Inscriptions)
                .ThenInclude(i => i.Employe)
                .Where(f => f.FormateurId == userId)
                .ToListAsync();

            return Ok(formations);
        }
    }
}
