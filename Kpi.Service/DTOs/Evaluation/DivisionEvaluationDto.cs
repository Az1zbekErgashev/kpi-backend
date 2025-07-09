using Kpi.Domain.Enum;

namespace Kpi.Service.DTOs.Evaluation
{
    public class DivisionEvaluationDto
    {
        public int KpiDivisionId { get; set; }
        public string DivisionName { get; set; }
        public Grade? Grade { get; set; }
        public string? Modifier { get; set; }
        public string Comment { get; set; }
        public int Score { get; set; }
        public double? Ratio { get; set; }
    }
}
