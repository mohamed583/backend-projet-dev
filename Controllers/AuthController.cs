using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using projetERP.Models;
using projetERP.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace projetERP.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<Personne> _userManager;
        private readonly SignInManager<Personne> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<Personne> userManager, SignInManager<Personne> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        // Endpoint de connexion
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim(ClaimTypes.Email, user.Email)
                        };

                        foreach (var role in roles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role));
                        }

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(
                            issuer: _configuration["Jwt:Issuer"],
                            audience: _configuration["Jwt:Audience"],
                            claims: claims,
                            expires: DateTime.Now.AddHours(1),
                            signingCredentials: creds
                        );

                        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                        return Ok(new { Token = tokenString });
                    }
                }

                return Unauthorized(new { message = "Invalid login attempt" });
            }

            return BadRequest(new { message = "Invalid model" });
        }

        // Inscription (enregistrement d'un nouvel utilisateur)
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new Candidat
                {
                    Id = Guid.NewGuid().ToString(),
                    Nom = model.Nom,
                    Prenom = model.Prenom,
                    Email = model.Email,
                    UserName = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Candidat");

                    // Générez le token JWT pour l'utilisateur inscrit
                    var token = await GenerateJwtToken(user);

                    return Ok(new { Token = token });
                }

                return BadRequest(new { message = "User creation failed", errors = result.Errors });
            }

            return BadRequest(new { message = "Invalid model" });
        }

        // Fonction pour générer un token JWT
        private async Task<string> GenerateJwtToken(Personne user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Déconnexion (simplement pour l'API, la gestion se fait côté frontend)
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Pas nécessaire de gérer ici, car la déconnexion se fait côté client avec le retrait du token
            return Ok(new { message = "Logged out successfully" });
        }
    }
}
