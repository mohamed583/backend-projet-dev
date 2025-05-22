using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend_projetdev.Models;
using backend_projetdev.ViewModels;
using System.Security.Claims;

namespace backend_projetdev.Controllers
{
    [ApiController]
    [Route("entretien")]
    [Authorize]
    public class EntretienController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public EntretienController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] EntretienCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var candidature = await _context.Candidatures
                .FirstOrDefaultAsync(c => c.Id == model.CandidatureId);

            if (candidature == null)
                return NotFound("La candidature spécifiée est introuvable.");

            var entretien = new Entretien
            {
                Id = Guid.NewGuid().ToString(),
                CandidatureId = model.CandidatureId,
                EmployeId = model.EmployeId,
                DateEntretien = model.DateEntretien,
                Commentaire = "",
                Status = StatusEntretien.NonCommence
            };

            _context.Entretiens.Add(entretien);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDetails), new { id = entretien.Id }, entretien);
        }


        [HttpGet("Candidature/{candidatureId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByCandidature(string candidatureId)
        {
            var entretiens = await _context.Entretiens
                .Include(e => e.Employe)
                .Where(e => e.CandidatureId == candidatureId)
                .ToListAsync();

            return Ok(entretiens);
        }


        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDetails(string id)
        {
            var entretien = await _context.Entretiens
                .Include(e => e.Candidature)
                .Include(e => e.Employe)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (entretien == null)
                return NotFound();

            return Ok(entretien);
        }


        [HttpGet("Employe")]
        [Authorize(Roles = "Employe")]
        public async Task<IActionResult> GetForEmploye()
        {
            var employeId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(employeId))
                return Unauthorized();

            var entretiens = await _context.Entretiens
                .Where(e => e.EmployeId == employeId && e.Status != StatusEntretien.Finalise)
                .ToListAsync();

            return Ok(entretiens);
        }


        [HttpGet("Employe/NonFinalise")]
        [Authorize(Roles = "Employe")]
        public async Task<IActionResult> GetNonFinalizedForEmploye()
        {
            var employeId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(employeId))
                return Unauthorized();

            var entretiens = await _context.Entretiens
                .Include(e => e.Candidature)
                .ThenInclude(c => c.Candidat)
                .Where(e => e.EmployeId == employeId && e.Status != StatusEntretien.Finalise)
                .ToListAsync();

            return Ok(entretiens);
        }


        [HttpPut("Complete/{id}")]
        [Authorize(Roles = "Employe")]
        public async Task<IActionResult> Complete(string id, [FromBody] EntretienCompleteViewModel model)
        {
            var employeId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(employeId))
                return Unauthorized();

            var entretien = await _context.Entretiens
                .FirstOrDefaultAsync(e => e.Id == id && e.EmployeId == employeId && e.Status != StatusEntretien.Finalise);

            if (entretien == null)
                return NotFound();

            entretien.Status = StatusEntretien.Finalise;
            entretien.Commentaire = model.Commentaire;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Entretien finalisé avec succès." });
        }
    }
}
