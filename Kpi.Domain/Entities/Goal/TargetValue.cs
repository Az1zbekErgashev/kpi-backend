using Kpi.Domain.Commons;
using Kpi.Domain.Enum;

namespace Kpi.Domain.Entities.Goal
{
    public class TargetValue : Auditable
    {
        public int KpiGoalId { get; set; }
        public TargetValueType Type { get; set; }
        public TargetStatus? Status { get; set; }
        public double? ValueRatio { get; set; }
        public double? ValueNumber { get; set; }
        public string? ValueText { get; set; }
        public string? EvaluationText { get; set; }
        public virtual KpiGoal KpiGoal { get; set; }
    }
}
