using Kpi.Domain.Commons;
using Kpi.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace Kpi.Domain.Entities.Goal;
public class MonthlyPerformance : Auditable
{
    [Required]
    public int GoalId { get; set; }
    public virtual Goal Goal { get; set; }
    [Required]
    public int Year { get; set; }
    [Required]
    public int Month { get; set; }
    public GoalStatus Status { get; set; } = GoalStatus.PendingReview;
    public virtual ICollection<MonthlyTargetValue> MonthlyTargetValue { get; set; }
    public virtual ICollection<Entities.Comment.MonthlyTargetComment> MonthlyTargetComment { get; set; }
    public bool IsSended { get; set; }
}
