using Kpi.Domain.Commons;
using Kpi.Domain.Enum;

namespace Kpi.Domain.Models.User
{
    public class UserModelForCEO : Auditable
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public int? RoomId { get; set; }
        public int? TeamId { get; set; }
        public string? Team { get; set; }
        public string? Room { get; set; }
        public GoalStatus Status { get; set; }
        public string? Year { get; set; }
        public PositionModel? Position { get; set; }

        public virtual UserModelForCEO MapFromEntity(Entities.User.User entity, string? year)
        {
            int parsedYear = !string.IsNullOrEmpty(year) ? int.Parse(year) : DateTime.UtcNow.Year;

            Id = entity.Id;
            UserName = entity.UserName;
            FullName = entity.FullName;
            Room = entity?.Room?.IsDeleted == 0 ? entity?.Room?.Name : null;
            Team = entity?.Team?.IsDeleted == 0 ? entity?.Team?.Name : null;
            RoomId = entity?.Room?.IsDeleted == 0 ? entity?.RoomId : null;
            TeamId = entity?.Team?.IsDeleted == 0 ? entity?.TeamId : null;
            Status = entity?.CreatedGoals
             .FirstOrDefault(x => x.CreatedAt.Year == parsedYear && x.IsDeleted == 0)?.Status
             ?? GoalStatus.NoWritte;
            Year = year;
            Position = entity?.Position is null ? null : new PositionModel().MapFromEntity(entity.Position);
            return this;
        }
    }
}
