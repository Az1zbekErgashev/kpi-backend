using Kpi.Domain.Commons;

namespace Kpi.Domain.Entities
{
    public class ScoreManagement : Auditable
    {
        public double MinScore { get; set; }
        public double MaxScore { get; set; }
        public string Grade { get; set; }
        public int? DivisionId { get; set; }
        public bool IsFinalScore { get; set; } = false;
        public bool IsMoreDivisions { get; set; } = false;
        public int[]? Divisions { get; set; }
        public Entities.Goal.Division? Division { get; set; }
    }
}
