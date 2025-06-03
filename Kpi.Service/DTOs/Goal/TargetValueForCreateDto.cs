using Kpi.Domain.Enum;
namespace Kpi.Service.DTOs.Goal
{
    public class TargetValueForCreateDto
    {
        public TargetValueType? Type { get; set; }
        public TargetStatus? Status { get; set; }
        public double? ValueRatio { get; set; }
        public double? ValueNumber { get; set; }
        public string? ValueText { get; set; }
        public string? EvaluationText { get; set; }
    }
}
