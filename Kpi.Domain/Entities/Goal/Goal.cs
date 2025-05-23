using Kpi.Domain.Commons;
using Kpi.Domain.Enum;

namespace Kpi.Domain.Entities.Goal
{
    public class Goal : Auditable
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Rated { get; set; }
        public GoalType Type { get; set; } 
        public GoalStatus Status { get; set; } 

        public int CreatedById { get; set; }
        public User.User CreatedBy { get; set; }

        public int? AssignedToId { get; set; }
        public User.User AssignedTo { get; set; }

        public ICollection<Evaluation> Evaluations { get; set; }
    }
}
