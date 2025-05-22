using Kpi.Domain.Commons;

namespace Kpi.Domain.Entities.Attachment;
public class Attachment : Auditable
{
    public required string Path { get; set; }
}
