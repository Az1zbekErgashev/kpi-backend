using Kpi.Domain.Commons;

namespace Kpi.Domain.Entities
{
    public class ScoreManagement : Auditable
    {
        public double Score { get; set; }
        public string Grade { get; set; }
        public int DivisionId { get; set; }
        public Entities.Goal.Division Division { get; set; }
    }
}
