using Kpi.Domain.Commons;

namespace Kpi.Domain.Models.Goal
{
    public class DivisionModel : Auditable
    {
        public string Name { get; set; }
        public virtual ICollection<KpiGoalModel>? Goals { get; set; }

        public virtual DivisionModel MapFromEntity(Entities.Goal.Division entity)
        {
            Id = entity.Id;
            CreatedAt = entity.CreatedAt;
            UpdatedAt = entity.UpdatedAt;
            Goals = entity.Goals == null || entity?.Goals?.Count == 0 ? null : entity.Goals.Select(x => new KpiGoalModel().MapFromEntity(x)).ToList();
            return this;
        }
    }
}
