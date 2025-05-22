
namespace Kpi.Domain.Commons;
public class Auditable
{
    public int Id { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    public int IsDeleted { get; set; } = 0;
}
