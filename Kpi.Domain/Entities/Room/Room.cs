using Kpi.Domain.Commons;

namespace Kpi.Domain.Entities.Room
{
    public class Room : Auditable
    {
        public string Name { get; set; }
        public ICollection<User.User> Users { get; set; }
    }
}
