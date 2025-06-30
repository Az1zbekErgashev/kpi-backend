using Kpi.Domain.Commons;

namespace Kpi.Domain.Models.Goal
{
    public class MonthlyTargetValueModel : Auditable
    {
        public double? ValueRatio { get; set; }
        public double? ValueRatioStatus { get; set; }
        public double? ValueNumber { get; set; }
        public string? ValueText { get; set; }
        public int TargetValueId { get; set; }
        public int MonthlyPerformanceId { get; set; }

        public virtual MonthlyTargetValueModel MapFromEntity(Domain.Entities.Goal.MonthlyTargetValue entity)
        {
            ValueRatioStatus = entity.ValueRatioStatus;
            ValueNumber = entity.ValueNumber;
            ValueText = entity.ValueText;
            ValueRatio = entity.ValueRatio;
            TargetValueId = entity.TargetValueId;
            MonthlyPerformanceId = entity.MonthlyPerformanceId;
            Id = entity.Id;
            CreatedAt = entity.CreatedAt;
            return this;
        }
    }
}
