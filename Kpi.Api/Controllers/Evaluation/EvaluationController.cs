using Kpi.Domain.Models.Response;
using Kpi.Service.DTOs.Evaluation;
using Kpi.Service.Extencions;
using Kpi.Service.Interfaces.Evaluation;
using Microsoft.AspNetCore.Mvc;

namespace Kpi.Api.Controllers.Evaluation
{
    [Route("api/[controller]")]
    [ApiController]
    public class EvaluationController : ControllerBase
    {
        private readonly IEvaluationService evaluationService;

        public EvaluationController(IEvaluationService evaluationService)
        {
            this.evaluationService = evaluationService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> GetAsync([FromQuery] EvaluationsForFilterDTO dto) => ResponseHandler.ReturnIActionResponse(await evaluationService.GetTeamEvaluationsAsync(dto));

        [HttpPut]
        [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> GetAsync(IEnumerable<EmployeeEvaluationFroCreateDTO> dtos) => ResponseHandler.ReturnIActionResponse(await evaluationService.UpdateAsync(dtos));

        [HttpPost]
        [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> GetAsync(List<EmployeeEvaluationFroCreateDTO> dtos) => ResponseHandler.ReturnIActionResponse(await evaluationService.CreateOrUpdateAsync(dtos));
    }
}
