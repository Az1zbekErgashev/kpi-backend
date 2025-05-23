

using Kpi.Domain.Commons;

namespace Kpi.Domain.Entities
{
    public class Evaluation : Auditable
    {
        public int GoalId { get; set; }
        public Goal.Goal Goal { get; set; }

        public int EvaluatedById { get; set; }
        public User.User EvaluatedBy { get; set; }

        public string Grade { get; set; } 
        public string Comment { get; set; }
    }
}
