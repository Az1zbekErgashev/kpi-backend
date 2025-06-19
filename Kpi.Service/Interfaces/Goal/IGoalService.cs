using Kpi.Domain.Models.Goal;
using Kpi.Domain.Models.Team;
using Kpi.Service.DTOs.Goal;

namespace Kpi.Service.Interfaces.Goal
{
    public interface IGoalService
    {
        ValueTask<bool> CreateAsync(GoalForCreationDTO @dto, int userId);
        ValueTask<bool> CreateFromTeamLeaderAsync(GoalForCreationDTO @dto);
        ValueTask<bool> CreateFromCEOAsync(GoalForCreationDTO @dto, int year);
        ValueTask<GoalModel> UpdateAsync(GoalForCreationDTO @dto);
        ValueTask<bool> ChangeGoalStatus(ChangeGoalStatusDTO dto);
        ValueTask<bool> SendGoalRequest(GoalForSendDTO @dto);
        ValueTask<GoalModel> GetByIdAsync(int id);
        ValueTask<GoalModel> GetByUserIdAsync(int id, int year);
        ValueTask<GoalModel> GetByTokenIdAsync(int year);
        ValueTask<GoalModel> GetByCeoGoal(int year);
        ValueTask<GoalModel> GetByTeamIdAsync(int id, int year);
        ValueTask<GoalModel> GetTeamLeaderGoal(int year);
        ValueTask<TeamAndRoom> GetRoomAndTeam(int teamId);
        ValueTask<TeamAndRoom> GetRoomAndTeamByToken();

    }
}
