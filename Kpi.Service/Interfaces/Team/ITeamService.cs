using Kpi.Domain.Models.PagedResult;
using Kpi.Domain.Models.Team;
using Kpi.Service.DTOs.Team;

namespace Kpi.Service.Interfaces.Team
{
    public interface ITeamService
    {
        ValueTask<PagedResult<TeamModel>> GetAllAsync(TeamForFilterDTO @dto);
        ValueTask<bool> DeleteAsync(int id);
        ValueTask<List<TeamModel>> GetAsync();
        ValueTask<TeamModel> GetByIdAsync(int id);
        ValueTask<TeamModel> CreateAsync(TeamForCreateDTO @dto);
        ValueTask<TeamModel> UpdateAsync(TeamForUpdateDTO @dto);
    }
}
