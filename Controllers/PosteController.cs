using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend_projetdev.Models;

namespace backend_projetdev.Controllers
{
    [ApiController]
    [Route("poste")]
    [Authorize]
    public class PosteController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public PosteController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

       
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var postes = await _dbContext.Postes.Include(p => p.Candidatures).ToListAsync();
            return Ok(postes); // 200
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var poste = await _dbContext.Postes.Include(p => p.Candidatures).FirstOrDefaultAsync(p => p.Id == id);
            if (poste == null) return NotFound(); // 404
            return Ok(poste); // 200
        }

        
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] Poste poste)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState); // 400

            _dbContext.Postes.Add(poste);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = poste.Id }, poste); // 201
        }

        
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] Poste poste)
        {
            if (id != poste.Id) return BadRequest("ID mismatch"); // 400
            if (!ModelState.IsValid) return BadRequest(ModelState); // 400

            _dbContext.Entry(poste).State = EntityState.Modified;
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_dbContext.Postes.Any(p => p.Id == id)) return NotFound(); // 404
                throw;
            }
            return NoContent(); // 204
        }

        
        [HttpPatch("statut/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ModifierStatut(int id, [FromQuery] StatutPoste status)
        {
            var poste = await _dbContext.Postes.FindAsync(id);
            if (poste == null) return NotFound(); // 404

            poste.StatutPoste = status;
            _dbContext.Postes.Update(poste);
            await _dbContext.SaveChangesAsync();
            return NoContent(); // 204
        }

        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var poste = await _dbContext.Postes.FindAsync(id);
            if (poste == null) return NotFound(); // 404

            _dbContext.Postes.Remove(poste);
            await _dbContext.SaveChangesAsync();
            return NoContent(); // 204
        }

        
        [HttpGet("{id}/candidatures")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCandidatures(int id)
        {
            var poste = await _dbContext.Postes.FindAsync(id);
            if (poste == null) return NotFound(); // 404

            var candidatures = await _dbContext.Candidatures.Include(c => c.Candidat).Where(c => c.PosteId == id).ToListAsync();
            return Ok(candidatures); // 200
        }

        
        [HttpPost("{id}/apply")]
        [Authorize(Roles = "Candidat")]
        public async Task<IActionResult> Apply(int id, IFormFile cvFile)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized(); // 401

            if (_dbContext.Candidatures.Any(c => c.CandidatId == userId && c.PosteId == id))
                return Conflict("Vous avez déjà postulé à ce poste."); // 409

            string cvFilePath = null;
            if (cvFile != null && cvFile.Length > 0)
            {
                var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsDir);
                var filePath = Path.Combine(uploadsDir, cvFile.FileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await cvFile.CopyToAsync(stream);
                cvFilePath = "/uploads/" + cvFile.FileName;
            }

            var candidature = new Candidature
            {
                Id = Guid.NewGuid().ToString(),
                CandidatId = userId,
                PosteId = id,
                Status = Status.EnCours,
                CVPath = cvFilePath
            };

            _dbContext.Candidatures.Add(candidature);
            await _dbContext.SaveChangesAsync();
            return Ok("Votre candidature a été enregistrée avec succès."); // 200
        }
    }
}
