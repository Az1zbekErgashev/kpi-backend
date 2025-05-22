using Kpi.Service.Extencions;
using Kpi.Service.Interfaces.Country;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Kpi.Api.Controllers.Country
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ICountryService _countryService;

        public CountryController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        [HttpGet]
        public async ValueTask<IActionResult> GetAllCountry() => ResponseHandler.ReturnIActionResponse(await _countryService.GetAsync());


        [HttpDelete]
        public async ValueTask<IActionResult> DeleteAsync([Required] int id) => ResponseHandler.ReturnIActionResponse(await _countryService.DeleteAsync(id));
    }
}
