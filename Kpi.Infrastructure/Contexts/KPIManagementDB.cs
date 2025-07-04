using Kpi.Domain.Entities;
using Kpi.Domain.Entities.Attachment;
using Kpi.Domain.Entities.Comment;
using Kpi.Domain.Entities.Country;
using Kpi.Domain.Entities.Goal;
using Kpi.Domain.Entities.MultilingualText;
using Kpi.Domain.Entities.Room;
using Kpi.Domain.Entities.Team;
using Kpi.Domain.Entities.User;
using Kpi.Infrastructure.Configuration;
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
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<KpiGoal> KpiGoals { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<TargetValue> TargetValues { get; set; }
        public DbSet<MonthlyPerformance> MonthlyPerformances { get; set; }
        public DbSet<MonthlyTargetValue> MonthlyTargetValues { get; set; }
        public DbSet<Evaluation> Evaluations { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<MonthlyTargetComment> MonthlyTargetComments { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>()
                .HasMany(t => t.Users)
                .WithOne(u => u.Team)
                .HasForeignKey(u => u.TeamId);

            modelBuilder.Entity<User>()
                .HasOne(t => t.Position)
                .WithMany()
                .HasForeignKey(u => u.PositionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Room>()
                .HasMany(t => t.Users)
                .WithOne(u => u.Room)
                .HasForeignKey(u => u.RoomId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.CreatedGoals)
                .WithOne(g => g.CreatedBy)
                .HasForeignKey(g => g.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Evaluation>()
                .HasOne(e => e.User)
                .WithMany(u => u.Evaluations)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);     
            
            modelBuilder.Entity<User>()
                .HasMany(e => e.Evaluations)
                .WithOne(u => u.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Goal>()
                .HasMany(x => x.Divisions)
                .WithOne(x => x.Goal)
                .HasForeignKey(g => g.GoalId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Goal>()
                .HasMany(g => g.Comments)
                .WithOne(e => e.Goal)
                .HasForeignKey(e => e.GoalId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Goal>()
                .HasMany(e => e.MonthlyPerformance)
                .WithOne(mt => mt.Goal)
                .HasForeignKey(mt => mt.GoalId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MonthlyPerformance>()
                .HasMany(e => e.MonthlyTargetComment)
                .WithOne(mt => mt.MonthlyPerformance)
                .HasForeignKey(mt => mt.MonthlyPerformanceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(e => e.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MonthlyPerformance>()
               .HasMany(m => m.MonthlyTargetValue)
               .WithOne(x => x.MonthlyPerformance)
               .HasForeignKey(t => t.MonthlyPerformanceId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<KpiGoal>(entity =>
            {
                entity.HasOne(e => e.Division)
                      .WithMany(d => d.Goals)
                      .HasForeignKey(e => e.DivisionId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.TargetValue)
                      .WithOne(tv => tv.KpiGoal)
                      .HasForeignKey<TargetValue>(tv => tv.KpiGoalId)
                      .OnDelete(DeleteBehavior.Cascade);
            });


            var staticUser = new User()
            {
                Id = 1,
                Role = 0,
                FullName = "System Admin",
                Password = "4224e31cf7876e3812095d34e1052b3a41174231789b1d4449842a72f005dc03",
                UserName = "CEO",
            };


            modelBuilder.Entity<User>().HasData(
               staticUser
            );
            modelBuilder.ApplyConfiguration(new CountryContentConfiguration());
            modelBuilder.ApplyConfiguration(new PositionContentConfiguration());
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }
}
