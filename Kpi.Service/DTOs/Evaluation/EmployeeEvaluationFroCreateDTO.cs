using Kpi.Domain.Enum;

namespace Kpi.Service.DTOs.Evaluation
{
    public class EmployeeEvaluationFroCreateDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int KpiDivisionId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public Grade Grade { get; set; }
        public string Modifier { get; set; }
        public string Comment { get; set; }
    }
}
