using Kpi.Domain.Configuration;

namespace Kpi.Service.DTOs.Team
{
    public class TeamForFilterDTO : PaginationParams
    {
        public string? Name { get; set; }
        public int IsDeleted { get; set; } = 0;
    }
}
