namespace Kpi.Domain.Models.Room
{
    public class RoomModel
    {
        public string Name { get; set; }
        public DateTime? CreateAt { get; set; }
        public int Id { get; set; }
        public int TeamsCount { get; set; }
        public virtual RoomModel MapFromEntity(Entities.Room.Room entity)
        {
            Name = entity.Name;
            CreateAt = entity.CreatedAt;
            Id = entity.Id;
            TeamsCount = entity.Users != null ? entity.Users.Where(u => u.TeamId != null)
            .Select(u => u.TeamId)
            .Distinct()
            .Count() : 0;
            return this;
        }
    }
}
