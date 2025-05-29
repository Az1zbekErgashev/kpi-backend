namespace Kpi.Domain.Models.Room
{
    public class RoomModel
    {
        public string Name { get; set; }
        public DateTime? CreateAt { get; set; }
        public int Id { get; set; }
        public int TeamsCount { get; set; }
        public int IsDeleted { get; set; }
        public virtual RoomModel MapFromEntity(Entities.Room.Room entity)
        {
            Name = entity.Name;
            CreateAt = entity.CreatedAt;
            Id = entity.Id;
            TeamsCount = entity.Users != null ? entity.Users.Where(u => u.TeamId != null && u.IsDeleted == 0 && u?.Team?.IsDeleted == 0)
            .Select(u => u.TeamId)
            .Distinct()
            .Count() : 0;
            IsDeleted = entity.IsDeleted;
            return this;
        }
    }
}
