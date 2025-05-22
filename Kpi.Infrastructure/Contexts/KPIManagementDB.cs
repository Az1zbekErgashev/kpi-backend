using Kpi.Domain.Entities.Attachment;
using Kpi.Domain.Entities.Country;
using Kpi.Domain.Entities.MultilingualText;
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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CountryContentConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
