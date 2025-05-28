

using Kpi.Domain.Configuration;

namespace Kpi.Service.DTOs.Room
{
    public class RoomForFilterDTO : PaginationParams
    {
        public string? Name { get; set; }
        public int IsDeleted { get; set; } = 0;
    }
}
