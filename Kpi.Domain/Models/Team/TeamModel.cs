using System.ComponentModel.DataAnnotations;

namespace Kpi.Domain.Models.Team
{
    public class TeamModel
    {
        [Required]
        public string Name { get; set; }
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int EmplyeesCount { get; set; }
        public int IsDeleted { get; set; }

        public virtual TeamModel MapFromEntity(Entities.Team.Team entity)
        {
            Name = entity.Name;
            Id = entity.Id;
            CreatedAt = entity.CreatedAt;
            EmplyeesCount = entity.Users != null ? entity.Users.Count() : 0;
            return this;
        }
    }
}
