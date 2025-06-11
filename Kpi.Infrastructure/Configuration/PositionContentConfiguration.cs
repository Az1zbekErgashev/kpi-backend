using Kpi.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Kpi.Infrastructure.Configuration
{
    public class PositionContentConfiguration : IEntityTypeConfiguration<Position>
    {
        public void Configure(EntityTypeBuilder<Position> builder)
        {
            builder.HasData(
                new Position { Id = 1, CreatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), UpdatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), Name = "personal_information_manager", IsDeleted = 0 },
                new Position { Id = 2, CreatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), UpdatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), Name = "advisor", IsDeleted = 0 },
                new Position { Id = 3, CreatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), UpdatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), Name = "manager", IsDeleted = 0 },
                new Position { Id = 4, CreatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), UpdatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), Name = "technical_security_officer", IsDeleted = 0 },
                new Position { Id = 5, CreatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), UpdatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), Name = "head_of_technical_security", IsDeleted = 0 },
                new Position { Id = 6, CreatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), UpdatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), Name = "assistant_manager", IsDeleted = 0 },
                new Position { Id = 7, CreatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), UpdatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), Name = "ceo", IsDeleted = 0 },
                new Position { Id = 8, CreatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), UpdatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), Name = "vice_president", IsDeleted = 0 },
                new Position { Id = 9, CreatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), UpdatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), Name = "staff", IsDeleted = 0 },
                new Position { Id = 10, CreatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), UpdatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), Name = "executive_director", IsDeleted = 0 },
                new Position { Id = 11, CreatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), UpdatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), Name = "senior_researcher", IsDeleted = 0 },
                new Position { Id = 12, CreatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), UpdatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), Name = "principal_researcher", IsDeleted = 0 },
                new Position { Id = 13, CreatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), UpdatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), Name = "researcher", IsDeleted = 0 },
                new Position { Id = 14, CreatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), UpdatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), Name = "director", IsDeleted = 0 },
                new Position { Id = 15, CreatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), UpdatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), Name = "personnel_security_manager", IsDeleted = 0 },
                new Position { Id = 16, CreatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), UpdatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), Name = "info_security_committee_chair", IsDeleted = 0 },
                new Position { Id = 17, CreatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), UpdatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), Name = "info_security_committee_member", IsDeleted = 0 },
                new Position { Id = 18, CreatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), UpdatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), Name = "lead_researcher", IsDeleted = 0 },
                new Position { Id = 19, CreatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), UpdatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), Name = "deputy_department_head", IsDeleted = 0 },
                new Position { Id = 20, CreatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), UpdatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), Name = "responsible_researcher", IsDeleted = 0 },
                new Position { Id = 21, CreatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), UpdatedAt = new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), Name = "chairman", IsDeleted = 0 }
             );
        }
    }
}
