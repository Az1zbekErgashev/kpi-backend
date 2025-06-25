using System.ComponentModel.DataAnnotations;

namespace Kpi.Service.DTOs.Goal
{
    public class CreateMonthlyTargetItemDto
    {
        public double? ValueRatio { get; set; }
        public double? ValueRatioStatus { get; set; }
        public double? ValueNumber { get; set; }
        public string? ValueText { get; set; }

        [Required]
        public int TargetValueId { get; set; }
    }
}
