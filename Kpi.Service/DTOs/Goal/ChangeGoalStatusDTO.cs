namespace Kpi.Service.DTOs.Goal
{
    public class ChangeGoalStatusDTO
    {
        public bool Status { get; set; }
        public int GoalId { get; set; }
        public string? Comment { get; set; }
    }
}
