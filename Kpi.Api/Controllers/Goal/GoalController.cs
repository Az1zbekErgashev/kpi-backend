using Kpi.Domain.Models.Goal;
using Kpi.Domain.Models.Response;
using Kpi.Domain.Models.Team;
using Kpi.Service.DTOs.Goal;
using Kpi.Service.Extencions;
using Kpi.Service.Interfaces.Goal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Kpi.Api.Controllers.Goal
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoalController : ControllerBase
    {
        private readonly IGoalService goalService;

        public GoalController(IGoalService goalService)
        {
            this.goalService = goalService;
        }

        [HttpPost("create-from-ceo")]
        [Authorize(Roles = "Ceo,Director")]
        [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> CreateFromCEOAsync(GoalForCreationDTO @dto) => ResponseHandler.ReturnIActionResponse(await goalService.CreateFromCEOAsync(@dto));

        [HttpPost("create-from-team")]
        [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> CreateFromTeamLeaderAsync(GoalForCreationDTO @dto) => ResponseHandler.ReturnIActionResponse(await goalService.CreateFromTeamLeaderAsync(@dto));

        [HttpPut("update")]
        [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> UpdateAsync(GoalForCreationDTO @dto) => ResponseHandler.ReturnIActionResponse(await goalService.UpdateAsync(@dto));

        [HttpPut("change-goal-status")]
        [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> ChangeGoalStatus(ChangeGoalStatusDTO dto) => ResponseHandler.ReturnIActionResponse(await goalService.ChangeGoalStatus(@dto));

        [HttpPut("send-goal")]
        [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> SendGoalRequest(GoalForSendDTO @dto) => ResponseHandler.ReturnIActionResponse(await goalService.SendGoalRequest(@dto));

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseModel<GoalModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> GetByIdAsync(int id) => ResponseHandler.ReturnIActionResponse(await goalService.GetByIdAsync(id));


        [HttpGet("by-team/{id}/{year}")]
        [Authorize(Roles = "Ceo,Director")]
        [ProducesResponseType(typeof(ResponseModel<GoalModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> GetByUserIdAsync(int id, [Required] int year) => ResponseHandler.ReturnIActionResponse(await goalService.GetByUserIdAsync(id, year));

        [HttpGet("by-user/{id}/{year}")]
        [Authorize(Roles = "TeamLeader,TeamMember")]
        [ProducesResponseType(typeof(ResponseModel<GoalModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> GetByTeamIdAsync(int id, [Required] int year) => ResponseHandler.ReturnIActionResponse(await goalService.GetByTeamIdAsync(id, year));

        [HttpGet("team-leader/{year}")]
        [Authorize(Roles = "TeamMember")]
        [ProducesResponseType(typeof(ResponseModel<GoalModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> GetTeamLeaderGoal([Required] int year) => ResponseHandler.ReturnIActionResponse(await goalService.GetTeamLeaderGoal( year));

        [HttpGet("ceo-goal/{year}")]
        [ProducesResponseType(typeof(ResponseModel<GoalModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> GetByCeoGoal([Required] int year) => ResponseHandler.ReturnIActionResponse(await goalService.GetByCeoGoal(year));


        [HttpGet("by-user-token")]
        [ProducesResponseType(typeof(ResponseModel<GoalModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> GetByTokenIdAsync(int id) => ResponseHandler.ReturnIActionResponse(await goalService.GetByTokenIdAsync(id));

        [HttpGet("team-by-id")]
        [ProducesResponseType(typeof(ResponseModel<TeamAndRoom>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> GetRoomAndTeam(int id) => ResponseHandler.ReturnIActionResponse(await goalService.GetRoomAndTeam(id));
    }
}
