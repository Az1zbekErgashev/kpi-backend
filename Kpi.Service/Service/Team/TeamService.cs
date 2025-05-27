using Kpi.Domain.Models.PagedResult;
using Kpi.Domain.Models.Team;
using Kpi.Service.DTOs.Team;
using Kpi.Service.Exception;
using Kpi.Service.Interfaces.IRepositories;
using Kpi.Service.Interfaces.Team;
using Kpi.Service.StringExtensions;
using Microsoft.EntityFrameworkCore;

namespace Kpi.Service.Service.Team
{
    public class TeamService : ITeamService
    {
        private readonly IGenericRepository<Domain.Entities.Team.Team> _teamRepository;

        public TeamService(IGenericRepository<Domain.Entities.Team.Team> teamRepository)
        {
            _teamRepository = teamRepository;
        }

        public async ValueTask<PagedResult<TeamModel>> GetAllAsync(TeamForFilterDTO @dto)
        {
            var allTeams = _teamRepository.GetAll(x => x.IsDeleted == 0).Include(x => x.Users).OrderByDescending(x => x.UpdatedAt).AsQueryable();

            if (!string.IsNullOrEmpty(dto.Name)) allTeams = allTeams.Where(x => x.Name.Contains(dto.Name));

            int totalCount = await allTeams.CountAsync();

            if (totalCount == 0)
            {
                return PagedResult<TeamModel>.Create(
                    Enumerable.Empty<TeamModel>(),
                    0,
                    dto.PageSize,
                    0,
                    dto.PageIndex,
                    0
                );
            }

            if (dto.PageIndex == 0)
            {
                dto.PageIndex = 1;
            }

            if (dto.PageSize == 0)
            {
                dto.PageSize = totalCount;
            }

            int itemsPerPage = dto.PageSize;
            int totalPages = (totalCount / itemsPerPage) + (totalCount % itemsPerPage == 0 ? 0 : 1);

            if (dto.PageIndex > totalPages)
            {
                dto.PageIndex = totalPages;
            }

            allTeams = allTeams.ToPagedList(dto);

            var list = await allTeams.ToListAsync();

            List<TeamModel> models = list.Select(
                f => new TeamModel().MapFromEntity(f))
                .ToList();

            var pagedResult = PagedResult<TeamModel>.Create(models,
                totalCount,
                itemsPerPage,
                models.Count,
                dto.PageIndex,
                totalPages
                );

            return pagedResult;
        }

        public async ValueTask<TeamModel> GetByIdAsync(int id)
        {
            var existTeam = await _teamRepository.GetAsync(x => x.Id == id && x.IsDeleted == 0);

            if (existTeam == null) throw new KpiException(404, "team_not_found");

            return new TeamModel().MapFromEntity(existTeam);
        }

        public async ValueTask<List<TeamModel>> GetAsync()
        {
            var allTeams = await _teamRepository.GetAll(x => x.IsDeleted == 0).ToListAsync();

            return allTeams.Select(x => new TeamModel().MapFromEntity(x)).ToList();
        }

        public async ValueTask<bool> DeleteAsync(int id)
        {
            var existTeam = await _teamRepository.GetAsync(x => x.Id == id && x.IsDeleted == 0);

            if (existTeam == null) throw new KpiException(404, "team_not_found");

            existTeam.UpdatedAt = DateTime.UtcNow;
            existTeam.IsDeleted = 1;

            _teamRepository.UpdateAsync(existTeam);
            await _teamRepository.SaveChangesAsync();

            return true;
        }

        public async ValueTask<TeamModel> UpdateAsync(TeamForUpdateDTO @dto)
        {
            var existTeam = await _teamRepository.GetAsync(x => x.Id == dto.Id && x.IsDeleted == 0);

            if (existTeam == null) throw new KpiException(404, "team_not_found");

            existTeam.Name = dto.Name;
            existTeam.UpdatedAt = DateTime.UtcNow;

            _teamRepository.UpdateAsync(existTeam);
            await _teamRepository.SaveChangesAsync();

            return new TeamModel().MapFromEntity(existTeam);
        }

        public async ValueTask<TeamModel> CreateAsync(TeamForCreateDTO @dto)
        {
            var team = new Domain.Entities.Team.Team()
            {
                Name = dto.Name
            };

            await _teamRepository.CreateAsync(team);
            await _teamRepository.SaveChangesAsync();

            return new TeamModel().MapFromEntity(team);
        }
    }
}
