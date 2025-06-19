using DocumentFormat.OpenXml.InkML;
using Kpi.Domain.Enum;
using Kpi.Domain.Models.PagedResult;
using Kpi.Domain.Models.User;
using Kpi.Infrastructure.Contexts;
using Kpi.Service.DTOs.User;
using Kpi.Service.Exception;
using Kpi.Service.Interfaces.IRepositories;
using Kpi.Service.Interfaces.User;
using Kpi.Service.StringExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using System.Security.Claims;

namespace Kpi.Service.Service.User
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<Domain.Entities.User.User> _userRepository;
        private readonly IGenericRepository<Domain.Entities.User.Position> _positionRepository;
        private readonly IGenericRepository<Domain.Entities.Team.Team> _teamRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly KpiDB kpiDB;
        public UserService(IGenericRepository<Domain.Entities.User.User> userRepository, IHttpContextAccessor httpContextAccessor, IGenericRepository<Domain.Entities.User.Position> positionRepository, KpiDB kpiDB, IGenericRepository<Domain.Entities.Team.Team> teamRepository)
        {
            _userRepository = userRepository;
            this.httpContextAccessor = httpContextAccessor;
            _positionRepository = positionRepository;
            this.kpiDB = kpiDB;
            _teamRepository = teamRepository;
        }
        public async ValueTask<UserModel> CreateAsync(UserForCreateDTO @dto)
        {
            var existUser = await _userRepository.GetAsync(x => x.UserName == dto.UserName && x.IsDeleted == 0);

            if (existUser != null) throw new KpiException(400, "this_user_already_exist");

            var isExistTeam = await _teamRepository.GetAll(
                x => x.Id == dto.TeamId &&
                     x.IsDeleted == 0 &&
                     x.Users.Any(u => u.Role == Role.TeamLeader && u.IsDeleted == 0)
            ).FirstOrDefaultAsync();

            if (isExistTeam != null)
            {
                throw new KpiException(400, "team_already_has_team_leader");
            }

            var user = new Domain.Entities.User.User()
            {
                TeamId = dto.TeamId,
                FullName = dto.FullName,
                Role = dto.Role,
                UserName = dto.UserName,
                Password = dto.Password.Encrypt(),
                RoomId = dto.RoomId,
                PositionId = dto.PositionId
            };

            await _userRepository.CreateAsync(user);
            await _userRepository.SaveChangesAsync();
            return new UserModel().MapFromEntity(user);
        }

        public async ValueTask<UserModel> UpdateAsync(UserForUpdateDTO @dto)
        {
            var existUser = await _userRepository.GetAsync(x => x.Id == dto.Id && x.IsDeleted == 0);

            var existuserName = await _userRepository.GetAsync(x => x.UserName == dto.UserName && x.Id != dto.Id);

            if (existUser == null) throw new KpiException(404, "user_not_found");

            if (existuserName != null) throw new KpiException(400, "this_user_already_exist");

            var isExistTeam = await _teamRepository.GetAll(
                x => x.Id == dto.TeamId &&
                     x.IsDeleted == 0 &&
                     x.Users.Any(u => u.Role == Role.TeamLeader && u.IsDeleted == 0)
            ).FirstOrDefaultAsync();

            if (isExistTeam != null)
            {
                throw new KpiException(400, "team_already_has_team_leader");
            }

            existUser.Role = dto.Role;
            existUser.FullName = dto.FullName;
            existUser.UserName = dto.UserName;
            existUser.Password = dto.Password ?? existUser.Password;
            existUser.TeamId = dto.TeamId;
            existUser.UpdatedAt = DateTime.UtcNow;
            existUser.RoomId = dto.RoomId;
            existUser.PositionId = dto.PositionId;

            _userRepository.UpdateAsync(existUser);
            await _userRepository.SaveChangesAsync();
            return new UserModel().MapFromEntity(existUser);
        }

        public async ValueTask<bool> DeleteAsync([Required] int id)
        {
            var existUser = await _userRepository.GetAsync(x => x.Id == id && x.IsDeleted == 0);

            if (existUser == null) throw new KpiException(404, "user_not_found");

            existUser.UpdatedAt = DateTime.UtcNow;
            existUser.IsDeleted = 1;

            _userRepository.UpdateAsync(existUser);
            await _userRepository.SaveChangesAsync();
            return true;
        }

        public async ValueTask<PagedResult<UserModel>> GetAllAsync(UserForFilterDTO @dto)
        {
            var query = _userRepository.GetAll(x => x.IsDeleted == dto.IsDeleted && x.Id != 1)
                .Include(x => x.AssignedGoals)
                .Include(x => x.CreatedGoals)
                .Include(x => x.Team)
                .Include(x => x.Evaluations)
                .Include(x => x.Room)
                .Include(x => x.Position)
                .OrderByDescending(x => x.UpdatedAt)
                .AsQueryable();

            if (!string.IsNullOrEmpty(dto.Text))
            {
                query = query.Where(x => x.UserName.Contains(dto.Text) || x.FullName.Contains(dto.Text) || (x.Team != null && x.Team.Name.Contains(dto.Text)));
            }

            int totalCount = await query.CountAsync();

            if (totalCount == 0)
            {
                return PagedResult<UserModel>.Create(
                    Enumerable.Empty<UserModel>(),
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

            query = query.ToPagedList(dto);

            var list = await query.ToListAsync();

            List<UserModel> models = list.Select(
                f => new UserModel().MapFromEntity(f))
                .ToList();

            var pagedResult = PagedResult<UserModel>.Create(models,
                totalCount,
                itemsPerPage,
                models.Count,
                dto.PageIndex,
                totalPages
                );

            return pagedResult;
        }

        public async ValueTask<UserModel> GetByIdAsync([Required] int id)
        {
            var existUser = await _userRepository.GetAll(x => x.Id == id && x.IsDeleted == 0)
                .Include(x => x.AssignedGoals)
                .Include(x => x.CreatedGoals)
                .Include(x => x.Team)
                .Include(x => x.Evaluations)
                .Include(x => x.Room)
                .Include(x => x.Position)
                .FirstOrDefaultAsync();

            if (existUser == null) throw new KpiException(404, "user_not_found");

            return new UserModel().MapFromEntity(existUser);
        }

        public async ValueTask<UserModel> GetByTokenAsync()
        {
            if (!int.TryParse(httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            {
                throw new InvalidCredentialException();
            }

            var existUser = await _userRepository.GetAll(x => x.Id == userId && x.IsDeleted == 0)
                .Include(x => x.AssignedGoals)
                .Include(x => x.CreatedGoals)
                .Include(x => x.Team)
                .Include(x => x.Evaluations)
                .Include(x => x.Room)
                .Include(x => x.Position)
                .FirstOrDefaultAsync();

            if (existUser == null) throw new KpiException(404, "user_not_found");

            return new UserModel().MapFromEntity(existUser);
        }

        public async ValueTask<PagedResult<UserModelForCEO>> GetUsersForCEO(UserForFilterCEOSideDTO dto)
        {
            var query = _userRepository.GetAll(x => x.Id != 1 && x.Role == Domain.Enum.Role.TeamLeader && x.IsDeleted == 0 && x.Team.IsDeleted != 0 && x.Room.IsDeleted != 0)
                .Include(x => x.CreatedGoals)
                .Include(x => x.Team)
                .Include(x => x.Evaluations)
                .Include(x => x.Room)
                .Include(x => x.Position)
                .OrderByDescending(x => x.UpdatedAt)
                .AsQueryable();

            if (!string.IsNullOrEmpty(dto.UserName))
            {
                query = query.Where(x => x.UserName.Contains(dto.UserName));
            }

            if (dto.RoomId != null)
            {
                query = query.Where(x => x.RoomId == dto.RoomId);
            }

            if (dto.TeamId != null)
            {
                query = query.Where(x => x.TeamId == dto.TeamId);
            }

            int totalCount = await query.CountAsync();

            if (totalCount == 0)
            {
                return PagedResult<UserModelForCEO>.Create(
                    Enumerable.Empty<UserModelForCEO>(),
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

            query = query.ToPagedList(dto);

            var list = await query.ToListAsync();

            string filterYear = dto?.Year?.Year.ToString() ?? DateTime.UtcNow.Year.ToString();

            List<UserModelForCEO> models = list.Select(
                f => new UserModelForCEO().MapFromEntity(f, filterYear))
                .ToList();

            var pagedResult = PagedResult<UserModelForCEO>.Create(models,
                totalCount,
                itemsPerPage,
                models.Count,
                dto.PageIndex,
                totalPages
                );

            return pagedResult;
        }

        public async ValueTask<List<PositionModel>> GetPositionAsync()
        {
            var position = await _positionRepository.GetAll().ToListAsync();
            return position.Select(x => new PositionModel().MapFromEntity(x)).ToList();
        }

        public async ValueTask<PagedResult<UserModelForCEO>> GetUserListWithGoal(UserForFilterCEOSideDTO dto)
        {
            var user = httpContextAccessor?.HttpContext?.User
            ?? throw new InvalidCredentialException();

            if (!int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ||
                !int.TryParse(user.FindFirstValue(ClaimTypes.Country), out var teamId) ||
                !Enum.TryParse<Role>(user.FindFirstValue(ClaimTypes.Role), ignoreCase: true, out var role))
            {
                throw new InvalidCredentialException("Invalid token claims.");
            }

            var users = _userRepository.GetAll(x => x.IsDeleted == 0 && x.Team.IsDeleted != 0 && x.Room.IsDeleted != 0)
                .Include(x => x.CreatedGoals)
                .Include(x => x.Team)
                .Include(x => x.Evaluations)
                .Include(x => x.Room)
                .Include(x => x.Position)
                .OrderByDescending(x => x.UpdatedAt)
                .AsQueryable();

            if (role == Role.TeamLeader)
            {
                users = users.Where(x => x.TeamId == teamId && x.Id != userId);
            }
            else
            {
                users = users.Where(x => teamId == x.TeamId && x.Id == userId);
            }

            int totalCount = await users.CountAsync();

            if (totalCount == 0)
            {
                return PagedResult<UserModelForCEO>.Create(
                    Enumerable.Empty<UserModelForCEO>(),
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

            users = users.ToPagedList(dto);

            var list = await users.ToListAsync();

            string filterYear = dto?.Year?.Year.ToString() ?? DateTime.UtcNow.Year.ToString();

            List<UserModelForCEO> models = list.Select(
                f => new UserModelForCEO().MapFromEntity(f, filterYear))
                .ToList();

            var pagedResult = PagedResult<UserModelForCEO>.Create(models,
                totalCount,
                itemsPerPage,
                models.Count,
                dto.PageIndex,
                totalPages
                );

            return pagedResult;
        }

        public async ValueTask<PagedResult<UserModelForCEO>> GetTeamLeader(UserForFilterCEOSideDTO dto)
        {
            var user = httpContextAccessor?.HttpContext?.User
           ?? throw new InvalidCredentialException();

            if (!int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ||
                !int.TryParse(user.FindFirstValue(ClaimTypes.Country), out var teamId) ||
                !Enum.TryParse<Role>(user.FindFirstValue(ClaimTypes.Role), ignoreCase: true, out var role))
            {
                throw new InvalidCredentialException("Invalid token claims.");
            }

            if (role != Role.TeamLeader) throw new KpiException(400, "inccorect_role");

            var users = _userRepository.GetAll(x => x.IsDeleted == 0 && x.Id == userId && x.TeamId == teamId && x.Team.IsDeleted != 0 && x.Room.IsDeleted != 0)
                .Include(x => x.CreatedGoals)
                .Include(x => x.Team)
                .Include(x => x.Evaluations)
                .Include(x => x.Room)
                .Include(x => x.Position)
                .OrderByDescending(x => x.UpdatedAt)
                .AsQueryable();


            int totalCount = await users.CountAsync();

            if (totalCount == 0)
            {
                return PagedResult<UserModelForCEO>.Create(
                    Enumerable.Empty<UserModelForCEO>(),
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

            users = users.ToPagedList(dto);

            var list = await users.ToListAsync();

            string filterYear = dto?.Year?.Year.ToString() ?? DateTime.UtcNow.Year.ToString();

            List<UserModelForCEO> models = list.Select(
                f => new UserModelForCEO().MapFromEntity(f, filterYear))
                .ToList();

            var pagedResult = PagedResult<UserModelForCEO>.Create(models,
                totalCount,
                itemsPerPage,
                models.Count,
                dto.PageIndex,
                totalPages
                );

            return pagedResult;
        }
    }
}
