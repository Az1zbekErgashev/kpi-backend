using Kpi.Domain.Configuration;

namespace Kpi.Service.DTOs.User
{
    public class UserForFilterCEOSideDTO : PaginationParams
    {
        public DateTime? Year { get; set; }
        public int? RoomId { get; set; }
        public int? TeamId { get; set; }
        public string? UserName { get; set; }
    }
}
