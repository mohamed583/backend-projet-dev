using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projetERP.Models;
using System.Linq;
using System.Threading.Tasks;

namespace projetERP.Controllers
{
    [ApiController]
    [Route("conge")]
    [Authorize]
    public class CongeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Personne> _userManager;

        public CongeController(ApplicationDbContext context, UserManager<Personne> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Conge conge)
        {
            var user = await _userManager.GetUserAsync(User);
            var employe = await _context.Employes.FirstOrDefaultAsync(e => e.Id == user.Id);

            if (employe == null)
                return NotFound(new { message = "Employé non trouvé." });

            conge.EmployeId = employe.Id;
            conge.StatusInscription = StatusConge.EnCours;
            _context.Conges.Add(conge);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMesConges), new { id = conge.Id }, conge);
        }

        
        [HttpGet("mes-conges")]
        public async Task<IActionResult> GetMesConges()
        {
            var user = await _userManager.GetUserAsync(User);
            var employe = await _context.Employes.FirstOrDefaultAsync(e => e.Id == user.Id);

            if (employe == null)
                return NotFound(new { message = "Employé non trouvé." });

            var mesConges = await _context.Conges
                .Where(c => c.EmployeId == employe.Id)
                .ToListAsync();

            return Ok(mesConges);
        }


        [Authorize(Roles = "Manager,Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllConges()
        {
            var user = await _userManager.GetUserAsync(User);
            var employe = await _context.Employes.FirstOrDefaultAsync(e => e.Id == user.Id);
            var isAdmin = User.IsInRole("Admin");
            var isManager = User.IsInRole("Manager");
            if (employe == null)
                return NotFound(new { message = "Employé non trouvé." });
            if (!isAdmin && !isManager)
                return StatusCode(403, new { message = "Accès refusé." });
            if (User.IsInRole("Admin"))
            {
                var conges = await _context.Conges.Include(c => c.Employe).ToListAsync();
                return Ok(conges);
            }

            if (User.IsInRole("Manager"))
            {
                var equipeId = employe.EquipeId;
                var conges = await _context.Conges
                    .Include(c => c.Employe)
                    .Where(c => c.Employe.EquipeId == equipeId)
                    .ToListAsync();
                return Ok(conges);
            }

            return Forbid();
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPut("status/{id}")]
        public async Task<IActionResult> ModifierStatusConge(int id, [FromBody] StatusConge status)
        {
            var conge = await _context.Conges
                .Include(c => c.Employe)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (conge == null)
                return NotFound(new { message = "Congé non trouvé." });

            var user = await _userManager.GetUserAsync(User);
            var employeConnecte = await _context.Employes.FirstOrDefaultAsync(e => e.Id == user.Id);

            if (employeConnecte == null)
                return NotFound(new { message = "Employé non trouvé." });

            var isAdmin = User.IsInRole("Admin");
            var isManager = User.IsInRole("Manager") && employeConnecte.EquipeId == conge.Employe.EquipeId;

            if (!isAdmin && !isManager)
                return StatusCode(403, new { message = "Accès refusé." });

            conge.StatusInscription = status;
            _context.Conges.Update(conge);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Statut du congé mis à jour." });
        }

  
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var conge = await _context.Conges.FindAsync(id);

            if (conge == null || conge.StatusInscription != StatusConge.EnCours)
                return NotFound(new { message = "Congé introuvable ou ne peut pas être supprimé." });

            _context.Conges.Remove(conge);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Congé supprimé avec succès." });
        }
    }
}
