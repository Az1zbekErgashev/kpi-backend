using Kpi.Domain.Enum;

namespace Kpi.Domain.Models.User
{
    public class UserModel
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public Role Role { get; set; }
        public int? TeamId { get; set; }
        public int? RoomId { get; set; }
        public string? Team { get; set; }
        public string? Room { get; set; }
        public int IsDeleted { get; set; }
        public PositionModel? Position { get; set; }
        public int? PositionId { get; set; }

        public virtual UserModel MapFromEntity(Entities.User.User entity)
        {
            UserName = entity.UserName;
            FullName = entity.FullName;
            Role = entity.Role;
            TeamId = entity?.TeamId;
            CreatedAt = entity.CreatedAt;
            UpdatedAt = entity.UpdatedAt;
            Id = entity.Id;
            Team = entity?.Team?.Name;
            Room = entity?.Room?.Name;
            RoomId = entity?.RoomId;
            Position = entity?.Position is null ? null : new PositionModel().MapFromEntity(entity.Position);
            PositionId = entity?.PositionId;
            return this;
        }
    }
}
