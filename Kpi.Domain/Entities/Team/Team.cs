using Kpi.Domain.Commons;

namespace Kpi.Domain.Entities.Team
{
    public class Team : Auditable
    {
        public string Name { get; set; }
        public int DirectorId { get; set; }
        public User.User Director { get; set; }
        public ICollection<User.User> Users { get; set; }
    }
}
