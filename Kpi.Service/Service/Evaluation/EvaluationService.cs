using DocumentFormat.OpenXml.InkML;
using Kpi.Domain.Enum;
using Kpi.Infrastructure.Contexts;
using Kpi.Service.DTOs.Evaluation;
using Kpi.Service.Exception;
using Kpi.Service.Interfaces.Evaluation;
using Kpi.Service.Interfaces.IRepositories;
using MathNet.Numerics.Distributions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;

namespace Kpi.Service.Service.Evaluation
{
    public class EvaluationService : IEvaluationService
    {
        private readonly IGenericRepository<Domain.Entities.Evaluation> evaluationService;
        private readonly IGenericRepository<Domain.Entities.Goal.Division> divisionService;
        private readonly IGenericRepository<Domain.Entities.ScoreManagement> scoreManagementService;
        private readonly IGenericRepository<Domain.Entities.Goal.Goal> goalService;
        private readonly IGenericRepository<Domain.Entities.User.User> userService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly KpiDB _context;

        public EvaluationService(IGenericRepository<Domain.Entities.Evaluation> evaluationService,
            IGenericRepository<Domain.Entities.User.User> userService,
            KpiDB context,
            IHttpContextAccessor httpContextAccessor,
            IGenericRepository<Domain.Entities.Goal.Goal> goalService,
            IGenericRepository<Domain.Entities.ScoreManagement> scoreManagementService,
            IGenericRepository<Domain.Entities.Goal.Division> divisionService)
        {
            this.evaluationService = evaluationService;
            this.userService = userService;
            _context = context;
            this.httpContextAccessor = httpContextAccessor;
            this.goalService = goalService;
            this.scoreManagementService = scoreManagementService;
            this.divisionService = divisionService;
        }

        public async ValueTask<IEnumerable<EmployeeEvaluationFroCreateDTO>> CreateOrUpdateAsync(List<EmployeeEvaluationFroCreateDTO> dtos)
        {
            if (dtos == null || !dtos.Any())
                throw new KpiException(400, "Список обновлений пуст");

            var result = new List<EmployeeEvaluationFroCreateDTO>();

            foreach (var dto in dtos)
            {
                var employeeExists = await _context.Users.AnyAsync(e => e.Id == dto.UserId);
                if (!employeeExists)
                    throw new KpiException(404, $"Сотрудник ID={dto.UserId} не найден");

                var divisionExists = await _context.Divisions.AnyAsync(d => d.Id == dto.KpiDivisionId);
                if (!divisionExists)
                    throw new KpiException(404, $"KPI Division ID={dto.KpiDivisionId} не найден");

                Domain.Entities.Evaluation entity = null;

                if (dto.Id > 0)
                {
                    entity = await _context.Evaluations.FirstOrDefaultAsync(e => e.Id == dto.Id);
                }

                if (entity == null)
                {
                    entity = await _context.Evaluations.FirstOrDefaultAsync(e =>
                        e.UserId == dto.UserId &&
                        e.KpiDivisionId == dto.KpiDivisionId &&
                        e.Year == dto.Year &&
                        e.Month == dto.Month);
                }

                var score = await scoreManagementService.GetAsync(x => x.Id == dto.ScoreId);

                if (entity == null)
                {
                    entity = new Domain.Entities.Evaluation
                    {
                        UserId = dto.UserId,
                        KpiDivisionId = dto.KpiDivisionId,
                        Year = dto.Year,
                        Month = dto.Month,
                        Comment = dto.Comment,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        ScoreManagementId = score?.Id
                    };

                    await _context.Evaluations.AddAsync(entity);
                }
                else
                {
                    entity.Comment = dto.Comment;
                    entity.UpdatedAt = DateTime.UtcNow;
                    entity.ScoreManagementId = score?.Id;
                }

                result.Add(MapToDto(entity, score?.Id));
            }

            await _context.SaveChangesAsync();
            return result;
        }

        private EmployeeEvaluationFroCreateDTO MapToDto(Domain.Entities.Evaluation entity, int? scoreId)
        {
            return new EmployeeEvaluationFroCreateDTO
            {
                Id = entity.Id,
                UserId = entity.UserId,
                KpiDivisionId = entity.KpiDivisionId,
                Year = entity.Year,
                Month = entity.Month,
                ScoreId = scoreId,
                Comment = entity?.Comment,
            };
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

            var employeeIds = employees?.Select(e => e.Id).ToList();

            var evaluations = await _context.Evaluations
                .Where(e => employeeIds.Contains(e.UserId)
                            && e.Year == dto.Year
                            && e.Month == dto.Month
                            && e.User.IsDeleted == 0)
                .Include(e => e.KpiDivision)
                .Include(x => x.ScoreManagement)
                .ToListAsync();

            var result = employees?.Select(emp =>
            {
                var employeeEvals = evaluations
                    ?.Where(e => e.UserId == emp.Id && emp.IsDeleted == 0)
                    ?.ToList();

                var divisionEvaluations = allDivisions?.Select(division =>
                {
                    var eval = employeeEvals?.FirstOrDefault(e => e.KpiDivisionId == division.Id && e.IsDeleted == 0);

                    return new DivisionEvaluationDto
                    {
                        KpiDivisionId = division.Id,
                        DivisionName = division.Name,
                        Ratio = division.Ratio,
                        Grade = eval?.ScoreManagement?.Grade,
                        ScoreId = eval?.ScoreManagement?.Id,
                        Comment = eval?.Comment,
                        Score = eval?.ScoreManagement?.Score,
                        Id = eval?.Id,
                    };
                }).ToList();

                return new TeamEvaluationResultDto
                {
                    EmployeeId = emp.Id,
                    FullName = emp.FullName,
                    DivisionEvaluations = divisionEvaluations,
                };
            })?.ToList();

            return result;
        }

        public async ValueTask<object> GetAllEvaluation(int year)
        {
            var user = httpContextAccessor?.HttpContext?.User
             ?? throw new InvalidCredentialException();

            if (!int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ||
                !int.TryParse(user.FindFirstValue(ClaimTypes.Country), out var teamIdByToken) ||
                !Enum.TryParse<Role>(user.FindFirstValue(ClaimTypes.Role), ignoreCase: true, out var role))
            {
                throw new InvalidCredentialException("Invalid token claims.");
            }

            var divisions = await goalService.GetAll(x => x.CreatedAt.Year == year && x.CreatedBy.Role == Role.Ceo && x.IsDeleted == 0).Include(x => x.Divisions).FirstOrDefaultAsync();

            if (divisions is null) throw new KpiException(404, "goal_not_found");

            var evaluations = await evaluationService.GetAll(x => x.IsDeleted == 0 && x.Year == year)
                .Include(x => x.User)
                .ThenInclude(x => x.Position)
                .Include(x => x.User)
                .ThenInclude(x => x.Team)
                .Include(x => x.User)
                .ThenInclude(x => x.Room)
                .Include(x => x.KpiDivision)
                .Include(x => x.ScoreManagement)
                .ToListAsync();


            if (role == Role.TeamLeader) evaluations = evaluations.Where(x => x.User.TeamId == teamIdByToken).ToList();

            var allDivisionNames = divisions.Divisions
             .Where(d => !string.IsNullOrWhiteSpace(d.Name))
             .Select(d => new
             {
                 Name = d.Name,
                 Ratio = d.Ratio,
                 Id = d.Id,
             })
             .ToList();

            var studentWithFinals = evaluations
                .GroupBy(e => e.UserId)
                .Select(group =>
                {
                    var first = group.First();

                    var grades = allDivisionNames.ToDictionary(
                     div => div.Id, // ключ
                     div => Enumerable.Range(1, 12).ToDictionary(
                         month => month.ToString(), // ключ
                         month =>
                         {
                             var match = group.FirstOrDefault(e =>
                                 e.KpiDivisionId == div.Id &&
                                 e.Month == month);
                             return match?.ScoreManagement?.Grade?.ToString(); // ← может вернуть null!
                         }
                     )
                 );

                    var divisionResults = new List<object>();
                    double totalFinalScore = 0;

                    foreach (var div in allDivisionNames)
                    {

                        var scoresByMonth = Enumerable.Range(1, 12)
                         .Select(month =>
                             group.FirstOrDefault(e =>
                                 e.KpiDivisionId == div.Id &&
                                 e.Month == month)?.ScoreManagement?.Score)
                         .Where(score => score.HasValue) 
                         .Select(score => score.Value)
                         .ToList();

                        var monthlyAvg = scoresByMonth.Any() ? scoresByMonth.Average() : 0;

                        var adjusted = Math.Round(monthlyAvg); // 환산 값
                        var weightedScore = adjusted * (div.Ratio / 100.0);
                        totalFinalScore += (double)weightedScore;

                        divisionResults.Add(new
                        {
                            divisionId = div.Id,
                            average = Math.Round(monthlyAvg, 2),
                            adjusted = adjusted,
                            weighted = Math.Round((decimal)weightedScore, 2)
                        });
                    }

                    return new
                    {
                        id = first.UserId.ToString(),
                        room = first.User.Room?.Name,
                        name = first.User.FullName,
                        position = first.User.Position?.Name,
                        department = first.User.Team?.Name,
                        date = first.CreatedAt.ToString("dd.MM.yyyy"),
                        grades,
                        finalScore = Math.Round(totalFinalScore, 2),
                        divisions = divisionResults
                    };
                })
                .ToList();

            var evaluationPeriods = allDivisionNames
                 .Select(div => new
                 {
                     id = div.Id,
                     name = div.Name,
                     percentage = div.Ratio,
                     periods = Enumerable.Range(1, 12).ToList(),
                     description = "KPI Division Assessment"
                 })
                 .ToList();

            return new
            {
                students = studentWithFinals,
                evaluationPeriods
            };
        }

        public async ValueTask<object> GetAllEvaluationByTeam(int year, int teamId)
        {
            var user = httpContextAccessor?.HttpContext?.User
                 ?? throw new InvalidCredentialException();

            if (!int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ||
                !Enum.TryParse<Role>(user.FindFirstValue(ClaimTypes.Role), ignoreCase: true, out var role))
            {
                throw new InvalidCredentialException("Invalid token claims.");
            }

            var divisions = await goalService.GetAll(x => x.CreatedAt.Year == year && x.CreatedBy.Role == Role.Ceo && x.IsDeleted == 0).Include(x => x.Divisions).FirstOrDefaultAsync();

            if (divisions is null) throw new KpiException(404, "goal_not_found");

            var evaluations = await evaluationService.GetAll(x => x.IsDeleted == 0 && x.Year == year && x.User.TeamId == teamId)
                .Include(x => x.User)
                .ThenInclude(x => x.Position)
                .Include(x => x.User)
                .ThenInclude(x => x.Team)
                .Include(x => x.User)
                .ThenInclude(x => x.Room)
                .Include(x => x.KpiDivision)
                .Include(x => x.ScoreManagement)
                .ToListAsync();

            var allDivisionNames = divisions.Divisions
             .Where(d => !string.IsNullOrWhiteSpace(d.Name))
             .Select(d => new
             {
                 Name = d.Name,
                 Ratio = d.Ratio,
                 Id = d.Id,
             })
             .ToList();

            var studentWithFinals = evaluations
                .GroupBy(e => e.UserId)
                .Select(group =>
                {
                    var first = group.First();

                    var grades = allDivisionNames.ToDictionary(
                     div => div.Id, // ключ
                     div => Enumerable.Range(1, 12).ToDictionary(
                         month => month.ToString(), // ключ
                         month =>
                         {
                             var match = group.FirstOrDefault(e =>
                                 e.KpiDivisionId == div.Id &&
                                 e.Month == month);
                             return match?.ScoreManagement?.Grade?.ToString(); // ← может вернуть null!
                         }
                     )
                 );

                    var divisionResults = new List<object>();
                    double totalFinalScore = 0;

                    foreach (var div in allDivisionNames)
                    {

                        var scoresByMonth = Enumerable.Range(1, 12)
                         .Select(month =>
                             group.FirstOrDefault(e =>
                                 e.KpiDivisionId == div.Id &&
                                 e.Month == month)?.ScoreManagement?.Score)
                         .Where(score => score.HasValue)
                         .Select(score => score.Value)
                         .ToList();

                        var monthlyAvg = scoresByMonth.Any() ? scoresByMonth.Average() : 0;

                        var adjusted = Math.Round(monthlyAvg); // 환산 값
                        var weightedScore = adjusted * (div.Ratio / 100.0);
                        totalFinalScore += (double)weightedScore;

                        divisionResults.Add(new
                        {
                            divisionId = div.Id,
                            average = Math.Round(monthlyAvg, 2),
                            adjusted = adjusted,
                            weighted = Math.Round((decimal)weightedScore, 2)
                        });
                    }

                    return new
                    {
                        id = first.UserId.ToString(),
                        room = first.User.Room?.Name,
                        name = first.User.FullName,
                        position = first.User.Position?.Name,
                        department = first.User.Team?.Name,
                        date = first.CreatedAt.ToString("dd.MM.yyyy"),
                        grades,
                        finalScore = Math.Round(totalFinalScore, 2),
                        divisions = divisionResults
                    };
                })
                .ToList();

            var evaluationPeriods = allDivisionNames
                 .Select(div => new
                 {
                     id = div.Id,
                     name = div.Name,
                     percentage = div.Ratio,
                     periods = Enumerable.Range(1, 12).ToList(),
                     description = "KPI Division Assessment"
                 })
                 .ToList();

            return new
            {
                students = studentWithFinals,
                evaluationPeriods
            };
        }

        public async ValueTask<object> GetAllEvaluationByYear(int year)
        {
            var user = httpContextAccessor?.HttpContext?.User
              ?? throw new InvalidCredentialException();

            if (!int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ||
                !Enum.TryParse<Role>(user.FindFirstValue(ClaimTypes.Role), ignoreCase: true, out var role))
            {
                throw new InvalidCredentialException("Invalid token claims.");
            }

            var divisions = await goalService.GetAll(x => x.CreatedAt.Year == year && x.CreatedBy.Role == Role.Ceo && x.IsDeleted == 0)
                .Include(x => x.Divisions)
                .FirstOrDefaultAsync();

            if (divisions is null) throw new KpiException(404, "goal_not_found");

            var evaluations = await evaluationService.GetAll(x => x.IsDeleted == 0 && x.Year == year)
                .Include(x => x.User)
                .ThenInclude(x => x.Position)
                .Include(x => x.User)
                .ThenInclude(x => x.Team)
                .Include(x => x.User)
                .ThenInclude(x => x.Room)
                .Include(x => x.KpiDivision)
                .Include(x => x.ScoreManagement)
                .ToListAsync();

            var allDivisionNames = divisions.Divisions
              .Where(d => !string.IsNullOrWhiteSpace(d.Name))
              .Select(d => new
              {
                  Name = d.Name,
                  Ratio = d.Ratio,
                  Id = d.Id,
              })
              .ToList();

            var studentWithFinals = evaluations
                .GroupBy(e => e.UserId)
                .Select(group =>
                {
                    var first = group.First();

                    var grades = allDivisionNames.ToDictionary(
                     div => div.Id, // ключ
                     div => Enumerable.Range(1, 12).ToDictionary(
                         month => month.ToString(), // ключ
                         month =>
                         {
                             var match = group.FirstOrDefault(e =>
                                 e.KpiDivisionId == div.Id &&
                                 e.Month == month);
                             return match?.ScoreManagement?.Grade?.ToString(); // ← может вернуть null!
                         }
                     )
                 );

                    var divisionResults = new List<object>();
                    double totalFinalScore = 0;

                    foreach (var div in allDivisionNames)
                    {

                        var scoresByMonth = Enumerable.Range(1, 12)
                         .Select(month =>
                             group.FirstOrDefault(e =>
                                 e.KpiDivisionId == div.Id &&
                                 e.Month == month)?.ScoreManagement?.Score)
                         .Where(score => score.HasValue)
                         .Select(score => score.Value)
                         .ToList();

                        var monthlyAvg = scoresByMonth.Any() ? scoresByMonth.Average() : 0;

                        var adjusted = Math.Round(monthlyAvg); // 환산 값
                        var weightedScore = adjusted * (div.Ratio / 100.0);
                        totalFinalScore += (double)weightedScore;

                        divisionResults.Add(new
                        {
                            divisionId = div.Id,
                            average = Math.Round(monthlyAvg, 2),
                            adjusted = adjusted,
                            weighted = Math.Round((decimal)weightedScore, 2)
                        });
                    }

                    return new
                    {
                        id = first.UserId.ToString(),
                        room = first.User.Room?.Name,
                        name = first.User.FullName,
                        position = first.User.Position?.Name,
                        department = first.User.Team?.Name,
                        date = first.CreatedAt.ToString("dd.MM.yyyy"),
                        grades,
                        finalScore = Math.Round(totalFinalScore, 2),
                        divisions = divisionResults
                    };
                })
                .ToList();

            var evaluationPeriods = allDivisionNames
                 .Select(div => new
                 {
                     id = div.Id,
                     name = div.Name,
                     percentage = div.Ratio,
                     periods = Enumerable.Range(1, 12).ToList(),
                     description = "KPI Division Assessment"
                 })
                 .ToList();

            return new
            {
                students = studentWithFinals,
                evaluationPeriods
            };
        }

        public async ValueTask<List<object>> GetEvaluationScores(int year)
        {
            var evaluations = await scoreManagementService.GetAll(x => x.IsDeleted == 0 && x.CreatedAt.Year == year)
                .Include(x => x.Division)
                .ToListAsync();

            var divisionGradeStats = evaluations
                .Select(g => new
                {
                    divisionId = g.DivisionId,
                    divisionName = g.Division.Name + " " + g.Division.Ratio,
                    grade = g.Grade,
                    score = g.Score,
                    scoreId = g.Id
                })
                .Cast<object>() 
                .ToList();

            return divisionGradeStats;
        }

        public async ValueTask<List<object>> GetDivisionName(int year)
        {
            var divisions = await divisionService.GetAll(x => x.CreatedAt.Year == year && x.IsDeleted == 0).ToListAsync();

            var divisionGradeStats = divisions.Select(g => new
             {
                 Id = g.Id,
                 Name = g.Name + " "+ g.Ratio,
                 
             })
            .Cast<object>()
            .ToList();

            return divisionGradeStats;
        }

        public async ValueTask<bool> CreateScore(ScoreForCreationDTO dto)
        {
            var existScore = await scoreManagementService.GetAsync(x => x.Grade == dto.Grade && x.DivisionId == dto.DivisionId && x.CreatedAt.Year == dto.Year);

            if (existScore is not null) throw new KpiException(400, "exist_grade_this_divison");

            var grade = new Domain.Entities.ScoreManagement
            {
                DivisionId = dto.DivisionId,
                Grade = dto.Grade,
                Score = dto.Score,
                CreatedAt = DateTime.Parse($"{dto.Year}-01-01")
            };

            await scoreManagementService.CreateAsync(grade);
            await scoreManagementService.SaveChangesAsync();
            return true;
        }

        public async ValueTask<bool> UpdateScore(ScoreForUpdateDTO dto)
        {
            var existScore = await scoreManagementService.GetAsync(x => x.Id == dto.Id);

            if (existScore is null) throw new KpiException(404, "score_not_found");

            existScore.Score = dto.Score;

            scoreManagementService.UpdateAsync(existScore);
            await scoreManagementService.SaveChangesAsync();
            return true;
        }

        public async ValueTask<bool> DeleteScore(int id)
        {
            var existScore = await scoreManagementService.GetAsync(x => x.Id == id);

            if (existScore is null) throw new KpiException(404, "score_not_found");

            await scoreManagementService.DeleteAsync(existScore.Id);
            await scoreManagementService.SaveChangesAsync();
            return true;
        }
    }
}
