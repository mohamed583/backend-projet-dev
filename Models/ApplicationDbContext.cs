using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace backend_projetdev.Models
{
    public class ApplicationDbContext : IdentityDbContext<Personne>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Personne> Personnes { get; set; }
        public DbSet<Candidat> Candidats { get; set; }
        public DbSet<Employe> Employes { get; set; }
        public DbSet<Formateur> Formateurs { get; set; }
        public DbSet<Formation> Formations { get; set; }
        public DbSet<Inscription> Inscriptions { get; set; }
        public DbSet<Evaluation> Evaluations { get; set; }
        public DbSet<Paie> Paies { get; set; }
        public DbSet<Conge> Conges { get; set; }
        public DbSet<Departement> Departements { get; set; }
        public DbSet<Equipe> Equipes { get; set; }
        public DbSet<Poste> Postes { get; set; }
        public DbSet<Candidature> Candidatures { get; set; }
        public DbSet<Entretien> Entretiens { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration des types dérivés
            modelBuilder.Entity<Candidat>().HasBaseType<Personne>();
            modelBuilder.Entity<Employe>().HasBaseType<Personne>();
            modelBuilder.Entity<Formateur>().HasBaseType<Personne>();
            modelBuilder.Entity<Personne>().ToTable("Personnes");

            // Configuration des relations
            modelBuilder.Entity<Inscription>()
                .HasOne(i => i.Employe)
                .WithMany(e => e.Inscriptions)
                .HasForeignKey(i => i.EmployeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Inscription>()
                .HasOne(i => i.Formation)
                .WithMany(f => f.Inscriptions)
                .HasForeignKey(i => i.FormationId);

            modelBuilder.Entity<Evaluation>()
                .HasOne(e => e.Employe)
                .WithMany(e => e.Evaluations)
                .HasForeignKey(e => e.EmployeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Paie>()
                .HasOne(p => p.Personne)
                .WithMany(p => p.Paies)
                .HasForeignKey(p => p.PersonneId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Conge>()
                .HasOne(c => c.Employe)
                .WithMany(e => e.Conges)
                .HasForeignKey(c => c.EmployeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employe>()
                .HasOne(e => e.Equipe)
                .WithMany(e => e.Employes)
                .HasForeignKey(e => e.EquipeId);

            modelBuilder.Entity<Formateur>()
                .HasMany(f => f.Formations)
                .WithOne(f => f.Formateur)
                .HasForeignKey(f => f.FormateurId);

            modelBuilder.Entity<Formation>()
                .HasOne(f => f.Formateur)
                .WithMany(f => f.Formations)
                .HasForeignKey(f => f.FormateurId);

            modelBuilder.Entity<Departement>()
                .HasMany(d => d.Equipes)
                .WithOne(e => e.Departement)
                .HasForeignKey(e => e.DepartementId);

            modelBuilder.Entity<Equipe>()
                .HasOne(e => e.Departement)
                .WithMany(d => d.Equipes)
                .HasForeignKey(e => e.DepartementId);

            modelBuilder.Entity<Equipe>()
                .HasMany(e => e.Employes)
                .WithOne(e => e.Equipe)
                .HasForeignKey(e => e.EquipeId);

            modelBuilder.Entity<Candidature>()
                .HasOne(cp => cp.Candidat)
                .WithMany(c => c.Candidatures)
                .HasForeignKey(cp => cp.CandidatId)
                .OnDelete(DeleteBehavior.Restrict); ;

            modelBuilder.Entity<Candidature>()
                .HasOne(cp => cp.Poste)
                .WithMany(p => p.Candidatures)
                .HasForeignKey(cp => cp.PosteId);

            modelBuilder.Entity<Entretien>()
                .HasOne(e => e.Candidature)
                .WithMany(c => c.Entretiens)
                .HasForeignKey(e => e.CandidatureId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}