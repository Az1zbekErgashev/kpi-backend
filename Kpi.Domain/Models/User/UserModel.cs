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


        public virtual UserModel MapFromEntity(Entities.User.User entity)
        {
            UserName = entity.UserName;
            FullName = entity.FullName;
            Role = entity.Role;
            TeamId = entity.TeamId;
            CreatedAt = entity.CreatedAt;
            UpdatedAt = entity.UpdatedAt;
            Id = entity.Id;
            Team = entity?.Team?.IsDeleted == 0 ? entity?.Team?.Name : null;
            IsDeleted = entity.IsDeleted;
            Room = entity?.Room?.IsDeleted == 0 ? entity?.Room?.Name : null;
            return this;
        }
    }
}
