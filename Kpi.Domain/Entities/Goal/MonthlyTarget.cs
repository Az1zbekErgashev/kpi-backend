using Kpi.Domain.Commons;

namespace Kpi.Domain.Entities.Goal
{
    public class MonthlyTarget : Auditable
    {
        public int GoalId { get; set; }
        public string? TargetValueText { get; set; }
        public double? TargetValueNumber { get; set; }
        public double? TargetValueRatio { get; set; }
        public string? TargetEvaluationText { get; set; }
        public virtual Goal Goal { get; set; }
        public virtual User.User CreatedBy { get; set; }
        public int CreatedById { get; set; }
        public int? AssignedToId { get; set; }
        public virtual User.User AssignedTo { get; set; }
    }
}
