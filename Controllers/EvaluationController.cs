using Microsoft.AspNetCore.Mvc;
using backend_projetdev.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace backend_projetdev.Controllers
{
    [ApiController]
    [Route("evaluation")]
    public class EvaluationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Personne> _userManager;

        public EvaluationController(ApplicationDbContext context, UserManager<Personne> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 1. Liste des évaluations (Admin, Manager, Employé)
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetEvaluations()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var employe = await _context.Employes.FirstOrDefaultAsync(e => e.Id == user.Id);

            if (User.IsInRole("Admin"))
            {
                var evaluations = await _context.Evaluations
                    .Include(e => e.Employe)
                    .Include(e => e.Responsable)
                    .ToListAsync();

                return Ok(evaluations);
            }

            if (User.IsInRole("Manager"))
            {
                if (employe == null)
                    return Forbid();

                var teamIds = await _context.Employes
                    .Where(e => e.EquipeId == employe.EquipeId)
                    .Select(e => e.Id)
                    .ToListAsync();

                var evaluations = await _context.Evaluations
                    .Where(e => teamIds.Contains(e.EmployeId))
                    .Include(e => e.Employe)
                    .Include(e => e.Responsable)
                    .ToListAsync();

                return Ok(evaluations);
            }

            return Unauthorized();
        }

        // 2. Créer une évaluation
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CreateEvaluation([FromBody] Evaluation evaluation)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var employe = await _context.Employes.FirstOrDefaultAsync(e => e.Id == evaluation.EmployeId);
            var responsable = await _context.Employes.FirstOrDefaultAsync(e => e.Id == user.Id);

            if (employe == null)
                return BadRequest(new { message = "Employé invalide." });

            evaluation.ResponsableId = user.Id;
            evaluation.Responsable = responsable;
            evaluation.Employe = employe;
            evaluation.CommentairesEmploye = "";
            evaluation.CommentairesResponsable = "";
            evaluation.Note = 0;
            evaluation.FinaliseParEmploye = false;
            evaluation.FinaliseParManager = false;
            evaluation.EstApprouve = EstApprouve.EnCours;
            evaluation.DateEvaluation = DateTime.Now;

            _context.Evaluations.Add(evaluation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEvaluationById), new { id = evaluation.Id }, evaluation);
        }

        // 3. Détails d'une évaluation
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetEvaluationById(int id)
        {
            var evaluation = await _context.Evaluations
                .Include(e => e.Employe)
                .Include(e => e.Responsable)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (evaluation == null)
                return NotFound(new { message = "Evaluation non trouvée." });

            var user = await _userManager.GetUserAsync(User);

            if (User.IsInRole("Admin") ||
                (User.IsInRole("Manager") && evaluation.ResponsableId == user.Id) ||
                (User.IsInRole("Employe") && evaluation.EmployeId == user.Id))
            {
                return Ok(evaluation);
            }

            return Unauthorized();
        }

        // 4. Mes évaluations (employé connecté)
        [HttpGet("mesEvaluations")]
        [Authorize(Roles = "Employe")]
        public async Task<IActionResult> GetMyEvaluations()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var evaluations = await _context.Evaluations
                .Where(e => e.EmployeId == user.Id)
                .Include(e => e.Employe)
                .Include(e => e.Responsable)
                .ToListAsync();

            return Ok(evaluations);
        }

        // 5. Finaliser par Employé (PUT)
        [HttpPut("finaliserParEmploye/{id}")]
        [Authorize(Roles = "Employe")]
        public async Task<IActionResult> FinaliserParEmploye(int id, [FromBody] Evaluation evaluation)
        {
            var existingEvaluation = await _context.Evaluations.FindAsync(id);
            if (existingEvaluation == null)
                return NotFound(new { message = "Evaluation non trouvée." });

            if (existingEvaluation.FinaliseParEmploye)
                return BadRequest(new { message = "Evaluation déjà finalisée par l'employé." });

            var user = await _userManager.GetUserAsync(User);
            if (existingEvaluation.EmployeId != user.Id)
                return Unauthorized();

            existingEvaluation.CommentairesEmploye = evaluation.CommentairesEmploye;
            existingEvaluation.FinaliseParEmploye = true;
            await _context.SaveChangesAsync();

            return Ok(existingEvaluation);
        }

        // 6. Finaliser par Manager (PUT)
        [HttpPut("finaliserParManager/{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> FinaliserParManager(int id, [FromBody] Evaluation evaluation)
        {
            var existingEvaluation = await _context.Evaluations.FindAsync(id);
            if (existingEvaluation == null)
                return NotFound(new { message = "Evaluation non trouvée." });

            if (!existingEvaluation.FinaliseParEmploye)
                return BadRequest(new { message = "L'employé doit finaliser avant le manager." });

            if (existingEvaluation.FinaliseParManager)
                return BadRequest(new { message = "Evaluation déjà finalisée par le manager." });

            var user = await _userManager.GetUserAsync(User);
            if (existingEvaluation.ResponsableId != user.Id)
                return Unauthorized();

            existingEvaluation.CommentairesResponsable = evaluation.CommentairesResponsable;
            existingEvaluation.Note = evaluation.Note;
            existingEvaluation.FinaliseParManager = true;
            await _context.SaveChangesAsync();

            return Ok(existingEvaluation);
        }

        // 7. Approuver ou rejeter une évaluation (PUT)
        [HttpPut("approuver/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approuver(int id, [FromQuery] bool approuve)
        {
            var evaluation = await _context.Evaluations.FindAsync(id);
            if (evaluation == null)
                return NotFound(new { message = "Evaluation non trouvée." });

            if (!evaluation.FinaliseParEmploye || !evaluation.FinaliseParManager || evaluation.EstApprouve != EstApprouve.EnCours)
                return BadRequest(new { message = "Evaluation non éligible à l'approbation." });

            evaluation.EstApprouve = approuve ? EstApprouve.Oui : EstApprouve.Non;
            await _context.SaveChangesAsync();

            return Ok(evaluation);
        }

        // 8. Supprimer une évaluation (DELETE)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEvaluation(int id)
        {
            var evaluation = await _context.Evaluations.FindAsync(id);
            if (evaluation == null)
                return NotFound(new { message = "Evaluation non trouvée." });

            _context.Evaluations.Remove(evaluation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 9. Lancer campagne d'évaluation (POST)
        [HttpPost("LancerCampagne")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> LancerCampagne()
        {
            var user = await _userManager.GetUserAsync(User);
            var admin = await _context.Employes.FirstOrDefaultAsync(e => e.Id == user.Id);

            if (admin == null)
                return BadRequest("Administrateur introuvable.");

            var employes = await _context.Employes.Include(e => e.Equipe).ToListAsync();

            foreach (var employe in employes)
            {
                Employe responsable = null;

                var userEmploye = await _userManager.FindByIdAsync(employe.Id);
                var rolesEmploye = await _userManager.GetRolesAsync(userEmploye);
                var roleEmploye = rolesEmploye.FirstOrDefault();

                if (roleEmploye == "Admin")
                {
                    continue;
                }
                else if (roleEmploye == "Manager")
                {
                    responsable = admin;
                }
                else
                {
                    // Pour trouver le manager de l'équipe, on doit faire une boucle manuelle
                    var membresEquipe = employes.Where(e => e.EquipeId == employe.EquipeId && e.Id != employe.Id).ToList();

                    foreach (var membre in membresEquipe)
                    {
                        var userMembre = await _userManager.FindByIdAsync(membre.Id);
                        var rolesMembre = await _userManager.GetRolesAsync(userMembre);
                        if (rolesMembre.Contains("Manager"))
                        {
                            responsable = membre;
                            break;
                        }
                    }
                }

                if (responsable == null) continue;

                var evaluation = new Evaluation
                {
                    EmployeId = employe.Id,
                    Employe = employe,
                    ResponsableId = responsable.Id,
                    Responsable = responsable,
                    CommentairesEmploye = "",
                    CommentairesResponsable = "",
                    Note = 0,
                    FinaliseParEmploye = false,
                    FinaliseParManager = false,
                    EstApprouve = EstApprouve.EnCours,
                    DateEvaluation = DateTime.Now
                };

                _context.Evaluations.Add(evaluation);
            }

            await _context.SaveChangesAsync();

            return Ok("Campagne d'évaluation lancée avec succès.");
        }

    }
}
