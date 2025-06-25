using Kpi.Domain.Commons;
using System.ComponentModel.DataAnnotations;

namespace Kpi.Domain.Entities.Goal
{
    public class MonthlyTargetValue : Auditable
    {
        public double? ValueRatio { get; set; }
        public double? ValueRatioStatus { get; set; }
        public double? ValueNumber { get; set; }
        public string? ValueText { get; set; }

        [Required]
        public int TargetValueId { get; set; }
        public int MonthlyPerformanceId { get; set; }
        public MonthlyPerformance MonthlyPerformance { get; set; }
    }
}
