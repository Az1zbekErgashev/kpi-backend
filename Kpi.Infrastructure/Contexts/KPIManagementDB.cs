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
        public DbSet<ScoreManagement> ScoreManagements { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>()
                 .HasMany(t => t.Users)
                 .WithOne(u => u.Team)
                 .HasForeignKey(u => u.TeamId)
                 .OnDelete(DeleteBehavior.SetNull); // Team — не удаляется

            modelBuilder.Entity<Room>()
                .HasMany(t => t.Users)
                .WithOne(u => u.Room)
                .HasForeignKey(u => u.RoomId)
                .OnDelete(DeleteBehavior.SetNull); // Room — не удаляется

            modelBuilder.Entity<User>()
                .HasOne(t => t.Position)
                .WithMany()
                .HasForeignKey(u => u.PositionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.CreatedGoals)
                .WithOne(g => g.CreatedBy)
                .HasForeignKey(g => g.CreatedById)
                .OnDelete(DeleteBehavior.Cascade); // ✅ Удаляем Goal, если удаляется User

            modelBuilder.Entity<User>()
                .HasMany(e => e.Evaluations)
                .WithOne(u => u.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade); // ✅ Удаляем оценки при удалении User

            modelBuilder.Entity<Evaluation>()
                .HasOne(e => e.User)
                .WithMany(u => u.Evaluations)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Evaluation>()
                .HasOne(e => e.ScoreManagement)
                .WithMany()
                .HasForeignKey(e => e.ScoreManagementId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Evaluation>()
                .HasOne(e => e.KpiDivision)
                .WithMany()
                .HasForeignKey(e => e.KpiDivisionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ScoreManagement>()
                .HasOne(t => t.Division)
                .WithMany()
                .HasForeignKey(u => u.DivisionId)
                .OnDelete(DeleteBehavior.Cascade); // ✅ Удаление при удалении Division

            modelBuilder.Entity<Goal>()
                .HasMany(x => x.Divisions)
                .WithOne(x => x.Goal)
                .HasForeignKey(g => g.GoalId)
                .OnDelete(DeleteBehavior.Cascade); // ✅ Удаляются Divisions при удалении Goal

            modelBuilder.Entity<Goal>()
                .HasMany(g => g.Comments)
                .WithOne(e => e.Goal)
                .HasForeignKey(e => e.GoalId)
                .OnDelete(DeleteBehavior.Cascade); // ✅ Удаляются Comments

            modelBuilder.Entity<Goal>()
                .HasMany(e => e.MonthlyPerformance)
                .WithOne(mt => mt.Goal)
                .HasForeignKey(mt => mt.GoalId)
                .OnDelete(DeleteBehavior.Cascade); // ✅ Удаляются MonthlyPerformance

            modelBuilder.Entity<MonthlyPerformance>()
                .HasMany(e => e.MonthlyTargetComment)
                .WithOne(mt => mt.MonthlyPerformance)
                .HasForeignKey(mt => mt.MonthlyPerformanceId)
                .OnDelete(DeleteBehavior.Cascade); // ✅ Удаляются комментарии к MonthlyPerformance

            modelBuilder.Entity<MonthlyPerformance>()
               .HasMany(m => m.MonthlyTargetValue)
               .WithOne(x => x.MonthlyPerformance)
               .HasForeignKey(t => t.MonthlyPerformanceId)
               .OnDelete(DeleteBehavior.Cascade); // ✅ Удаляются MonthlyTargetValue

            modelBuilder.Entity<Comment>()
                .HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(e => e.CreatedById)
                .OnDelete(DeleteBehavior.Restrict); // Не удаляем пользователя при наличии комментариев

            modelBuilder.Entity<Division>()
                .HasMany(d => d.Goals)
                .WithOne(k => k.Division)
                .HasForeignKey(k => k.DivisionId)
                .OnDelete(DeleteBehavior.Cascade); // ✅ Удаляются KpiGoals при удалении Division

            modelBuilder.Entity<KpiGoal>()
                .HasOne(e => e.TargetValue)
                .WithOne(tv => tv.KpiGoal)
                .HasForeignKey<TargetValue>(tv => tv.KpiGoalId)
                .OnDelete(DeleteBehavior.Cascade); 


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
