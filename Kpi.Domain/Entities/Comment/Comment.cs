using Kpi.Domain.Commons;

namespace Kpi.Domain.Entities.Comment
{
    public class Comment : Auditable
    {
        public int GoalId { get; set; }
        public Entities.Goal.Goal Goal { get; set; }
        public string Text { get; set; }
    }
}
