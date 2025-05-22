using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend_projetdev.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace backend_projetdev.Controllers
{
    [ApiController]
    [Route("equipe")]
    [Authorize(Roles = "Admin")]
    public class EquipeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public EquipeController(ApplicationDbContext context)
        {
            _context = context;
        }

       
        [HttpGet("Departement/{departementId}")]
        public async Task<IActionResult> GetEquipesByDepartement(int departementId)
        {
            var departement = await _context.Departements
                .Include(d => d.Equipes)
                .FirstOrDefaultAsync(d => d.Id == departementId);

            if (departement == null)
                return NotFound(new { message = "Département introuvable." });

            return Ok(departement.Equipes);
        }

       
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEquipe(int id)
        {
            var equipe = await _context.Equipes
                .Include(e => e.Employes)
                .Include(e => e.Departement)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (equipe == null)
                return NotFound(new { message = "Équipe introuvable." });

            return Ok(equipe);
        }

     
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Equipe model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var departement = await _context.Departements.FindAsync(model.DepartementId);
            if (departement == null)
                return NotFound(new { message = "Département associé introuvable." });

            var equipe = new Equipe
            {
                Nom = model.Nom,
                DepartementId = model.DepartementId
            };

            _context.Equipes.Add(equipe);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEquipe), new { id = equipe.Id }, equipe);
        }

       
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Equipe model)
        {
            if (id != model.Id)
                return BadRequest(new { message = "L'identifiant ne correspond pas." });

            var equipe = await _context.Equipes.FindAsync(id);
            if (equipe == null)
                return NotFound(new { message = "Équipe introuvable." });

            equipe.Nom = model.Nom;
            equipe.DepartementId = model.DepartementId;

            _context.Equipes.Update(equipe);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Équipe mise à jour avec succès." });
        }

       
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var equipe = await _context.Equipes
                .Include(e => e.Employes)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (equipe == null)
                return NotFound(new { message = "Équipe introuvable." });

            _context.Equipes.Remove(equipe);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 No Content
        }
    }
}