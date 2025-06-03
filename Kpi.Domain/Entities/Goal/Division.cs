using Kpi.Domain.Commons;
using System.ComponentModel.DataAnnotations;

namespace Kpi.Domain.Entities.Goal
{
    public class Division : Auditable
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public virtual ICollection<KpiGoal> Goals { get; set; }

        public virtual Goal Goal { get; set; }

        [Required]
        public int GoalId { get; set; }
    }
}
