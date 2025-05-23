using Kpi.Domain.Entities;
using Kpi.Domain.Entities.Attachment;
using Kpi.Domain.Entities.Country;
using Kpi.Domain.Entities.Goal;
using Kpi.Domain.Entities.MultilingualText;
using Kpi.Domain.Entities.Role;
using Kpi.Domain.Entities.Team;
using Kpi.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
namespace Kpi.Infrastructure.Contexts
{
    public class KpiDB : DbContext
    {
        public KpiDB(DbContextOptions<KpiDB> options) : base(options)
        {
        }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Country> Countrys { get; set; }
        public DbSet<MultilingualText> MultilingualText { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<Evaluation> Evaluations { get; set; }
        public DbSet<Role> Roles { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Role>()
                .HasMany(r => r.Users)
                .WithOne(u => u.Role)
                .HasForeignKey(u => u.RoleId);

            modelBuilder.Entity<Team>()
                .HasMany(t => t.Users)
                .WithOne(u => u.Team)
                .HasForeignKey(u => u.TeamId);

            modelBuilder.Entity<Team>()
                .HasOne(t => t.Director)
                .WithMany()
                .HasForeignKey(t => t.DirectorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.CreatedGoals)
                .WithOne(g => g.CreatedBy)
                .HasForeignKey(g => g.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.AssignedGoals)
                .WithOne(g => g.AssignedTo)
                .HasForeignKey(g => g.AssignedToId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Goal>()
                .HasMany(g => g.Evaluations)
                .WithOne(e => e.Goal)
                .HasForeignKey(e => e.GoalId);

            modelBuilder.Entity<Evaluation>()
                .HasOne(e => e.EvaluatedBy)
                .WithMany(u => u.Evaluations)
                .HasForeignKey(e => e.EvaluatedById)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.ApplyConfiguration(new CountryContentConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
