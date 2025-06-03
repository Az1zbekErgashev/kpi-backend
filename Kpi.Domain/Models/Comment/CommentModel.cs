using Kpi.Domain.Commons;
using Kpi.Domain.Enum;

namespace Kpi.Domain.Models.Comment
{
    public class CommentModel : Auditable
    {
        public string? Content { get; set; }
        public GoalStatus Status { get; set; }

        public virtual CommentModel MapFromEntity(Entities.Comment.Comment entity)
        {
            Id = entity.Id;
            CreatedAt = entity.CreatedAt;
            Content = entity.Content;
            Status = entity.Status;
            return this;
        }
    }
}
