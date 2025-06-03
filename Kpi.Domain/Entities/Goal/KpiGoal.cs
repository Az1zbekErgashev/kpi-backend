using Kpi.Domain.Commons;
using System.ComponentModel.DataAnnotations;

namespace Kpi.Domain.Entities.Goal
{
    public class KpiGoal : Auditable
    {
        [Required]
        public int DivisionId { get; set; }

        [Required]
        [StringLength(500)]
        public string GoalContent { get; set; }

        [Required]
        public virtual TargetValue TargetValue { get; set; }
        public virtual Division Division { get; set; }
    }
}
