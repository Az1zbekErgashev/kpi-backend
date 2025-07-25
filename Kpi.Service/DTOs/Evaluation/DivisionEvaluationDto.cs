using Kpi.Domain.Enum;

namespace Kpi.Service.DTOs.Evaluation
{
    public class DivisionEvaluationDto
    {
        public int KpiDivisionId { get; set; }
        public string DivisionName { get; set; }
        public string Grade { get; set; }
        public string? Modifier { get; set; }
        public string? Comment { get; set; }
        public double? Score { get; set; }
        public int? ScoreId { get; set; }
        public double? Ratio { get; set; }
        public int? Id { get; set; }
    }
}
