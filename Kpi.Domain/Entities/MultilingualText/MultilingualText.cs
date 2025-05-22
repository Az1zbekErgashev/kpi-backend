using Kpi.Domain.Commons;
using Kpi.Domain.Enum;

namespace Kpi.Domain.Entities.MultilingualText
{
    public class MultilingualText : Auditable
    {
        public string? Key { get; set; }
        public string? Text { get; set; }
        public SupportLanguage SupportLanguage { get; set; }

    }
}
