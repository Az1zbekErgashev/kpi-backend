using Kpi.Domain.Commons;
using Kpi.Domain.Entities.Goal;
using Kpi.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace Kpi.Domain.Entities.Comment
{
    public class MonthlyTargetComment : Auditable
    {
        [StringLength(1000)]
        public string? Content { get; set; }
        public int CreatedById { get; set; }
        public int MonthlyPerformanceId { get; set; }
        public MonthlyPerformance MonthlyPerformance { get; set; }
        public virtual User.User CreatedBy { get; set; }
        public GoalStatus Status { get; set; }
    }
}
