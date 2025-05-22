using Kpi.Domain.Configuration;

namespace Kpi.Service.DTOs.MultilingualText
{
    public class UIContentGetAllAndSearchDTO : PaginationParams
    {
        public string? Key { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
