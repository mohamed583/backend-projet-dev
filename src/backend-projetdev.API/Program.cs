using backend_projetdev.API.Extensions;
using backend_projetdev.API.Filters;
using backend_projetdev.Application.DependencyInjection;
using backend_projetdev.Infrastructure.DependencyInjection;
using backend_projetdev.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
// 1. Add layered services
builder.Services.AddApplicationServices(); // Application layer (MediatR, AutoMapper, FluentValidation)
builder.Services.AddInfrastructure(builder.Configuration); // Infrastructure (EF, Identity, Repos, JWT)
builder.Services.AddPresentation(builder.Configuration); // Presentation (Swagger, Controllers, etc.)

var app = builder.Build();

// Applique automatiquement les migrations au d�marrage
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

// 2. Middleware pipeline
app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<TokenRevocationMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ERP API V1");
    c.RoutePrefix = string.Empty; // Acc�s direct � la racine
});

// 3. Init Roles
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Candidat", "Formateur", "Employe", "Manager", "Admin" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

// 4. Map Controllers
app.MapControllers();

app.Run();
