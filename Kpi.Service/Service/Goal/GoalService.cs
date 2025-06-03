using Kpi.Domain.Entities.Goal;
using Kpi.Domain.Models.Goal;
using Kpi.Service.DTOs.Goal;
using Kpi.Service.Exception;
using Kpi.Service.Interfaces.Goal;
using Kpi.Service.Interfaces.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
        private readonly IHttpContextAccessor httpContextAccessor;
        public GoalService(IGenericRepository<KpiGoal> kpiGoalRepository,
            IGenericRepository<Division> divisionRepository,
            IGenericRepository<MonthlyTarget> monthlyTargetRepository,
            IGenericRepository<TargetValue> targetValueTargetRepository,
            IGenericRepository<Domain.Entities.User.User> userRepository,
            IHttpContextAccessor httpContextAccessor,
            IGenericRepository<Domain.Entities.Goal.Goal> goalRepository)
        {
            _kpiGoalRepository = kpiGoalRepository;
            _divisionRepository = divisionRepository;
            _monthlyTargetRepository = monthlyTargetRepository;
            _targetValueTargetRepository = targetValueTargetRepository;
            _userRepository = userRepository;
            this.httpContextAccessor = httpContextAccessor;
            _goalRepository = goalRepository;
        }

        public async ValueTask<bool> CreateAsync(GoalForCreationDTO @dto, int userId)
        {
            var goal = new Domain.Entities.Goal.Goal
            {
                CreatedAt = DateTime.UtcNow,
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
                    Name = division.Name
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


        public async ValueTask<GoalModel> GetByUserIdAsync(int id)
        {
            var model = await _goalRepository.GetAll(x => x.CreatedById == id && x.IsDeleted == 0)
                .Include(x => x.CreatedBy)
                .Include(x => x.AssignedTo)
                .Include(x => x.Comments)
                .Include(x => x.Divisions)
                .ThenInclude(x => x.Goals)
                .ThenInclude(x => x.TargetValue)
                .Include(x => x.MonthlyTargets)
                .FirstOrDefaultAsync();

            if (model == null) return null;

            return new GoalModel().MapFromEntity(model);
        }

        public async ValueTask<GoalModel> GetByTokenIdAsync(int id)
        {
            var model = await _goalRepository.GetAll(x => x.CreatedById == GetUserIdFromContext() && x.IsDeleted == 0)
                .Include(x => x.CreatedBy)
                .Include(x => x.AssignedTo)
                .Include(x => x.Comments)
                .Include(x => x.Divisions)
                .ThenInclude(x => x.Goals)
                .ThenInclude(x => x.TargetValue)
                .Include(x => x.MonthlyTargets)
                .FirstOrDefaultAsync();

            if (model == null) return null;

            return new GoalModel().MapFromEntity(model);
        }

        public async ValueTask<bool> CreateFromCEOAsync(GoalForCreationDTO @dto)
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
               .GetAll(goal => goal.CreatedAt.Year == DateTime.UtcNow.Year && ceoIds.Contains(goal.CreatedById) && goal.IsDeleted == 0)
               .AnyAsync();

            if (ceoGoalExists)
                throw new KpiException(400, "ceo_goal_already_exists");

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

        public async ValueTask<GoalModel> UpdateAsync(GoalForCreationDTO @dto)
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

            var newComment = new Domain.Entities.Comment.Comment
            {
                Content = dto.Comment,
                Status = Domain.Enum.GoalStatus.PendingReview,
                GoalId = existGoal.Id
            };

            foreach (var existingDivision in existGoal.Divisions)
            {
                var updatedDivision = dto.Divisions.FirstOrDefault(d => d.Id == existingDivision.Id);
                if (updatedDivision == null) continue;

                existingDivision.Name = updatedDivision.Name;

                foreach (var existingGoal in existingDivision.Goals)
                {
                    var updatedGoal = updatedDivision.Goals.FirstOrDefault(g => g.Id == existingGoal.Id);
                    if (updatedGoal == null) continue;

                    existingGoal.GoalContent = updatedGoal.GoalContent;
                    existingGoal.UpdatedAt = DateTime.UtcNow;

                    var existingTarget = existingGoal.TargetValue;
                    var updatedTarget = updatedGoal.TargetValue;

                    if (existingTarget != null && updatedTarget != null)
                    {
                        existingTarget.ValueText = updatedTarget.ValueText;
                        existingTarget.ValueNumber = updatedTarget.ValueNumber;
                        existingTarget.EvaluationText = updatedTarget.EvaluationText;
                        existingTarget.Status = updatedTarget.Status;
                        existingTarget.ValueRatio = updatedTarget.ValueRatio;
                        existingTarget.Type = (Domain.Enum.TargetValueType)updatedTarget.Type;
                        existingTarget.UpdatedAt = DateTime.UtcNow;

                        _targetValueTargetRepository.UpdateAsync(existingTarget);
                        await _targetValueTargetRepository.SaveChangesAsync();
                    }

                    _kpiGoalRepository.UpdateAsync(existingGoal);
                    await _kpiGoalRepository.SaveChangesAsync();
                }

                _divisionRepository.UpdateAsync(existingDivision);
                await _divisionRepository.SaveChangesAsync();
            }

            _goalRepository.UpdateAsync(existGoal);
            await _goalRepository.SaveChangesAsync();

            await _coomentRepository.CreateAsync(newComment);
            await _coomentRepository.SaveChangesAsync();

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
    }
}
