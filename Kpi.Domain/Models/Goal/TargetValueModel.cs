using Kpi.Domain.Commons;
using Kpi.Domain.Enum;

namespace Kpi.Domain.Models.Goal
{
    public class TargetValueModel : Auditable
    {
        public TargetValueType Type { get; set; }
        public TargetStatus? Status { get; set; }
        public double? ValueRatio { get; set; }
        public double? ValueNumber { get; set; }
        public string? ValueText { get; set; }
        public string? EvaluationText { get; set; }

        public virtual TargetValueModel MapFromEntity(Entities.Goal.TargetValue entity)
        {
            Id = entity.Id;
            CreatedAt = entity.CreatedAt;
            UpdatedAt = entity.UpdatedAt;
            Type = entity.Type;
            Status = entity.Status;
            ValueNumber = entity.ValueNumber;
            ValueRatio = entity.ValueRatio;
            ValueText = entity.ValueText;
            EvaluationText = entity.EvaluationText;
            return this;
        }
    }
}
