namespace Kpi.Service.DTOs.Goal
{
    public class CreateMonthlyTargetGroupDto
    {
        public List<CreateMonthlyTargetItemDto> Targets { get; set; }
        public int GoalId { get; set; }
        public string Comment { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
