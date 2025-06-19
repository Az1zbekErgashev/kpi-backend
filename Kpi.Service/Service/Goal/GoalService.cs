using Kpi.Domain.Entities.Comment;
using Kpi.Domain.Entities.Goal;
using Kpi.Domain.Enum;
using Kpi.Domain.Models.Goal;
using Kpi.Domain.Models.Team;
using Kpi.Service.DTOs.Goal;
using Kpi.Service.Exception;
using Kpi.Service.Interfaces.Goal;
using Kpi.Service.Interfaces.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Authentication;
using System.Security.Claims;


namespace Kpi.Service.Service.Goal
{
    public class GoalService : IGoalService
    {
        private readonly IGenericRepository<Domain.Entities.Goal.KpiGoal> _kpiGoalRepository;
        private readonly IGenericRepository<Domain.Entities.Goal.Goal> _goalRepository;
        private readonly IGenericRepository<Domain.Entities.Goal.Division> _divisionRepository;
        private readonly IGenericRepository<Domain.Entities.Goal.MonthlyTarget> _monthlyTargetRepository;
        private readonly IGenericRepository<Domain.Entities.Goal.TargetValue> _targetValueTargetRepository;
        private readonly IGenericRepository<Domain.Entities.User.User> _userRepository;
        private readonly IGenericRepository<Domain.Entities.Comment.Comment> _coomentRepository;
        private readonly IGenericRepository<Domain.Entities.Team.Team> teamRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        public GoalService(IGenericRepository<KpiGoal> kpiGoalRepository,
            IGenericRepository<Division> divisionRepository,
            IGenericRepository<MonthlyTarget> monthlyTargetRepository,
            IGenericRepository<TargetValue> targetValueTargetRepository,
            IGenericRepository<Domain.Entities.User.User> userRepository,
            IHttpContextAccessor httpContextAccessor,
            IGenericRepository<Domain.Entities.Goal.Goal> goalRepository,
            IGenericRepository<Comment> coomentRepository,
            IGenericRepository<Domain.Entities.Team.Team> teamRepository)
        {
            _kpiGoalRepository = kpiGoalRepository;
            _divisionRepository = divisionRepository;
            _monthlyTargetRepository = monthlyTargetRepository;
            _targetValueTargetRepository = targetValueTargetRepository;
            _userRepository = userRepository;
            this.httpContextAccessor = httpContextAccessor;
            _goalRepository = goalRepository;
            _coomentRepository = coomentRepository;
            this.teamRepository = teamRepository;
        }

        public async ValueTask<bool> CreateAsync(GoalForCreationDTO @dto, int userId)
        {
            var goal = new Domain.Entities.Goal.Goal
            {
                CreatedAt = dto.CreatetAt ?? DateTime.UtcNow,
                CreatedById = userId,
                Status = Domain.Enum.GoalStatus.PendingReview,
                Comments = new List<Domain.Entities.Comment.Comment>()
            };

            var comment = new Domain.Entities.Comment.Comment
            {
                Content = dto.Comment,
                CreatedById = userId,
                GoalId = goal.Id,
                Status = Domain.Enum.GoalStatus.PendingReview
            };

            goal.Comments.Add(comment);

            await _goalRepository.CreateAsync(goal);
            await _goalRepository.SaveChangesAsync();


            foreach (var division in dto.Divisions)
            {
                var newDivision = new Domain.Entities.Goal.Division
                {
                    CreatedAt = DateTime.UtcNow,
                    GoalId = goal.Id,
                    Name = division.Name,
                    Ratio = division.Ratio
                };

                await _divisionRepository.CreateAsync(newDivision);
                await _divisionRepository.SaveChangesAsync();

                foreach (var item in division.Goals)
                {
                    var newKpiGoal = new Domain.Entities.Goal.KpiGoal
                    {
                        DivisionId = newDivision.Id,
                        GoalContent = item.GoalContent,
                        TargetValue = new Domain.Entities.Goal.TargetValue
                        {
                            Status = item.TargetValue.Status,
                            EvaluationText = item.TargetValue.EvaluationText,
                            ValueText = item.TargetValue.ValueText,
                            Type = (Domain.Enum.TargetValueType)item.TargetValue.Type,
                            ValueNumber = item.TargetValue.ValueNumber,
                            ValueRatio = item.TargetValue.ValueRatio
                        }
                    };
                    await _kpiGoalRepository.CreateAsync(newKpiGoal);
                }
            }
            await _kpiGoalRepository.SaveChangesAsync();

            return true;
        }

        public async ValueTask<GoalModel> GetByIdAsync(int id)
        {
            var model = await _goalRepository.GetAll(x => x.Id == id && x.IsDeleted == 0)
                .Include(x => x.CreatedBy)
                .ThenInclude(x => x.Team)
                .Include(x => x.CreatedBy)
                .ThenInclude(x => x.Room)
                .Include(x => x.AssignedTo)
                .Include(x => x.Comments)
                .Include(x => x.Divisions)
                .ThenInclude(x => x.Goals)
                .ThenInclude(x => x.TargetValue)
                .Include(x => x.MonthlyTargets)
                .FirstOrDefaultAsync();

            if (model == null) throw new KpiException(404, "goal_not_found");

            return new GoalModel().MapFromEntity(model);
        }

        public async ValueTask<GoalModel> GetByUserIdAsync(int id, int year)
        {
            var model = await _goalRepository.GetAll(x => x.CreatedBy.TeamId == id && x.IsDeleted == 0 && x.CreatedAt.Year == year && x.CreatedBy.Role == Domain.Enum.Role.TeamLeader)
                .Include(x => x.CreatedBy)
                .ThenInclude(x => x.Team)
                .Include(x => x.CreatedBy)
                .ThenInclude(x => x.Room)
                .Include(x => x.AssignedTo)
                .Include(x => x.Comments)
                .Include(x => x.Divisions)
                .ThenInclude(x => x.Goals)
                .ThenInclude(x => x.TargetValue)
                .Include(x => x.MonthlyTargets)
                .FirstOrDefaultAsync();

            if (model == null) throw new KpiException(404, "goal_not_found");

            return new GoalModel().MapFromEntity(model);
        }

        public async ValueTask<GoalModel> GetByTokenIdAsync(int year)
        {
            var model = await _goalRepository.GetAll(x => x.CreatedById == GetUserIdFromContext() && x.IsDeleted == 0 && x.CreatedAt.Year == year)
                .Include(x => x.CreatedBy)
                .ThenInclude(x => x.Team)
                .Include(x => x.CreatedBy)
                .ThenInclude(x => x.Room)
                .Include(x => x.AssignedTo)
                .Include(x => x.Comments)
                .Include(x => x.Divisions)
                .ThenInclude(x => x.Goals)
                .ThenInclude(x => x.TargetValue)
                .Include(x => x.MonthlyTargets)
                .FirstOrDefaultAsync();

            if (model == null) throw new KpiException(404, "goal_not_found");

            return new GoalModel().MapFromEntity(model);
        }

        public async ValueTask<GoalModel> GetByCeoGoal(int year)
        {
            var model = await _goalRepository.GetAll(x => x.CreatedBy.Role == Domain.Enum.Role.Ceo && x.CreatedAt.Year == year && x.IsDeleted == 0)
               .Include(x => x.CreatedBy)
               .ThenInclude(x => x.Team)
               .Include(x => x.CreatedBy)
               .ThenInclude(x => x.Room)
               .Include(x => x.Divisions)
               .ThenInclude(x => x.Goals)
               .ThenInclude(x => x.TargetValue)
               .FirstOrDefaultAsync();

            if (model == null) throw new KpiException(404, "goal_not_found");

            return new GoalModel().MapFromEntity(model);
        }

        public async ValueTask<bool> CreateFromCEOAsync(GoalForCreationDTO @dto, int year)
        {
            var userId = GetUserIdFromContext();

            var existUser = await _userRepository
             .GetAll(x => x.Id == userId && x.Role == Domain.Enum.Role.Ceo && x.IsDeleted == 0)
             .FirstOrDefaultAsync();

            if (existUser == null) throw new KpiException(404, "user_not_found");

            var ceoIds = await _userRepository
            .GetAll(x => x.Role == Domain.Enum.Role.Ceo)
            .Select(x => x.Id)
            .ToListAsync();

            bool ceoGoalExists = await _goalRepository
               .GetAll(goal => goal.CreatedAt.Year == year && ceoIds.Contains(goal.CreatedById) && goal.IsDeleted == 0)
               .AnyAsync();

            if (ceoGoalExists)
                throw new KpiException(400, "ceo_goal_already_exists");

            dto.CreatetAt = new DateTime(year, 1, 1);

            return await CreateAsync(dto, userId);
        }

        public async ValueTask<bool> CreateFromTeamLeaderAsync(GoalForCreationDTO @dto)
        {
            var userId = GetUserIdFromContext();

            var existUser = await _userRepository
             .GetAll(x => x.Id == userId && (x.Role == Domain.Enum.Role.TeamLeader || x.Role == Domain.Enum.Role.TeamMember) && x.IsDeleted == 0)
             .Include(x => x.CreatedGoals)
             .FirstOrDefaultAsync();

            if (existUser == null) throw new KpiException(404, "user_not_found");

            var existGoal = existUser.CreatedGoals.Where(x => x.CreatedAt.Year == DateTime.UtcNow.Year && x.IsDeleted == 0).FirstOrDefault();

            if (existGoal != null) throw new KpiException(400, "goal_exist");

            return await CreateAsync(dto, userId);
        }

        public async ValueTask<bool> ChangeGoalStatus(ChangeGoalStatusDTO dto)
        {
            var existGoal = await _goalRepository.GetAll(x => x.Id == dto.GoalId && x.IsDeleted == 0)
                .Include(x => x.Comments)
                .FirstOrDefaultAsync();

            if (existGoal == null)
                throw new KpiException(404, "goal_not_found");

            var newComment = new Domain.Entities.Comment.Comment
            {
                Content = dto.Comment,
                Status = dto.Status ? Domain.Enum.GoalStatus.Approved : Domain.Enum.GoalStatus.Returned,
                GoalId = existGoal.Id
            };

            await _coomentRepository.CreateAsync(newComment);
            await _coomentRepository.SaveChangesAsync();

            existGoal.Status = dto.Status ? Domain.Enum.GoalStatus.Approved : Domain.Enum.GoalStatus.Returned;
            existGoal.UpdatedAt = DateTime.UtcNow;

            _goalRepository.UpdateAsync(existGoal);
            await _goalRepository.SaveChangesAsync();

            return true;
        }

        public async ValueTask<GoalModel> UpdateAsync(GoalForCreationDTO dto)
        {
            var existGoal = await _goalRepository.GetAll(x => x.Id == dto.GoalId && x.IsDeleted == 0)
                .Include(x => x.CreatedBy)
                .Include(x => x.AssignedTo)
                .Include(x => x.Comments)
                .Include(x => x.Divisions)
                    .ThenInclude(d => d.Goals)
                        .ThenInclude(g => g.TargetValue)
                .Include(x => x.MonthlyTargets)
                .FirstOrDefaultAsync();

            if (existGoal == null)
                throw new KpiException(404, "goal_not_found");

            existGoal.UpdatedAt = DateTime.UtcNow;
            existGoal.Status = Domain.Enum.GoalStatus.PendingReview;

            var comment = new Comment
            {
                GoalId = existGoal.Id,
                CreatedById = GetUserIdFromContext(),
                Content = dto.Comment,
                Status = Domain.Enum.GoalStatus.PendingReview,
            };

            if (dto.Divisions != null && dto.Divisions.Any())
            {
                var incomingDivisionIds = dto.Divisions.Select(d => d?.Id).ToList();
                var existingDivisions = existGoal.Divisions.ToList();

                foreach (var divisionDto in dto.Divisions)
                {
                    var existingDivision = existingDivisions.FirstOrDefault(d => d.Id == divisionDto?.Id);

                    if (existingDivision != null)
                    {
                        existingDivision.Name = divisionDto.Name;
                        existingDivision.Ratio = divisionDto.Ratio;
                        existingDivision.UpdatedAt = DateTime.UtcNow;
                        _divisionRepository.UpdateAsync(existingDivision);

                        var existingGoals = existingDivision.Goals.ToList();
                        var incomingGoalIds = divisionDto.Goals?.Select(g => g?.Id).ToList();

                        if (divisionDto.Goals == null || !divisionDto.Goals.Any())
                            throw new KpiException(400, "please_fill_fields");

                        foreach (var goalDto in divisionDto.Goals)
                        {
                            var existingGoal = existingGoals.FirstOrDefault(g => g.Id == goalDto?.Id);

                            if (existingGoal != null)
                            {
                                existingGoal.GoalContent = goalDto.GoalContent;
                                existingGoal.UpdatedAt = DateTime.UtcNow;

                                if (goalDto.TargetValue == null)
                                    throw new KpiException(400, "please_fill_fields");

                                var target = existingGoal.TargetValue;
                                target.ValueText = goalDto.TargetValue.ValueText;
                                target.ValueNumber = goalDto.TargetValue.ValueNumber;
                                target.EvaluationText = goalDto.TargetValue.EvaluationText;
                                target.Status = goalDto.TargetValue.Status;
                                target.ValueRatio = goalDto.TargetValue.ValueRatio;
                                target.Type = (Domain.Enum.TargetValueType)goalDto.TargetValue.Type;
                                target.UpdatedAt = DateTime.UtcNow;

                                _targetValueTargetRepository.UpdateAsync(target);
                            }
                            else
                            {
                                if (goalDto.TargetValue == null)
                                    throw new KpiException(400, "please_fill_fields");

                                var newGoal = new KpiGoal
                                {
                                    GoalContent = goalDto.GoalContent,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow,
                                    DivisionId = existingDivision.Id,
                                    TargetValue = new TargetValue
                                    {
                                        ValueText = goalDto.TargetValue.ValueText,
                                        ValueNumber = goalDto.TargetValue.ValueNumber,
                                        EvaluationText = goalDto.TargetValue.EvaluationText,
                                        Status = goalDto.TargetValue.Status,
                                        ValueRatio = goalDto.TargetValue.ValueRatio,
                                        Type = (Domain.Enum.TargetValueType)goalDto.TargetValue.Type,
                                        CreatedAt = DateTime.UtcNow,
                                        UpdatedAt = DateTime.UtcNow
                                    }
                                };

                                existingDivision.Goals.Add(newGoal);
                            }
                        }

                        var goalsToDelete = existingGoals.Where(g => !incomingGoalIds.Contains(g.Id)).ToList();
                        foreach (var goalToDelete in goalsToDelete)
                        {
                            existingDivision.Goals.Remove(goalToDelete);
                        }
                    }
                    else
                    {
                        var newDivision = new Division
                        {
                            Name = divisionDto.Name,
                            Ratio = divisionDto.Ratio,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            Goals = new List<KpiGoal>()
                        };

                        await _divisionRepository.CreateAsync(newDivision);

                        if (divisionDto.Goals == null || !divisionDto.Goals.Any())
                            throw new KpiException(400, "please_fill_fields");

                        foreach (var goalDto in divisionDto.Goals)
                        {
                            if (goalDto.TargetValue == null)
                                throw new KpiException(400, "please_fill_fields");

                            var newGoal = new KpiGoal
                            {
                                GoalContent = goalDto.GoalContent,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow,
                                TargetValue = new TargetValue
                                {
                                    ValueText = goalDto.TargetValue.ValueText,
                                    ValueNumber = goalDto.TargetValue.ValueNumber,
                                    EvaluationText = goalDto.TargetValue.EvaluationText,
                                    Status = goalDto.TargetValue.Status,
                                    ValueRatio = goalDto.TargetValue.ValueRatio,
                                    Type = (Domain.Enum.TargetValueType)goalDto.TargetValue.Type,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow
                                }
                            };

                            newDivision.Goals.Add(newGoal);
                        }

                        existGoal.Divisions.Add(newDivision);
                    }
                }

                var divisionsToDelete = existingDivisions.Where(d => !incomingDivisionIds.Contains(d.Id)).ToList();
                foreach (var divisionToDelete in divisionsToDelete)
                {
                    await _divisionRepository.DeleteAsync(divisionToDelete.Id);
                }

                await _divisionRepository.SaveChangesAsync();
            }

            else throw new KpiException(400, "please_fill_fields");

            await _goalRepository.SaveChangesAsync();
            await _kpiGoalRepository.SaveChangesAsync();
            await _coomentRepository.CreateAsync(comment);
            await _coomentRepository.SaveChangesAsync();
            await _targetValueTargetRepository.SaveChangesAsync();

            return new GoalModel().MapFromEntity(existGoal);
        }

        public async ValueTask<bool> SendGoalRequest(GoalForSendDTO @dto)
        {
            var existGoal = await _goalRepository.GetAll(x => x.Id == dto.GoalId && x.IsDeleted == 0)
                .Include(x => x.Comments)
                .FirstOrDefaultAsync();

            if (existGoal == null)
                throw new KpiException(404, "goal_not_found");

            var newComment = new Domain.Entities.Comment.Comment
            {
                Content = dto.Comment,
                Status = Domain.Enum.GoalStatus.PendingReview,
                GoalId = existGoal.Id
            };

            await _coomentRepository.CreateAsync(newComment);
            await _coomentRepository.SaveChangesAsync();

            existGoal.Status = Domain.Enum.GoalStatus.PendingReview;
            existGoal.UpdatedAt = DateTime.UtcNow;

            _goalRepository.UpdateAsync(existGoal);
            await _goalRepository.SaveChangesAsync();

            return true;
        }
        private int GetUserIdFromContext()
        {
            if (!int.TryParse(httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                throw new InvalidCredentialException();
            return userId;
        }

        public async ValueTask<GoalModel> GetByTeamIdAsync(int id, int year)
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
                     x => x.Id == teamId && x.Users.Any(o => o.Id == id)
                 );

            if (existUserThisTeam == null) throw new KpiException(404, "user_not_found");

            var model = await _goalRepository.GetAll(x => x.CreatedBy.TeamId == teamId && x.IsDeleted == 0 && x.CreatedAt.Year == year && x.CreatedById == id)
                .Include(x => x.CreatedBy)
                .ThenInclude(x => x.Team)
                .Include(x => x.CreatedBy)
                .ThenInclude(x => x.Room)
                .Include(x => x.AssignedTo)
                .Include(x => x.Comments)
                .Include(x => x.Divisions)
                .ThenInclude(x => x.Goals)
                .ThenInclude(x => x.TargetValue)
                .Include(x => x.MonthlyTargets)
                .FirstOrDefaultAsync();

            if (model == null) throw new KpiException(404, "goal_not_found");

            return new GoalModel().MapFromEntity(model);
        }

        public async ValueTask<GoalModel> GetTeamLeaderGoal(int year)
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
                     x => x.Id == teamId && x.Users.Any(o => o.Role == Role.TeamLeader)
                 );

            if (existUserThisTeam == null) throw new KpiException(404, "team_leader_not_found");

            var model = await _goalRepository.GetAll(x => x.CreatedBy.TeamId == teamId && x.IsDeleted == 0 && x.CreatedAt.Year == year && x.CreatedBy.Role == Role.TeamLeader)
                .Include(x => x.CreatedBy)
                .ThenInclude(x => x.Team)
                .Include(x => x.CreatedBy)
                .ThenInclude(x => x.Room)
                .Include(x => x.AssignedTo)
                .Include(x => x.Comments)
                .Include(x => x.Divisions)
                .ThenInclude(x => x.Goals)
                .ThenInclude(x => x.TargetValue)
                .Include(x => x.MonthlyTargets)
                .FirstOrDefaultAsync();

            if (model == null) throw new KpiException(404, "goal_not_found");

            return new GoalModel().MapFromEntity(model);
        }

        public async ValueTask<TeamAndRoom> GetRoomAndTeam(int teamId)
        {
            var team = await teamRepository.GetAll(x => x.Id == teamId && x.IsDeleted == 0).Include(x => x.Users).ThenInclude(x => x.Room).FirstOrDefaultAsync();


            if (team is null) throw new KpiException(404, "team_not_found");

            var activeUsers = team?.Users.Where(x => x.IsDeleted == 0);

            string teamName = team.Name;
            string roomName = activeUsers?.FirstOrDefault()?.Room?.Name;
            int? roomId = activeUsers?.FirstOrDefault()?.Room?.Id;

            return new TeamAndRoom().MapFromEntity(team.Id, roomId, teamName, roomName);
        }
        
        public async ValueTask<TeamAndRoom> GetRoomAndTeamByToken()
        {
            var user = httpContextAccessor?.HttpContext?.User
             ?? throw new InvalidCredentialException();

            if (!int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ||
                !int.TryParse(user.FindFirstValue(ClaimTypes.Country), out var teamId) ||
                !Enum.TryParse<Role>(user.FindFirstValue(ClaimTypes.Role), ignoreCase: true, out var role))
            {
                throw new InvalidCredentialException("Invalid token claims.");
            }

            var team = await teamRepository.GetAll(x => x.Id == teamId && x.IsDeleted == 0).Include(x => x.Users).ThenInclude(x => x.Room).FirstOrDefaultAsync();


            if (team is null) throw new KpiException(404, "team_not_found");

            var activeUsers = team?.Users.Where(x => x.IsDeleted == 0);

            string teamName = team.Name;
            string roomName = activeUsers?.FirstOrDefault()?.Room?.Name;
            int? roomId = activeUsers?.FirstOrDefault()?.Room?.Id;

            return new TeamAndRoom().MapFromEntity(team.Id, roomId, teamName, roomName);
        }
    }
}
