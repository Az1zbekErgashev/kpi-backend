using Kpi.Service.DTOs.Goal;
using Kpi.Service.Extencions;
using Kpi.Service.Interfaces.MonthlyTarget;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kpi.Api.Controllers.MonthlyTarget
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MonthlyTarget : ControllerBase
    {
        private readonly IMonthlyTargetService monthlyTargetService;

        public MonthlyTarget(IMonthlyTargetService monthlyTargetService)
        {
            this.monthlyTargetService = monthlyTargetService;
        }

        [HttpPost]
        public async ValueTask<IActionResult> CreateAsync(CreateMonthlyTargetGroupDto dto) => ResponseHandler.ReturnIActionResponse(await monthlyTargetService.CreateAsync(dto));

        [HttpPut]
        public async ValueTask<IActionResult> UpdateAsync(CreateMonthlyTargetGroupDto dto) => ResponseHandler.ReturnIActionResponse(await monthlyTargetService.UpdateAsync(dto));

        [HttpPut("change-status")]
        public async ValueTask<IActionResult> ChangeStatus(ChangeGoalStatusDTO dto) => ResponseHandler.ReturnIActionResponse(await monthlyTargetService.ChangeMonthlyTargetStatus(dto));

        [HttpGet]
        public async ValueTask<IActionResult> GetByYearAndMonth([FromQuery] MonthlyPerformanceForFilterDTO dto) => ResponseHandler.ReturnIActionResponse(await monthlyTargetService.GetByIdAsync(dto));

        [HttpGet("list")]
        [Authorize(Roles = "TeamMember,TeamLeader")]
        public async ValueTask<IActionResult> GetAllAsync([FromQuery] MonthlyPerformanceForFilterDTO dto) => ResponseHandler.ReturnIActionResponse(await monthlyTargetService.GetAllAsync(dto));

        [HttpGet("list-ceo")]
        public async ValueTask<IActionResult> GetUsersForCEO([FromQuery] MonthlyPerformanceForFilterDTO dto) => ResponseHandler.ReturnIActionResponse(await monthlyTargetService.GetUsersForCEO(dto));

        [HttpGet("get-team-leader-target")]
        public async ValueTask<IActionResult> GetByIdForCeoAsync([FromQuery] MonthlyPerformanceForFilterDTO dto) => ResponseHandler.ReturnIActionResponse(await monthlyTargetService.GetByIdForCeoAsync(dto));

        [HttpGet("team-leader")]
        [Authorize(Roles = "TeamLeader")]
        public async ValueTask<IActionResult> GetTeamLeader([FromQuery] MonthlyPerformanceForFilterDTO dto) => ResponseHandler.ReturnIActionResponse(await monthlyTargetService.GetTeamLeader(dto));
        
        [HttpGet("leader")]
        [Authorize(Roles = "TeamLeader")]
        public async ValueTask<IActionResult> GetTeamLeaderTarget([FromQuery] MonthlyPerformanceForFilterDTO dto) => ResponseHandler.ReturnIActionResponse(await monthlyTargetService.GetTeamLeaderTarget(dto));        
       
        [HttpGet("member")]
        [Authorize(Roles = "TeamMember")]
        public async ValueTask<IActionResult> GetMemberTarget([FromQuery] MonthlyPerformanceForFilterDTO dto) => ResponseHandler.ReturnIActionResponse(await monthlyTargetService.GetMemberTarget(dto));
    }
}
