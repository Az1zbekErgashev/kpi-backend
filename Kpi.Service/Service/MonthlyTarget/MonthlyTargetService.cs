using Kpi.Domain.Enum;
using Kpi.Domain.Models.Goal;
using Kpi.Domain.Models.PagedResult;
using Kpi.Service.DTOs.Goal;
using Kpi.Service.Exception;
using Kpi.Service.Interfaces.IRepositories;
using Kpi.Service.Interfaces.MonthlyTarget;
using Kpi.Service.StringExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using System.Security.Claims;

namespace Kpi.Service.Service.MonthlyTarget
{
    public class MonthlyTargetService : IMonthlyTargetService
    {
        private readonly IGenericRepository<Domain.Entities.Goal.MonthlyPerformance> monthlyPerformanceRepository;
        private readonly IGenericRepository<Domain.Entities.Goal.MonthlyTargetValue> monthlyTargetValueRepository;
        private readonly IGenericRepository<Domain.Entities.Goal.Goal> goalRepository;
        private readonly IGenericRepository<Domain.Entities.User.User> userRepository;
        private readonly IGenericRepository<Domain.Entities.Goal.TargetValue> targetValueRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IGenericRepository<Domain.Entities.Team.Team> teamRepository;
        private readonly IGenericRepository<Domain.Entities.Comment.MonthlyTargetComment> monthlyTargetCommentRepository;
        public MonthlyTargetService(IGenericRepository<
            Domain.Entities.Goal.MonthlyPerformance> monthlyPerformanceRepository,
            IGenericRepository<Domain.Entities.Goal.Goal> goalRepository,
            IHttpContextAccessor httpContextAccessor,
            IGenericRepository<Domain.Entities.Goal.MonthlyTargetValue> monthlyTargetValueRepository,
            IGenericRepository<Domain.Entities.Comment.MonthlyTargetComment> monthlyTargetCommentRepository,
            IGenericRepository<Domain.Entities.Goal.TargetValue> targetValueRepository,
            IGenericRepository<Domain.Entities.User.User> userRepository,
            IGenericRepository<Domain.Entities.Team.Team> teamRepository)
        {
            this.monthlyPerformanceRepository = monthlyPerformanceRepository;
            this.goalRepository = goalRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.monthlyTargetValueRepository = monthlyTargetValueRepository;
            this.monthlyTargetCommentRepository = monthlyTargetCommentRepository;
            this.targetValueRepository = targetValueRepository;
            this.userRepository = userRepository;
            this.teamRepository = teamRepository;
        }

        public async ValueTask<bool> CreateAsync(CreateMonthlyTargetGroupDto dto)
        {
            var existGoal = await goalRepository.GetAll(x => x.Id == dto.GoalId && x.CreatedAt.Year == dto.Year).Include(x => x.Divisions).ThenInclude(x => x.Goals)
               .ThenInclude(x => x.TargetValue).FirstOrDefaultAsync();

            if (existGoal is null) throw new KpiException(404, "goal_not_found");

            var existMonthlyEvalutions = await monthlyPerformanceRepository.GetAsync(x => x.Year == dto.Year && x.Month == dto.Month && x.GoalId == existGoal.Id);

            if (existMonthlyEvalutions is null) throw new KpiException(404, "monthly_not_found");

            foreach (var targetDto in dto.Targets)
            {
                var existTargetValue = await targetValueRepository.GetAsync(x => x.Id == targetDto.TargetValueId);

                if (existTargetValue == null) throw new KpiException(404, "kpi_goal_not_found");

                if (existTargetValue.Type != Domain.Enum.TargetValueType.IndividualEvaluation || existTargetValue.Type != Domain.Enum.TargetValueType.LeaderEvaluation)
                {
                    var alreadyExists = await monthlyTargetValueRepository
                                                     .GetAsync(x => x.TargetValueId == targetDto.TargetValueId && x.MonthlyPerformanceId == existMonthlyEvalutions.Id);

                    if(alreadyExists == null)
                    {
                        var monthlyTarget = new Domain.Entities.Goal.MonthlyTargetValue
                        {
                            ValueRatio = targetDto.ValueRatio,
                            ValueRatioStatus = targetDto.ValueRatioStatus,
                            ValueNumber = targetDto.ValueNumber,
                            ValueText = targetDto.ValueText,
                            TargetValueId = targetDto.TargetValueId,
                            MonthlyPerformanceId = existMonthlyEvalutions.Id
                        };

                       await monthlyTargetValueRepository.CreateAsync(monthlyTarget);
                    }
                }
            }

            var comment = new Domain.Entities.Comment.MonthlyTargetComment
            {
                Content = dto.Comment,
                Status = Domain.Enum.GoalStatus.PendingReview,
                CreatedById = GetUserIdFromContext(),
                MonthlyPerformanceId = existMonthlyEvalutions.Id
            };

            await monthlyTargetCommentRepository.CreateAsync(comment);

            existMonthlyEvalutions.IsSended = true;
            monthlyPerformanceRepository.UpdateAsync(existMonthlyEvalutions);

            await monthlyPerformanceRepository.SaveChangesAsync();
            await monthlyTargetValueRepository.SaveChangesAsync();
            await monthlyTargetCommentRepository.SaveChangesAsync();

            return true;
        }

        public async ValueTask<bool> UpdateAsync(CreateMonthlyTargetGroupDto dto)
        {
            var existGoal = await goalRepository.GetAll(x => x.Id == dto.GoalId && x.CreatedAt.Year == dto.Year).Include(x => x.Divisions).ThenInclude(x => x.Goals)
              .ThenInclude(x => x.TargetValue).FirstOrDefaultAsync();

            if (existGoal is null) throw new KpiException(404, "goal_not_found");

            var existMonthlyEvalutions = await monthlyPerformanceRepository.GetAsync(x => x.Year == dto.Year && x.Month == dto.Month && x.GoalId == existGoal.Id);

            if (existMonthlyEvalutions is null) throw new KpiException(404, "monthly_not_found");

            foreach (var targetDto in dto.Targets)
            {
                var alreadyExists = await monthlyTargetValueRepository
                                                    .GetAsync(x => x.TargetValueId == targetDto.TargetValueId && x.MonthlyPerformanceId == existMonthlyEvalutions.Id && x.Id == targetDto.Id);

                if (alreadyExists == null)
                {
                    throw new KpiException(404, "goal_not_found");
                }

                alreadyExists.ValueText = targetDto.ValueText;
                alreadyExists.ValueRatio = targetDto.ValueRatio;
                alreadyExists.ValueNumber = targetDto.ValueNumber;
                alreadyExists.ValueRatioStatus = targetDto.ValueRatioStatus;
                alreadyExists.ValueText = targetDto.ValueText;

                monthlyTargetValueRepository.UpdateAsync(alreadyExists);
            }

            var comment = new Domain.Entities.Comment.MonthlyTargetComment
            {
                Content = dto.Comment,
                Status = Domain.Enum.GoalStatus.PendingReview,
                CreatedById = GetUserIdFromContext(),
                MonthlyPerformanceId = existMonthlyEvalutions.Id
            };

            existMonthlyEvalutions.Status = GoalStatus.PendingReview;

            monthlyPerformanceRepository.UpdateAsync(existMonthlyEvalutions);
            await monthlyPerformanceRepository.SaveChangesAsync();

            await monthlyTargetCommentRepository.CreateAsync(comment);
            await monthlyTargetValueRepository.SaveChangesAsync();
            await monthlyTargetCommentRepository.SaveChangesAsync();

            return true;
        }

        private int GetUserIdFromContext()
        {
            if (!int.TryParse(httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                throw new InvalidCredentialException();
            return userId;
        }

        public async ValueTask<MonthlyPerformanceModel> GetByIdAsync(MonthlyPerformanceForFilterDTO dto)
        {
            var user = httpContextAccessor?.HttpContext?.User
               ?? throw new InvalidCredentialException();

            if (!int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ||
                !int.TryParse(user.FindFirstValue(ClaimTypes.Country), out var teamId) ||
                !Enum.TryParse<Role>(user.FindFirstValue(ClaimTypes.Role), ignoreCase: true, out var role))
            {
                throw new InvalidCredentialException("Invalid token claims.");
            }

            var existUserThisTeam = await teamRepository.GetAsync(
              x => x.Id == teamId && x.Users.Any(o => o.Id == dto.UserId)
              );

            if (existUserThisTeam == null) throw new KpiException(404, "user_not_found");


            var model = await monthlyPerformanceRepository.GetAll(x => x.Goal.CreatedById == dto.UserId && x.IsDeleted == 0 && x.Year == dto.Year && x.Month == dto.Month && x.Goal.Status == GoalStatus.Approved)
                .Include(x => x.MonthlyTargetComment)
                .Include(x => x.MonthlyTargetValue)
                .Include(x => x.Goal)
                .ThenInclude(x => x.CreatedBy)
                .ThenInclude(x => x.Team)
                .Include(x => x.Goal)
                .ThenInclude(x => x.CreatedBy)
                .ThenInclude(x => x.Room)
                .Include(x => x.Goal)
                .ThenInclude(x => x.Divisions)
                .ThenInclude(x => x.Goals)
                .ThenInclude(x => x.TargetValue)
                .Include(x => x.Goal)
                .ThenInclude(x => x.Comments)
                .FirstOrDefaultAsync();

            if (model == null || model.Goal == null) throw new KpiException(404, "goal_not_found");

            bool isTeamLeader = dto.UserId != GetUserIdFromContext() && role == Role.TeamLeader ? true : false;

            return new MonthlyPerformanceModel().MapFromEntity(model, isTeamLeader);
        }

        public async ValueTask<PagedResult<MonthlyPerformanceListModel>> GetAllAsync([Required] MonthlyPerformanceForFilterDTO dto)
        {
            var user = httpContextAccessor?.HttpContext?.User
                ?? throw new InvalidCredentialException();

            if (!int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ||
                !int.TryParse(user.FindFirstValue(ClaimTypes.Country), out var teamId) ||
                !Enum.TryParse<Role>(user.FindFirstValue(ClaimTypes.Role), ignoreCase: true, out var role))
            {
                throw new InvalidCredentialException("Invalid token claims.");
            }

            var users = userRepository.GetAll(x => x.IsDeleted == 0 && x.Team.IsDeleted == 0 && x.Room.IsDeleted == 0)
                .Include(x => x.CreatedGoals)
                .ThenInclude(x => x.MonthlyPerformance)
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
                return PagedResult<MonthlyPerformanceListModel>.Create(
                    Enumerable.Empty<MonthlyPerformanceListModel>(),
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

            string filterYear = dto?.Year.ToString() ?? DateTime.UtcNow.Year.ToString();

            List<MonthlyPerformanceListModel> models = list.Select(
                f => new MonthlyPerformanceListModel().MapFromEntity(f, filterYear, dto.Month.ToString(), false))
                .ToList();

            var pagedResult = PagedResult<MonthlyPerformanceListModel>.Create(models,
                totalCount,
                itemsPerPage,
                models.Count,
                dto.PageIndex,
                totalPages
            );
            return pagedResult;
        }

        public async ValueTask<PagedResult<MonthlyPerformanceListModel>> GetUsersForCEO([Required] MonthlyPerformanceForFilterDTO dto)
        {
            var query = userRepository.GetAll(x => x.Id != 1 && x.Role == Domain.Enum.Role.TeamLeader && x.IsDeleted == 0 && x.Team.IsDeleted == 0 && x.Room.IsDeleted == 0)
                .Include(x => x.CreatedGoals)
                .ThenInclude(x => x.MonthlyPerformance)
                .Include(x => x.Team)
                .Include(x => x.Evaluations)
                .Include(x => x.Room)
                .Include(x => x.Position)
                .OrderByDescending(x => x.UpdatedAt)
                .AsQueryable();

            int totalCount = await query.CountAsync();

            if (totalCount == 0)
            {
                return PagedResult<MonthlyPerformanceListModel>.Create(
                    Enumerable.Empty<MonthlyPerformanceListModel>(),
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

            string filterYear = dto?.Year.ToString() ?? DateTime.UtcNow.Year.ToString();

            List<MonthlyPerformanceListModel> models = list.Select(
                f => new MonthlyPerformanceListModel().MapFromEntity(f, filterYear, dto.Month.ToString(), false))
                .ToList();

            var pagedResult = PagedResult<MonthlyPerformanceListModel>.Create(models,
                totalCount,
                itemsPerPage,
                models.Count,
                dto.PageIndex,
                totalPages
                );

            return pagedResult;
        }

        public async ValueTask<PagedResult<MonthlyPerformanceListModel>> GetTeamLeader(MonthlyPerformanceForFilterDTO dto)
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

            var users = userRepository.GetAll(x => x.IsDeleted == 0 && x.Id == userId && x.TeamId == teamId && x.Team.IsDeleted == 0 && x.Room.IsDeleted == 0)
                .Include(x => x.CreatedGoals)
                .ThenInclude(x => x.MonthlyPerformance)
                .Include(x => x.Team)
                .Include(x => x.Evaluations)
                .Include(x => x.Room)
                .Include(x => x.Position)
                .OrderByDescending(x => x.UpdatedAt)
                .AsQueryable();


            int totalCount = await users.CountAsync();

            if (totalCount == 0)
            {
                return PagedResult<MonthlyPerformanceListModel>.Create(
                    Enumerable.Empty<MonthlyPerformanceListModel>(),
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

            string filterYear = dto?.Year.ToString() ?? DateTime.UtcNow.Year.ToString();

            var teamWithAllUsersFilled = await teamRepository.GetAll(x =>
                x.Id == teamId && x.IsDeleted == 0 && 
                x.Users.All(user => user.IsDeleted == 0 &&
                    user.CreatedGoals.Any(goal =>
                        goal.MonthlyPerformance.Any(mp => mp.Month == dto.Month && mp.Year == dto.Year && mp.Status == GoalStatus.Approved && mp.IsDeleted == 0)
                    )
                )
            ).FirstOrDefaultAsync();

            bool monthlyFinish = teamWithAllUsersFilled == null ? true : false;

            List<MonthlyPerformanceListModel> models = list.Select(
                f => new MonthlyPerformanceListModel().MapFromEntity(f, filterYear, dto.Month.ToString(), monthlyFinish))
                .ToList();

            var pagedResult = PagedResult<MonthlyPerformanceListModel>.Create(models,
                totalCount,
                itemsPerPage,
                models.Count,
                dto.PageIndex,
                totalPages
                );

            return pagedResult;
        }

        public async ValueTask<bool> ChangeMonthlyTargetStatus(ChangeGoalStatusDTO dto)
        {
            var existTargetValue = await monthlyPerformanceRepository.GetAsync(x => x.Id == dto.GoalId && x.IsDeleted == 0 && x.IsSended == true);

            if(existTargetValue is null) throw new KpiException(400, "monthly_target_value_not_found");

            existTargetValue.Status = dto.Status ? GoalStatus.Approved : GoalStatus.Returned;

            var newComment = new Domain.Entities.Comment.MonthlyTargetComment
            {
                Content = dto.Comment,
                Status = dto.Status ? Domain.Enum.GoalStatus.Approved : Domain.Enum.GoalStatus.Returned,
                MonthlyPerformanceId = existTargetValue.Id,
                CreatedById = GetUserIdFromContext()
            };

            await monthlyTargetCommentRepository.CreateAsync(newComment);
            await monthlyTargetCommentRepository.SaveChangesAsync();

            monthlyPerformanceRepository.UpdateAsync(existTargetValue);
            await monthlyPerformanceRepository.SaveChangesAsync();

            return true;
        }
    }
}
