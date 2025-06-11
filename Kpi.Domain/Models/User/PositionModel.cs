using Kpi.Domain.Commons;
using Kpi.Domain.Entities.User;

namespace Kpi.Domain.Models.User
{
    public class PositionModel : Auditable
    {
        public string Name { get; set; }

        public virtual PositionModel MapFromEntity(Position entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            return this;
        }
    }
}
