using Kpi.Domain.Commons;
using Kpi.Domain.Models.User;

namespace Kpi.Domain.Models.Goal
{
    public class MonthlyTargetModel : Auditable
    {
        public string? TargetValueText { get; set; }
        public double? TargetValueNumber { get; set; }
        public double? TargetValueRatio { get; set; }
        public string? TargetEvaluationText { get; set; }
        public UserModel? CreatedBy { get; set; }
        public int? CreatedById { get; set; }
        public int? AssignedToId { get; set; }
        public UserModel? AssignedTo { get; set; }

        public virtual MonthlyTargetModel MapFromEntity(Entities.Goal.MonthlyTarget entity)
        {
            Id = entity.Id;
            CreatedAt = entity.CreatedAt;
            UpdatedAt = entity.UpdatedAt;
            TargetEvaluationText = entity.TargetEvaluationText;
            TargetValueNumber = entity.TargetValueNumber;
            TargetValueRatio = entity.TargetValueRatio;
            TargetValueText = entity.TargetValueText;
            CreatedById = entity.CreatedById;
            AssignedToId = entity.AssignedToId;
            CreatedBy = entity.CreatedBy is null ? null : new UserModel().MapFromEntity(entity.CreatedBy);
            AssignedTo = entity.AssignedTo is null ? null : new UserModel().MapFromEntity(entity.AssignedTo);
            return this;
        }
    }

}
