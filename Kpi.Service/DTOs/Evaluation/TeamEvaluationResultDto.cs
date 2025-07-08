namespace Kpi.Service.DTOs.Evaluation
{
    public class TeamEvaluationResultDto
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; }

        public List<DivisionEvaluationDto> DivisionEvaluations { get; set; }
    }
}
