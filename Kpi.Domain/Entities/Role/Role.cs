using Kpi.Domain.Commons;

namespace Kpi.Domain.Entities.Role
{
    public class Role : Auditable
    {
        public string Name { get; set; }
        public ICollection<User.User> Users { get; set; }
    }
}
