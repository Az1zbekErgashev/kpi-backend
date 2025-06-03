using Kpi.Domain.Models.Goal;
using Kpi.Service.DTOs.Goal;

namespace Kpi.Service.Interfaces.Goal
{
    public interface IGoalService
    {
        ValueTask<bool> CreateAsync(GoalForCreationDTO @dto, int userId);
        ValueTask<bool> CreateFromTeamLeaderAsync(GoalForCreationDTO @dto);
        ValueTask<bool> CreateFromCEOAsync(GoalForCreationDTO @dto);
        ValueTask<GoalModel> UpdateAsync(GoalForCreationDTO @dto);
        ValueTask<bool> ChangeGoalStatus(ChangeGoalStatusDTO dto);
        ValueTask<bool> SendGoalRequest(GoalForSendDTO @dto);
        ValueTask<GoalModel> GetByIdAsync(int id);
        ValueTask<GoalModel> GetByUserIdAsync(int id);
        ValueTask<GoalModel> GetByTokenIdAsync(int id);
    }
}
