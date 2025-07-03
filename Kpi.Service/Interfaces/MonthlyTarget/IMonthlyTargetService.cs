using Kpi.Domain.Models.Goal;
using Kpi.Domain.Models.PagedResult;
using Kpi.Service.DTOs.Goal;
using System.ComponentModel.DataAnnotations;

namespace Kpi.Service.Interfaces.MonthlyTarget
{
    public interface IMonthlyTargetService
    {
        ValueTask<MonthlyPerformanceModel> GetByIdAsync(MonthlyPerformanceForFilterDTO dto);
        ValueTask<bool> CreateAsync(CreateMonthlyTargetGroupDto dto);
        ValueTask<PagedResult<MonthlyPerformanceListModel>> GetAllAsync([Required] MonthlyPerformanceForFilterDTO dto);
        ValueTask<PagedResult<MonthlyPerformanceListModel>> GetUsersForCEO([Required] MonthlyPerformanceForFilterDTO dto);
        ValueTask<PagedResult<MonthlyPerformanceListModel>> GetTeamLeader(MonthlyPerformanceForFilterDTO dto);
        ValueTask<bool> UpdateAsync(CreateMonthlyTargetGroupDto dto);
        ValueTask<bool> ChangeMonthlyTargetStatus(ChangeGoalStatusDTO dto);
    }
}
