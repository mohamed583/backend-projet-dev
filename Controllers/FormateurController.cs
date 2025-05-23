using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using backend_projetdev.Models;
using backend_projetdev.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System;

namespace backend_projetdev.Controllers
{
    [ApiController]
    [Route("formateur")]
    [Authorize(Roles = "Admin")]
    public class FormateurController : ControllerBase
    {
        private readonly UserManager<Personne> _userManager;
        private readonly SignInManager<Personne> _signInManager;
        private readonly ApplicationDbContext _dbContext;

        public FormateurController(UserManager<Personne> userManager, SignInManager<Personne> signInManager, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
        }

        // 1. Créer un formateur
        [HttpPost]
        public async Task<IActionResult> CreateFormateur([FromBody] RegisterFormateurDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var formateur = new Formateur
            {
                Id = Guid.NewGuid().ToString(),
                Nom = model.Nom,
                Prenom = model.Prenom,
                Email = model.Email,
                UserName = model.Email,
                Salaire = model.Salaire,
                Domaine = model.Domaine,
                Description = model.Description
            };

            var result = await _userManager.CreateAsync(formateur, model.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(formateur, "Formateur");

            return CreatedAtAction(nameof(GetFormateur), new { id = formateur.Id }, formateur);
        }

        // 2. Lister tous les formateurs
        [HttpGet]
        public async Task<IActionResult> GetFormateurs()
        {
            var formateurs = await _dbContext.Formateurs.ToListAsync();
            return Ok(formateurs);
        }

        // 3. Afficher un formateur spécifique
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFormateur(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Id non renseigné");

            var formateur = await _dbContext.Formateurs.FirstOrDefaultAsync(f => f.Id == id);
            if (formateur == null)
                return NotFound();

            return Ok(formateur);
        }

        // 4. Modifier un formateur
        [HttpPut("{id}")]
        public async Task<IActionResult> EditFormateur(string id, [FromBody] EditFormateurDTO model)
        {
            if (string.IsNullOrEmpty(id) || id != model.Id)
                return BadRequest("Id invalide");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var formateur = await _dbContext.Formateurs.FirstOrDefaultAsync(f => f.Id == id);
            if (formateur == null)
                return NotFound();

            formateur.Nom = model.Nom;
            formateur.Prenom = model.Prenom;
            formateur.Domaine = model.Domaine;
            formateur.Salaire = model.Salaire;
            formateur.Description = model.Description;

            _dbContext.Update(formateur);
            await _dbContext.SaveChangesAsync();

            return NoContent(); // 204 - modifié avec succès, pas de contenu retourné
        }

        // 5. Supprimer un formateur
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFormateur(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Id non renseigné");

            var formateur = await _dbContext.Formateurs.FirstOrDefaultAsync(f => f.Id == id);
            if (formateur == null)
                return NotFound();

            _dbContext.Formateurs.Remove(formateur);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        // 6. Changer les infos de login (email et mot de passe)
        [HttpPut("ChangeLoginInfo")]
        public async Task<IActionResult> ChangeLoginInfo([FromBody] ChangeLoginInfoDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByIdAsync(model.UserId);
            var formateur = await _dbContext.Formateurs.FirstOrDefaultAsync(e => e.Id == model.UserId);
            if (user == null || formateur == null)
                return NotFound();

            if (!string.IsNullOrEmpty(model.NewEmail) && model.NewEmail != user.Email)
            {
                var emailChangeResult = await _userManager.SetEmailAsync(user, model.NewEmail);
                if (!emailChangeResult.Succeeded)
                    return BadRequest(emailChangeResult.Errors);

                var usernameChangeResult = await _userManager.SetUserNameAsync(user, model.NewEmail);
                if (!usernameChangeResult.Succeeded)
                    return BadRequest(usernameChangeResult.Errors);
            }

            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                var passwordRemoveResult = await _userManager.RemovePasswordAsync(user);
                if (!passwordRemoveResult.Succeeded)
                    return BadRequest(passwordRemoveResult.Errors);

                var passwordAddResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
                if (!passwordAddResult.Succeeded)
                    return BadRequest(passwordAddResult.Errors);
            }

            return NoContent();
        }
    }
}
