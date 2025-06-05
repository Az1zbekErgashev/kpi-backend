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

        public virtual UserModelForCEO MapFromEntity(Entities.User.User entity, string? year)
        {
            Id = entity.Id;
            UserName = entity.UserName;
            FullName = entity.FullName;
            Room = entity?.Room?.Name;
            Team = entity?.Team?.Name;
            RoomId = entity?.RoomId;
            TeamId = entity?.TeamId;
            Status = entity?.CreatedGoals
             .FirstOrDefault(x => x.CreatedAt.Year == DateTime.UtcNow.Year)?.Status
             ?? GoalStatus.NoWritte;
            Year = year;
            return this;
        }
    }
}
