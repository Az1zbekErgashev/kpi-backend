﻿using Kpi.Domain.Commons;

namespace Kpi.Domain.Entities.Team
{
    public class Team : Auditable
    {
        public string Name { get; set; }
        public ICollection<User.User> Users { get; set; }
    }
}
