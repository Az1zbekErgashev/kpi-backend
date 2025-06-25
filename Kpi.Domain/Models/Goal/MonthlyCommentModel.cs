using Kpi.Domain.Commons;
using Kpi.Domain.Enum;


namespace Kpi.Domain.Models.Goal
{
    public class MonthlyCommentModel : Auditable
    {
        public string? Content { get; set; }
        public GoalStatus Status { get; set; }

        public virtual MonthlyCommentModel MapFromEntity(Entities.Comment.MonthlyTargetComment entity)
        {
            Id = entity.Id;
            CreatedAt = entity.CreatedAt;
            Content = entity.Content;
            Status = entity.Status;
            return this;
        }
    }
}
