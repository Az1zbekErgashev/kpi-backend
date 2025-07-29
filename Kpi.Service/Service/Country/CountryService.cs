using Kpi.Domain.Models.Country;
using Kpi.Service.Exception;
using Kpi.Service.Interfaces.Country;
using Kpi.Service.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Kpi.Service.Service.Country
{
    public class CountryService : ICountryService
    {
        private readonly IGenericRepository<Domain.Entities.Country.Country> countryRepository;

        public CountryService(IGenericRepository<Domain.Entities.Country.Country> countryRepository)
        {
            this.countryRepository = countryRepository;
        }

        public async ValueTask<List<CountryModel>> GetAsync()
        {
            var countrys = await countryRepository.GetAll().ToListAsync();

            var model = countrys.Select(x => new CountryModel().MapFromEntity(x)).ToList();
            return model;
        }

        public async ValueTask<bool> DeleteAsync(int id)
        {
            var country = await countryRepository.GetAll(x => x.Id == id).FirstOrDefaultAsync();

            if (country is null) throw new KpiException(404, "country_not_found");

            country.UpdatedAt = DateTime.UtcNow;

            countryRepository.UpdateAsync(country);
            await countryRepository.SaveChangesAsync();
            return true;
        }
    }
}
