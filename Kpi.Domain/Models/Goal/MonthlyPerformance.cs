using Kpi.Domain.Commons;
using Kpi.Domain.Enum;

namespace Kpi.Domain.Models.Goal
{
    public class MonthlyPerformanceModel : Auditable
    {
        public GoalModel? Goal { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public GoalStatus Status { get; set; }
        public virtual ICollection<MonthlyTargetValueModel> MonthlyTargetValue { get; set; }
        public virtual ICollection<MonthlyCommentModel> MonthlyTargetComment { get; set; }
        public bool IsSended { get; set; }
        public bool? IsTeamLeader { get; set; }

        public virtual MonthlyPerformanceModel MapFromEntity(Entities.Goal.MonthlyPerformance entity, bool? isCurrent = false)
        {
            Id = entity.Id;
            CreatedAt = entity.CreatedAt;
            UpdatedAt = entity.UpdatedAt;
            Year = entity.Year;
            Month = entity.Month;
            Goal = entity.Goal is null ? null : new GoalModel().MapFromEntity(entity.Goal);
            MonthlyTargetComment = entity.MonthlyTargetComment is null ? null : entity.MonthlyTargetComment.Select(x => new MonthlyCommentModel().MapFromEntity(x)).ToList();
            MonthlyTargetValue = entity.MonthlyTargetValue is null ? null : entity.MonthlyTargetValue.Select(x => new MonthlyTargetValueModel().MapFromEntity(x)).ToList();
            IsSended = entity.IsSended;
            IsTeamLeader = isCurrent;
            Status = entity.MonthlyTargetComment is null || entity.MonthlyTargetComment.Count == 0 ? GoalStatus.NoWritte : entity.MonthlyTargetComment.OrderByDescending(x => x.Id).FirstOrDefault().Status;
            return this;
        }
    }

}
