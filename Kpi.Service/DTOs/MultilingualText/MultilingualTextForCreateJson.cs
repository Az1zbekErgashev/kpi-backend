using Kpi.Domain.Enum;
using Microsoft.AspNetCore.Http;

namespace Kpi.Service.DTOs.MultilingualText
{
    public class MultilingualTextForCreateJson
    {
        public IFormFile File { get; set; }
        public SupportLanguage Language { get; set; }
    }
}
