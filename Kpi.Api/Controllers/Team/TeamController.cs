using Kpi.Domain.Models.Response;
using Kpi.Domain.Models.Team;
using Kpi.Service.DTOs.Team;
using Kpi.Service.Extencions;
using Kpi.Service.Interfaces.Team;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Kpi.Api.Controllers.Team
{
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        [HttpGet("filter")]
        [ProducesResponseType(typeof(ResponseModel<TeamModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> GetAllAsync([FromQuery] TeamForFilterDTO @dto) => ResponseHandler.ReturnIActionResponse(await _teamService.GetAllAsync(@dto));

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseModel<TeamModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> GetByIdAsync([Required] int id) => ResponseHandler.ReturnIActionResponse(await _teamService.GetByIdAsync(id));

        [HttpGet("list")]
        [ProducesResponseType(typeof(ResponseModel<List<TeamModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> GetAsync() => ResponseHandler.ReturnIActionResponse(await _teamService.GetAsync());

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> DeleteAsync([Required] int id) => ResponseHandler.ReturnIActionResponse(await _teamService.DeleteAsync(id));

        [HttpPost("create")]
        [ProducesResponseType(typeof(ResponseModel<TeamModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> CreateAsync(TeamForCreateDTO @dto) => ResponseHandler.ReturnIActionResponse(await _teamService.CreateAsync(@dto));

        [HttpPut("update")]
        [ProducesResponseType(typeof(ResponseModel<TeamModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> UpdateAsync(TeamForUpdateDTO @dto) => ResponseHandler.ReturnIActionResponse(await _teamService.UpdateAsync(@dto));
    }
}
