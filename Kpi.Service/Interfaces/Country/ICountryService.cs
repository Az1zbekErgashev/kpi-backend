using Kpi.Domain.Models.Country;

namespace Kpi.Service.Interfaces.Country
{
    public interface ICountryService
    {
        ValueTask<List<CountryModel>> GetAsync();
        ValueTask<bool> DeleteAsync(int id);
    }
}
