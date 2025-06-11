
using Kpi.Domain.Commons;
using Kpi.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace Kpi.Domain.Entities.User;
public class User : Auditable
{
    [Required]
    public string UserName { get; set; }

    [Required]
    public string Password { get; set; }
    public string FullName { get; set; }

    [Required]
    public Role Role { get; set; }

    public int? TeamId { get; set; }
    public Team.Team Team { get; set; }

    public int? RoomId { get; set; }
    public Room.Room Room { get; set; }
    public Position? Position { get; set; }
    public int? PositionId { get; set; }

    public ICollection<Goal.Goal> CreatedGoals { get; set; }
    public ICollection<Goal.Goal> AssignedGoals { get; set; }
    public ICollection<Evaluation> Evaluations { get; set; }
}
