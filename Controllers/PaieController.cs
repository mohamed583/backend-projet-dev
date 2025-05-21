using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using projetERP.Models;
using projetERP.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace projetERP.Controllers
{
    [ApiController]
    [Route("paie")]
    [Authorize]
    public class PaieController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Personne> _userManager;

        public PaieController(ApplicationDbContext context, UserManager<Personne> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 1. Liste de toutes les paies (Admin seulement)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPaies()
        {
            var paies = await _context.Paies.Include(p => p.Personne).ToListAsync();
            return Ok(paies); // 200 OK
        }

        // 2. Paies de l'utilisateur connecté
        [HttpGet("mes-paies")]
        public async Task<IActionResult> GetMesPaies()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(); // 401

            var paies = await _context.Paies
                .Where(p => p.PersonneId == user.Id)
                .Include(p => p.Personne)
                .ToListAsync();

            return Ok(paies); // 200 OK
        }

        // 3. Détails d'une paie
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaie(int id)
        {
            var paie = await _context.Paies.Include(p => p.Personne).FirstOrDefaultAsync(p => p.Id == id);
            if (paie == null)
                return NotFound(); // 404

            return Ok(paie); // 200 OK
        }

        // 4. Créer une paie (Admin uniquement)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreatePaie([FromBody] CreatePaieViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // 400

            var personne = await _context.Personnes.FirstOrDefaultAsync(p => p.Id == viewModel.PersonneId);
            if (personne == null)
                return NotFound("Personne non trouvée"); // 404

            var paie = new Paie
            {
                PersonneId = viewModel.PersonneId,
                DatePaie = viewModel.DatePaie,
                Montant = viewModel.Montant,
                Description = viewModel.Description,
                Avantages = viewModel.Avantages,
                Retenues = viewModel.Retenues
            };

            _context.Paies.Add(paie);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPaie), new { id = paie.Id }, paie); // 201 Created
        }

        // 5. Générer la paie pour tous les employés
        [HttpPost("effectuer-tous-employes")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EffectuerPaiePourTous()
        {
            var employes = await _context.Employes.ToListAsync();

            if (!employes.Any())
                return NotFound("Aucun employé trouvé"); // 404

            var paies = new List<Paie>();
            foreach (var employe in employes)
            {
                paies.Add(new Paie
                {
                    PersonneId = employe.Id,
                    DatePaie = DateTime.Now,
                    Montant = employe.Salaire,
                    Description = "Paie mensuelle",
                    Avantages = "Aucun",
                    Retenues = "Aucune"
                });
            }

            _context.Paies.AddRange(paies);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Paies générées pour tous les employés", total = paies.Count }); // 200 OK
        }
    }
}
