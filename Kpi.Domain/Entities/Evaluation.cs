using Kpi.Domain.Commons;

namespace Kpi.Domain.Entities
{
    public class Evaluation : Auditable
    {
        public int Year { get; set; }
        public int Month { get; set; } 

        public string FinalGrade { get; set; }
        public double FinalScore { get; set; }

        public double BaseWorkScore { get; set; }
        public double TeamAttitudeScore { get; set; } 
        public double AttendanceScore { get; set; } 
        public double SkillImprovementScore { get; set; } 
        public int UserId { get; set; }
        public virtual User.User User { get; set; }
    }
}
