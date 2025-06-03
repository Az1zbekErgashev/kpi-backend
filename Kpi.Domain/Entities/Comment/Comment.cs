using Kpi.Domain.Commons;
using Kpi.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace Kpi.Domain.Entities.Comment
{
    public class Comment : Auditable
    {
        public int GoalId { get; set; }

        [StringLength(1000)]
        public string? Content { get; set; }
        public int CreatedById { get; set; }
        public virtual Goal.Goal Goal { get; set; }
        public virtual User.User CreatedBy { get; set; }
        public GoalStatus Status { get; set; }
    }
}
