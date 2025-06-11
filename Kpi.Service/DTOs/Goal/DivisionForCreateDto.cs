namespace Kpi.Service.DTOs.Goal
{
    public class DivisionForCreateDto
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public double? Ratio { get; set; }
        public List<KpiGoalForCreateDto> Goals { get; set; }
    }
}
