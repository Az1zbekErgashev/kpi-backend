using Kpi.Domain.Configuration;

namespace Kpi.Service.DTOs.User
{
    public class UserForFilterDTO : PaginationParams
    {
        public string? Text { get; set; }
    }
}
