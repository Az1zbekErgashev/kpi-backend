using Kpi.Domain.Configuration;

namespace Kpi.Service.DTOs.Goal
{
    public class MonthlyPerformanceForFilterDTO : PaginationParams
    {
        public int UserId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
