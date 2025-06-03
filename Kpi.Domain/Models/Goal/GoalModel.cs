using Kpi.Domain.Commons;
using Kpi.Domain.Enum;
using Kpi.Domain.Models.Comment;
using Kpi.Domain.Models.User;

namespace Kpi.Domain.Models.Goal
{
    public class GoalModel : Auditable
    {
        public ICollection<DivisionModel>? Divisions { get; set; }
        public virtual ICollection<CommentModel>? Comments { get; set; }
        public int? AssignedToId { get; set; }
        public UserModel? AssignedTo { get; set; }
        public int CreatedById { get; set; }
        public UserModel? CreatedBy { get; set; }
        public GoalStatus Status { get; set; }

        public virtual GoalModel MapFromEntity(Entities.Goal.Goal entity)
        {
            Id = entity.Id;
            CreatedAt = entity.CreatedAt;
            UpdatedAt = entity.UpdatedAt;
            Status = entity.Status;
            CreatedById = entity.CreatedById;
            AssignedToId = entity.AssignedToId;
            CreatedBy = entity.CreatedBy == null ? null : new UserModel().MapFromEntity(entity.CreatedBy);
            AssignedTo = entity.AssignedTo == null ? null : new UserModel().MapFromEntity(entity.AssignedTo);
            Divisions = entity.Divisions == null || entity?.Divisions?.Count == 0 ? null : entity.Divisions.Select(x => new DivisionModel().MapFromEntity(x)).ToList();
            Comments = entity.Comments == null || entity?.Comments?.Count == 0 ? null : entity.Comments.Select(x => new CommentModel().MapFromEntity(x)).ToList();
            return this;
        }
    }
}
