using Kpi.Domain.Commons;
using Kpi.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace Kpi.Domain.Entities.Goal
{
    public class Goal : Auditable
    {

        [Required]
        public ICollection<Division> Divisions { get; set; }
        public virtual ICollection<Comment.Comment> Comments { get; set; }

        [Required]
        public int CreatedById { get; set; }
        public virtual User.User CreatedBy { get; set; }

        [Required]
        public GoalStatus Status { get; set; }
        public ICollection<MonthlyPerformance> MonthlyPerformance { get; set; }
    }
}
