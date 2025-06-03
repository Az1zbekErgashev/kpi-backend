using System.ComponentModel.DataAnnotations;
namespace Kpi.Service.DTOs.Goal
{
    public class KpiGoalForCreateDto
    {
        public int? Id { get; set; }

        [Required]
        public string GoalContent { get; set; }
        public int? AssignedToId { get; set; }

        [Required]
        public TargetValueForCreateDto TargetValue { get; set; }
    }
}
