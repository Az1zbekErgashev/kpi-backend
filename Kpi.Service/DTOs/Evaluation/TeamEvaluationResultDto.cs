using Kpi.Domain.Enum;

namespace Kpi.Service.DTOs.Evaluation
{
    public class TeamEvaluationResultDto
    {
        public int EmployeeId { get; set; }
        public Role Role { get; set; }
        public string? Position { get; set; }
        public string FullName { get; set; }
        public List<DivisionEvaluationDto>? DivisionEvaluations { get; set; }
    }
}
