using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend_projetdev.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace backend_projetdev.Controllers
{
    [ApiController]
    [Route("formation")]
    public class FormationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FormationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Formation/formateur/{formateurId}
        [HttpGet("formateur/{formateurId}")]
        public async Task<ActionResult<IEnumerable<Formation>>> GetFormationsByFormateur(string formateurId)
        {
            var formations = await _context.Formations
                .Where(f => f.FormateurId == formateurId)
                .Include(f => f.Formateur)
                .ToListAsync();

            return Ok(formations);
        }

        // GET: api/Formation/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Formation>> GetFormation(int id)
        {
            var formation = await _context.Formations
                .Include(f => f.Formateur)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (formation == null)
                return NotFound(new { message = "Formation non trouvée." });

            return Ok(formation);
        }

        // POST: api/Formation
        [HttpPost]
        public async Task<ActionResult<Formation>> CreateFormation(Formation formation)
        {
            var formateur = await _context.Formateurs.FirstOrDefaultAsync(f => f.Id == formation.FormateurId);
            if (formateur == null)
                return BadRequest(new { message = "Formateur introuvable." });

            formation.Formateur = formateur;

            _context.Formations.Add(formation);
            await _context.SaveChangesAsync();

            // Retourne 201 Created avec l'URL pour récupérer la ressource créée
            return CreatedAtAction(nameof(GetFormation), new { id = formation.Id }, formation);
        }

        // PUT: api/Formation/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateFormation(int id, Formation formation)
        {
            if (id != formation.Id)
                return BadRequest(new { message = "L'ID de la formation ne correspond pas." });

            var existingFormation = await _context.Formations.FindAsync(id);
            if (existingFormation == null)
                return NotFound(new { message = "Formation non trouvée." });

            // Met à jour les propriétés
            existingFormation.Titre = formation.Titre;
            existingFormation.Description = formation.Description;
            existingFormation.DateDebut = formation.DateDebut;
            existingFormation.DateFin = formation.DateFin;
            existingFormation.FormateurId = formation.FormateurId;
            existingFormation.Cout = formation.Cout;
            // ajoute d'autres propriétés si besoin

            _context.Entry(existingFormation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FormationExists(id))
                    return NotFound(new { message = "Formation non trouvée lors de la mise à jour." });
                else
                    throw;
            }

            return NoContent(); // 204 No Content
        }

        // DELETE: api/Formation/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteFormation(int id)
        {
            var formation = await _context.Formations.FindAsync(id);
            if (formation == null)
                return NotFound(new { message = "Formation non trouvée." });

            _context.Formations.Remove(formation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FormationExists(int id)
        {
            return _context.Formations.Any(e => e.Id == id);
        }
    }
}
