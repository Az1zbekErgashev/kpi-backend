using Kpi.Domain.Enum;
using Kpi.Infrastructure.Contexts;
using Kpi.Service.DTOs.Evaluation;
using Kpi.Service.Exception;
using Kpi.Service.Interfaces.Evaluation;
using Kpi.Service.Interfaces.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;
using System.Security.Claims;

namespace Kpi.Service.Service.Evaluation
{
    public class EvaluationService : IEvaluationService
    {
        private readonly IGenericRepository<Domain.Entities.Evaluation> evaluationService;
        private readonly IGenericRepository<Domain.Entities.Goal.Goal> goalService;
        private readonly IGenericRepository<Domain.Entities.User.User> userService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly KpiDB _context;

        public EvaluationService(IGenericRepository<Domain.Entities.Evaluation> evaluationService,
            IGenericRepository<Domain.Entities.User.User> userService,
            KpiDB context,
            IHttpContextAccessor httpContextAccessor,
            IGenericRepository<Domain.Entities.Goal.Goal> goalService)
        {
            this.evaluationService = evaluationService;
            this.userService = userService;
            _context = context;
            this.httpContextAccessor = httpContextAccessor;
            this.goalService = goalService;
        }

        public async ValueTask<IEnumerable<EmployeeEvaluationFroCreateDTO>> CreateOrUpdateAsync(List<EmployeeEvaluationFroCreateDTO> dtos)
        {
            var newEntities = new List<Domain.Entities.Evaluation>();

            foreach (var dto in dtos)
            {
                bool exists = await _context.Evaluations.AnyAsync(e =>
                    e.UserId == dto.UserId &&
                    e.KpiDivisionId == dto.KpiDivisionId &&
                    e.Year == dto.Year &&
                    e.Month == dto.Month);

                if (exists)
                {
                    continue;
                }

                var entity = MapToEntity(dto);
                newEntities.Add(entity);
            }

            if (newEntities.Any())
            {
                await _context.Evaluations.AddRangeAsync(newEntities);
                await _context.SaveChangesAsync();
            }

            return newEntities.Select(MapToDto).ToList();
        }

        private EmployeeEvaluationFroCreateDTO MapToDto(Domain.Entities.Evaluation entity)
        {
            return new EmployeeEvaluationFroCreateDTO
            {
                Id = entity.Id,
                UserId = entity.UserId,
                KpiDivisionId = entity.KpiDivisionId,
                Year = entity.Year,
                Month = entity.Month,
                Grade = entity.Grade,
                Modifier = entity.Modifier,
                Comment = entity.Comment
            };
        }

        private Domain.Entities.Evaluation MapToEntity(EmployeeEvaluationFroCreateDTO dto)
        {
            return new Domain.Entities.Evaluation
            {
                UserId = dto.UserId,
                KpiDivisionId = dto.KpiDivisionId,
                Year = dto.Year,
                Month = dto.Month,
                Grade = dto.Grade,
                Modifier = dto.Modifier,
                Comment = dto.Comment
            };
        }

        public async Task<IEnumerable<EmployeeEvaluationFroCreateDTO>> UpdateAsync(IEnumerable<EmployeeEvaluationFroCreateDTO> dtos)
        {
            if (dtos == null || !dtos.Any())
                throw new KpiException(400, "Список обновлений пуст");

            var updatedList = new List<EmployeeEvaluationFroCreateDTO>();

            foreach (var dto in dtos)
            {
                var entity = await _context.Evaluations
                    .FirstOrDefaultAsync(e => e.Id == dto.Id);

                if (entity == null)
                    throw new KpiException(404, $"Оценка с ID={dto.Id} не найдена");

                var employeeExists = await _context.Users.AnyAsync(e => e.Id == dto.UserId);
                if (!employeeExists)
                    throw new KpiException(404, $"Сотрудник ID={dto.UserId} не найден");

                var divisionExists = await _context.Divisions.AnyAsync(d => d.Id == dto.KpiDivisionId);
                if (!divisionExists)
                    throw new KpiException(404, $"KPI Division ID={dto.KpiDivisionId} не найден");

                var isDuplicate = await _context.Evaluations.AnyAsync(e =>
                    e.UserId == dto.UserId &&
                    e.KpiDivisionId == dto.KpiDivisionId &&
                    e.Year == dto.Year &&
                    e.Month == dto.Month &&
                    e.Id != dto.Id);

                if (isDuplicate)
                    throw new KpiException(404, $"Дубликат: сотрудник {dto.UserId}, division {dto.KpiDivisionId}, {dto.Month}.{dto.Year}");

                entity.UserId = dto.UserId;
                entity.KpiDivisionId = dto.KpiDivisionId;
                entity.Year = dto.Year;
                entity.Month = dto.Month;
                entity.Grade = dto.Grade;
                entity.Modifier = dto.Modifier;
                entity.Comment = dto.Comment;

                updatedList.Add(MapToDto(entity));
            }

            await _context.SaveChangesAsync();

            return updatedList;
        }

        public async Task<IEnumerable<TeamEvaluationResultDto>> GetTeamEvaluationsAsync(EvaluationsForFilterDTO dto)
        {
            var user = httpContextAccessor?.HttpContext?.User
             ?? throw new InvalidCredentialException();

            if (!int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ||
                !int.TryParse(user.FindFirstValue(ClaimTypes.Country), out var teamIdByToken) ||
                !Enum.TryParse<Role>(user.FindFirstValue(ClaimTypes.Role), ignoreCase: true, out var role))
            {
                throw new InvalidCredentialException("Invalid token claims.");
            }

            var teamId = dto.TeamId ?? teamIdByToken;

            var goalWithDivisions = await goalService.GetAll(x =>
                x.IsDeleted == 0 &&
                x.CreatedAt.Year == dto.Year &&
                x.CreatedBy.Role == Role.Ceo)
                .Include(x => x.Divisions)
                .FirstOrDefaultAsync();

                if (goalWithDivisions == null)
                    throw new KpiException(404, "Goal with divisions not found.");

                var allDivisions = goalWithDivisions.Divisions.ToList();

            var employees = await _context.Users
                    .Where(e => e.TeamId == teamId && e.IsDeleted == 0)
                    .ToListAsync();

            var employeeIds = employees.Select(e => e.Id).ToList();

            var evaluations = await _context.Evaluations
                .Where(e => employeeIds.Contains(e.UserId)
                            && e.Year == dto.Year
                            && e.Month == dto.Month
                            && e.User.IsDeleted == 0)
                .Include(e => e.KpiDivision)
                .ToListAsync();

            var result = employees.Select(emp =>
            {
                var employeeEvals = evaluations
                    .Where(e => e.UserId == emp.Id && emp.IsDeleted == 0)
                    .ToList();

                var divisionEvaluations = allDivisions.Select(division =>
                {
                    var eval = employeeEvals.FirstOrDefault(e => e.KpiDivisionId == division.Id && e.IsDeleted == 0);

                    return new DivisionEvaluationDto
                    {
                        KpiDivisionId = division.Id,
                        DivisionName = division.Name,
                        Grade = eval?.Grade,
                        Modifier = eval?.Modifier,
                        Comment = eval?.Comment
                    };
                }).ToList();

                return new TeamEvaluationResultDto
                {
                    EmployeeId = emp.Id,
                    FullName = emp.FullName,
                    DivisionEvaluations = divisionEvaluations
                };
            }).ToList();

            return result;
        }
    }
}
