using Kpi.Domain.Commons;


namespace Kpi.Domain.Models.Goal
{
    public class KpiGoalModel : Auditable
    {
        public string GoalContent { get; set; }
        public TargetValueModel? TargetValue { get; set; }

        public virtual KpiGoalModel MapFromEntity(Entities.Goal.KpiGoal entity)
        {
            Id = entity.Id;
            CreatedAt = entity.CreatedAt;
            UpdatedAt = entity.UpdatedAt;
            GoalContent = entity.GoalContent;
            TargetValue = entity.TargetValue is null ? null : new TargetValueModel().MapFromEntity(entity.TargetValue);
            return this;
        }
    }
}
