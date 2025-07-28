using Kpi.Domain.Models.Goal;
using Kpi.Domain.Models.Response;
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

        [HttpPost("create-from-ceo/{year}")]
        [Authorize(Roles = "Ceo,Director")]
        [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> CreateFromCEOAsync(GoalForCreationDTO @dto, int year) => ResponseHandler.ReturnIActionResponse(await goalService.CreateFromCEOAsync(@dto, year));

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


        [HttpGet("by-user-id/{id}/{year}")]
        [Authorize(Roles = "Ceo,Director")]
        [ProducesResponseType(typeof(ResponseModel<GoalModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> GetByUserIdAsync(int id, [Required] int year) => ResponseHandler.ReturnIActionResponse(await goalService.GetByUserIdAsync(id, year));

        [HttpGet("user-side/{year}")]
        [Authorize(Roles = "TeamMember")]
        public async ValueTask<IActionResult> GetByTeamIdAsync([Required] int year) => ResponseHandler.ReturnIActionResponse(await goalService.GetTeamMemberGoal(year));

        [HttpGet("team-leader/{year}")]
        [Authorize(Roles = "TeamMember")]
        public async ValueTask<IActionResult> GetTeamLeaderGoal([Required] int year) => ResponseHandler.ReturnIActionResponse(await goalService.GetTeamLeaderGoal(year));

        [HttpGet("ceo-goal/{year}")]
        public async ValueTask<IActionResult> GetByCeoGoal([Required] int year) => ResponseHandler.ReturnIActionResponse(await goalService.GetByCeoGoal(year));

        [HttpGet("by-user-token")]
        public async ValueTask<IActionResult> GetByTokenIdAsync(int year) => ResponseHandler.ReturnIActionResponse(await goalService.GetByTokenIdAsync(year));

        [HttpGet("team-by-id")]
        public async ValueTask<IActionResult> GetRoomAndTeam(int id) => ResponseHandler.ReturnIActionResponse(await goalService.GetRoomAndTeam(id));

        [HttpGet("team-by-token")]
        public async ValueTask<IActionResult> GetRoomAndTeamByToken() => ResponseHandler.ReturnIActionResponse(await goalService.GetRoomAndTeamByToken());


        [HttpDelete("delete-all-goal")]
        public async ValueTask<IActionResult> DeleteAllData() => ResponseHandler.ReturnIActionResponse(await goalService.DeleteAllData());
        

        [HttpGet("team-leader-side")]
        [Authorize(Roles = "TeamLeader")]
        public async ValueTask<IActionResult> GetTeamLeaderGoalAsync([Required] int year) => ResponseHandler.ReturnIActionResponse(await goalService.GetTeamLeaderSideGoal(year));
    }
}
