using Kpi.Domain.Commons;


namespace Kpi.Domain.Entities.Country
{
    public class Country : Auditable
    {
        public required string Name { get; set; }
    }
}
