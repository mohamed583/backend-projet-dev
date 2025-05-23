using backend_projetdev.DTOs;
using backend_projetdev.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
namespace backend_projetdev.Controllers;
[Route("department")]
[ApiController]
[Authorize(Roles = "Admin")]
public class DepartementController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DepartementController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Lister les départements
    [HttpGet]
    public async Task<IActionResult> GetDepartements()
    {
        var departements = await _context.Departements
                                   .Include(d => d.Equipes)  // Inclus les équipes associées
                                   .ToListAsync();
        if (departements == null || !departements.Any())
        {
            return NotFound(new { message = "Aucun département trouvé." });
        }
        return Ok(departements); // Code 200 OK
    }

    // Créer un département
    [HttpPost]
    public async Task<IActionResult> CreateDepartement([FromBody] ManageDepartementDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { message = "Données invalides.", errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
        }

        var departement = new Departement
        {
            Nom = dto.Nom
        };

        _context.Departements.Add(departement);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDepartements), new { id = departement.Id }, departement);
    }


    // Modifier un département
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDepartement(int id, [FromBody] ManageDepartementDTO dto)
    {
        if (id <= 0) return BadRequest(new { message = "ID du département incorrect." });

        var departement = await _context.Departements.FindAsync(id);
        if (departement == null) return NotFound(new { message = "Département non trouvé." });

        // Met à jour uniquement le Nom
        departement.Nom = dto.Nom;

        _context.Departements.Update(departement);
        await _context.SaveChangesAsync();

        return Ok(departement); // Code 200 OK
    }

    // Supprimer un département
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDepartement(int id)
    {
        var departement = await _context.Departements
            .Include(d => d.Equipes)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (departement == null) return NotFound(new { message = "Département non trouvé." });

        if (departement.Equipes.Any())
        {
            return Conflict(new { message = "Impossible de supprimer un département qui contient des équipes." });
        }

        _context.Departements.Remove(departement);
        await _context.SaveChangesAsync();

        return NoContent(); // Code 204 No Content
    }

    // Afficher les détails d’un département
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDepartementDetails(int id)
    {
        var departement = await _context.Departements
            .Include(d => d.Equipes)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (departement == null) return NotFound(new { message = "Département non trouvé." });

        return Ok(departement); // Code 200 OK
    }
}
