using Kpi.Domain.Enum;
using Kpi.Service.DTOs.MultilingualText;
using Kpi.Service.Extencions;
using Kpi.Service.Interfaces.MultilingualText;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Kpi.Api.Controllers.MultilingualText
{
    [ApiController]
    [Route("api/[controller]")]
    public class MultilingualTextController : ControllerBase
    {
        private readonly IMultilingualTextInterface multilingualTextService;

        public MultilingualTextController(IMultilingualTextInterface multilingualTextService)
        {
            this.multilingualTextService = multilingualTextService;
        }

        [HttpPost("create-from-json")]
        [Authorize(Roles = "Ceo,Director")]
        public async ValueTask<IActionResult> PostDocumentAsync([FromForm] MultilingualTextForCreateJson dto) => ResponseHandler.ReturnIActionResponse(await multilingualTextService.CreateFromJson(dto.File, dto.Language));


        [HttpGet]
        public async ValueTask<IActionResult> GetDictonary([FromQuery] SupportLanguage language) => ResponseHandler.ReturnIActionResponse(await multilingualTextService.GetDictonary(language));


        [HttpGet("all/translations")]
        [Authorize(Roles = "Ceo,Director")]
        public async ValueTask<IActionResult> GetAllAsync([FromQuery] UIContentGetAllAndSearchDTO dto) => ResponseHandler.ReturnIActionResponse(await multilingualTextService.GetTranslations(dto));


        [HttpDelete("delete")]
        [Authorize(Roles = "Ceo,Director")]
        public async ValueTask<IActionResult> DeleteAsync([Required] string key) => ResponseHandler.ReturnIActionResponse(await multilingualTextService.DeleteOrRecoverAsync(key));

        [HttpPost("create")]
        [Authorize(Roles = "Ceo,Director")]
        public async ValueTask<IActionResult> CreateAsync(MultilingualTextForCreateDTO dto) => ResponseHandler.ReturnIActionResponse(await multilingualTextService.CreateAsync(dto));

        [HttpPut("update")]
        [Authorize(Roles = "Ceo,Director")]
        public async ValueTask<IActionResult> UpdateAsync(MultilingualTextForCreateDTO dto) => ResponseHandler.ReturnIActionResponse(await multilingualTextService.UpdateAsync(dto));
    }
}
