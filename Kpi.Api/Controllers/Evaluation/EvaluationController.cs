using Kpi.Domain.Models.Response;
using Kpi.Service.DTOs.Evaluation;
using Kpi.Service.Extencions;
using Kpi.Service.Interfaces.Evaluation;
using Microsoft.AspNetCore.Authorization;
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
        public async ValueTask<IActionResult> GetAsync([FromQuery] EvaluationsForFilterDTO dto) => ResponseHandler.ReturnIActionResponse(await evaluationService.GetTeamEvaluationsAsync(dto));

        [HttpGet("member")]
        [Authorize(Roles = "TeamLeader")]
        public async ValueTask<IActionResult> GetUserEvaluationsAsync([FromQuery] EvaluationsForFilterDTO dto) => ResponseHandler.ReturnIActionResponse(await evaluationService.GetUserEvaluationsAsync(dto));

        [HttpPost]
        [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> GetAsync(List<EmployeeEvaluationFroCreateDTO> dtos) => ResponseHandler.ReturnIActionResponse(await evaluationService.CreateOrUpdateAsync(dtos));


        [HttpGet("all-evaluation")]
        [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> GetEvaluation(int year) => ResponseHandler.ReturnIActionResponse(await evaluationService.GetAllEvaluation(year));

        [HttpGet("all-evaluation-by-team")]
        [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> GetAllEvaluationByTeam(int year, int team) => ResponseHandler.ReturnIActionResponse(await evaluationService.GetAllEvaluationByTeam(year, team));

        [HttpGet("all-evaluation-by-year")]
        [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> GetAllEvaluationByYear(int year) => ResponseHandler.ReturnIActionResponse(await evaluationService.GetAllEvaluationByYear(year));


        [HttpGet("all-score")]
        public async Task<IActionResult> GetConfigForYear(int year) => ResponseHandler.ReturnIActionResponse(await evaluationService.GetEvaluationScores(year));

        [HttpGet("divisions-name")]
        public async Task<IActionResult> GetDivisionName(int year) => ResponseHandler.ReturnIActionResponse(await evaluationService.GetDivisionName(year));

        [HttpPost("score")]
        public async Task<IActionResult> GetDivisionName(ScoreForCreationDTO dto) => ResponseHandler.ReturnIActionResponse(await evaluationService.CreateScore(dto));

        [HttpPut("score")]
        public async Task<IActionResult> UpdateScore(ScoreForUpdateDTO dto) => ResponseHandler.ReturnIActionResponse(await evaluationService.UpdateScore(dto));

        [HttpDelete("score/{id}")]
        public async Task<IActionResult> DeleteScore(int id) => ResponseHandler.ReturnIActionResponse(await evaluationService.DeleteScore(id));
    }
}
