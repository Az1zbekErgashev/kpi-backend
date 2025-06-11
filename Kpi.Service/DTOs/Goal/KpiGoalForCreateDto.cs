namespace Kpi.Service.DTOs.Goal
{
    public class KpiGoalForCreateDto
    {
        public int? Id { get; set; }
        public string GoalContent { get; set; }
        public int? AssignedToId { get; set; }
        public TargetValueForCreateDto TargetValue { get; set; }
    }
}
