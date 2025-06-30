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

        [HttpGet]
        public async ValueTask<IActionResult> GetByYearAndMonth([FromQuery] MonthlyPerformanceForFilterDTO dto) => ResponseHandler.ReturnIActionResponse(await monthlyTargetService.GetByIdAsync(dto));

        [HttpGet("list")]
        public async ValueTask<IActionResult> GetAllAsync([FromQuery] MonthlyPerformanceForFilterDTO dto) => ResponseHandler.ReturnIActionResponse(await monthlyTargetService.GetAllAsync(dto));
       
        [HttpGet("list-ceo")]
        public async ValueTask<IActionResult> GetUsersForCEO([FromQuery] MonthlyPerformanceForFilterDTO dto) => ResponseHandler.ReturnIActionResponse(await monthlyTargetService.GetUsersForCEO(dto));  

        [HttpGet("team-leader")]
        public async ValueTask<IActionResult> GetTeamLeader([FromQuery] MonthlyPerformanceForFilterDTO dto) => ResponseHandler.ReturnIActionResponse(await monthlyTargetService.GetTeamLeader(dto));
    }
}
