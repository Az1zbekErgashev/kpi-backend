﻿using DocumentFormat.OpenXml.Bibliography;
using Kpi.Domain.Entities;
using Kpi.Domain.Enum;
using Kpi.Infrastructure.Contexts;
using Kpi.Service.DTOs.Evaluation;
using Kpi.Service.Exception;
using Kpi.Service.Interfaces.Evaluation;
using Kpi.Service.Interfaces.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.Record.Aggregates;
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
                        ScoreManagementId = score?.Id,
                        Status = GoalStatus.PendingReview
                    };

                    await _context.Evaluations.AddAsync(entity);
                }
                else
                {
                    entity.Comment = dto.Comment;
                    entity.UpdatedAt = DateTime.UtcNow;
                    entity.ScoreManagementId = score?.Id;
                    entity.Status = GoalStatus.PendingReview;
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
                x.CreatedAt.Year == dto.Year &&
                x.CreatedBy.Role == Role.Ceo)
                .Include(x => x.Divisions)
                .FirstOrDefaultAsync();

            if (goalWithDivisions == null)
                throw new KpiException(404, "Goal with divisions not found.");

            var allDivisions = goalWithDivisions.Divisions.ToList();

            var employees = await _context.Users
                    .Where(e => e.TeamId == teamId)
                    .Include(x => x.Position)
                    .ToListAsync();

            var employeeIds = employees?.Select(e => e.Id).ToList();

            var evaluations = await _context.Evaluations
                .Where(e => employeeIds.Contains(e.UserId)
                            && e.Year == dto.Year
                            && e.Month == dto.Month
                                )
                .Include(e => e.KpiDivision)
                .Include(x => x.ScoreManagement)
                .ToListAsync();

            var result = employees?.Select(emp =>
            {
                var employeeEvals = evaluations
                    ?.Where(e => e.UserId == emp.Id)
                    ?.ToList();

                var divisionEvaluations = allDivisions?.Select(division =>
                {
                    var eval = employeeEvals?.FirstOrDefault(e => e.KpiDivisionId == division.Id);

                    return new DivisionEvaluationDto
                    {
                        KpiDivisionId = division.Id,
                        DivisionName = division.Name,
                        Ratio = division.Ratio,
                        Grade = eval?.ScoreManagement?.Grade,
                        ScoreId = eval?.ScoreManagement?.Id,
                        Comment = eval?.Comment,
                        Id = eval?.Id,
                    };
                }).ToList();

                return new TeamEvaluationResultDto
                {
                    Role = emp.Role,
                    EmployeeId = emp.Id,
                    Position = emp?.Position?.Name,
                    FullName = emp.FullName,
                    DivisionEvaluations = divisionEvaluations,
                };
            })?.ToList();

            return result;
        }


        public async Task<IEnumerable<TeamEvaluationResultDto>> GetUserEvaluationsAsync(EvaluationsForFilterDTO dto)
        {
            var user = httpContextAccessor?.HttpContext?.User
             ?? throw new InvalidCredentialException();

            if (!int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ||
                !int.TryParse(user.FindFirstValue(ClaimTypes.Country), out var teamIdByToken) ||
                !Enum.TryParse<Role>(user.FindFirstValue(ClaimTypes.Role), ignoreCase: true, out var role))
            {
                throw new InvalidCredentialException("Invalid token claims.");
            }

            var teamId = teamIdByToken;

            var goalWithDivisions = await goalService.GetAll(x =>
                x.CreatedAt.Year == dto.Year &&
                x.CreatedBy.Role == Role.Ceo)
                .Include(x => x.Divisions)
                .FirstOrDefaultAsync();

            if (goalWithDivisions == null)
                throw new KpiException(404, "Goal with divisions not found.");

            var allDivisions = goalWithDivisions.Divisions.ToList();

            var employees = await _context.Users
                    .Where(e => e.TeamId == teamId && e.Id == dto.UserId)
                    .Include(x => x.Position)
                    .ToListAsync();

            if (employees == null)
                throw new KpiException(404, "employee_not_found");

            var employeeIds = employees?.Select(e => e.Id).ToList();

            var evaluations = await _context.Evaluations
                .Where(e => employeeIds.Contains(e.UserId)
                            && e.Year == dto.Year
                            && e.Month == dto.Month
                           )
                .Include(e => e.KpiDivision)
                .Include(x => x.ScoreManagement)
                .ToListAsync();

            var result = employees?.Select(emp =>
            {
                var employeeEvals = evaluations
                    ?.Where(e => e.UserId == emp.Id)
                    ?.ToList();

                var divisionEvaluations = allDivisions?.Select(division =>
                {
                    var eval = employeeEvals?.FirstOrDefault(e => e.KpiDivisionId == division.Id);

                    return new DivisionEvaluationDto
                    {
                        KpiDivisionId = division.Id,
                        DivisionName = division.Name,
                        Ratio = division.Ratio,
                        Grade = eval?.ScoreManagement?.Grade,
                        ScoreId = eval?.ScoreManagement?.Id,
                        Comment = eval?.Comment,
                        Id = eval?.Id,
                    };
                }).ToList();

                return new TeamEvaluationResultDto
                {
                    Role = emp.Role,
                    EmployeeId = emp.Id,
                    Position = emp?.Position?.Name,
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

            var divisions = await goalService.GetAll(x => x.CreatedAt.Year == year && x.CreatedBy.Role == Role.Ceo).Include(x => x.Divisions).FirstOrDefaultAsync();

            if (divisions is null) throw new KpiException(404, "goal_not_found");

            var evaluations = await evaluationService.GetAll(x => x.Year == year && x.User.TeamId != null && x.User.RoomId != null)
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

            var complexScores = await scoreManagementService.GetAll(x => x.IsMoreDivisions).ToListAsync();

            var finalScore = await scoreManagementService.GetAll(x => x.IsFinalScore).ToListAsync();

            var studentWithFinals = await Task.WhenAll(evaluations
          .GroupBy(e => e.UserId)
          .Select(async group =>
          {
              var first = group.First();

              var grades = allDivisionNames.ToDictionary(
               div => div.Id,
               div => Enumerable.Range(1, 12).ToDictionary(
                   month => month.ToString(),
                   month =>
                   {
                       var match = group.FirstOrDefault(e =>
                           e.KpiDivisionId == div.Id &&
                           e.Month == month);
                       return match?.ScoreManagement?.Grade?.ToString();
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
                           e.Month == month)?.ScoreManagement?.MaxScore)
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

              var addedComplexIds = new HashSet<string>();
              foreach (var complexScore in complexScores)
              {
                  var relatedDivisionIds = complexScore?.Divisions ?? new int[0];
                  var complexKey = string.Join(", ", relatedDivisionIds);
                  if (!addedComplexIds.Add(complexKey)) continue;

                  var relatedRatios = allDivisionNames
                      .Where(d => relatedDivisionIds.Contains(d.Id))
                      .ToList();

                  double percentSum = (double)relatedRatios.Sum(r => r.Ratio);

                  var relatedScores = relatedRatios.SelectMany(r =>
                      Enumerable.Range(1, 12)
                          .Select(month => group.FirstOrDefault(e =>
                              e.KpiDivisionId == r.Id && e.Month == month)?.ScoreManagement?.MaxScore)
                          .Where(score => score.HasValue)
                          .Select(score => score.Value)
                  ).ToList();

                  double avgScore = relatedScores.Any() ? relatedScores.Average() : 0;
                  double adjusted = Math.Round(avgScore);
                  double weighted = adjusted * (percentSum / 100.0);

                  var allDivisionIds = allDivisionNames.Select(d => d.Id).ToHashSet();

                  var gradeForManyDivisions = complexScores
                      .Where(x => x.Divisions.Any(divId => allDivisionIds.Contains(divId)))
                      .ToList();

                  var newGrade = gradeForManyDivisions.FirstOrDefault(e => weighted >= e.MinScore && weighted <= e.MaxScore);

                  var divisionName = string.Join(", ", relatedRatios.Select(x => $"{x.Name} ({x.Ratio})"));

                  divisionResults.Add(new
                  {
                      divisionId = divisionName,
                      average = Math.Round(avgScore, 2),
                      adjusted = adjusted,
                      weighted = Math.Round((decimal)weighted, 2),
                      ratio = percentSum,
                      grade = newGrade?.Grade ?? "-"
                  });
              }

              var finalGradeScore = finalScore.FirstOrDefault(e =>
                  totalFinalScore >= e.MinScore &&
                  totalFinalScore <= e.MaxScore);


              string finalGrade = finalGradeScore?.Grade ?? "-";

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
                  finalGrade,
                  divisions = divisionResults
              };
          })
          .ToList());

            var evaluationPeriods = allDivisionNames
                 .Select(div => new
                 {
                     id = div.Id,
                     name = div.Name,
                     percentage = div.Ratio,
                     periods = Enumerable.Range(1, 12).ToList(),
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

            var divisions = await goalService.GetAll(x => x.CreatedAt.Year == year && x.CreatedBy.Role == Role.Ceo).Include(x => x.Divisions).FirstOrDefaultAsync();

            if (divisions is null) throw new KpiException(404, "goal_not_found");

            var evaluations = await evaluationService.GetAll(x => x.Year == year && x.User.TeamId == teamId)
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

            var complexScores = await scoreManagementService.GetAll(x => x.IsMoreDivisions).ToListAsync();

            var finalScore = await scoreManagementService.GetAll(x => x.IsFinalScore).ToListAsync();

            var studentWithFinals = await Task.WhenAll(evaluations
            .GroupBy(e => e.UserId)
            .Select(async group =>
            {
                var first = group.First();

                var grades = allDivisionNames.ToDictionary(
                 div => div.Id,
                 div => Enumerable.Range(1, 12).ToDictionary(
                     month => month.ToString(),
                     month =>
                     {
                         var match = group.FirstOrDefault(e =>
                             e.KpiDivisionId == div.Id &&
                             e.Month == month);
                         return match?.ScoreManagement?.Grade?.ToString();
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
                             e.Month == month)?.ScoreManagement?.MaxScore)
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

                var addedComplexIds = new HashSet<string>();
                foreach (var complexScore in complexScores)
                {
                    var relatedDivisionIds = complexScore?.Divisions ?? new int[0];
                    var complexKey = string.Join("_", relatedDivisionIds);
                    if (!addedComplexIds.Add(complexKey)) continue;

                    var relatedRatios = allDivisionNames
                        .Where(d => relatedDivisionIds.Contains(d.Id))
                        .ToList();

                    double percentSum = (double)relatedRatios.Sum(r => r.Ratio);

                    var relatedScores = relatedRatios.SelectMany(r =>
                        Enumerable.Range(1, 12)
                            .Select(month => group.FirstOrDefault(e =>
                                e.KpiDivisionId == r.Id && e.Month == month)?.ScoreManagement?.MaxScore)
                            .Where(score => score.HasValue)
                            .Select(score => score.Value)
                    ).ToList();

                    double avgScore = relatedScores.Any() ? relatedScores.Average() : 0;
                    double adjusted = Math.Round(avgScore);
                    double weighted = adjusted * (percentSum / 100.0);

                    var divisionName = string.Join(", ", relatedRatios.Select(x => $"{x.Name} ({x.Ratio})"));

                    var allDivisionIds = allDivisionNames.Select(d => d.Id).ToHashSet();

                    var gradeForManyDivisions = complexScores
                        .Where(x => x.Divisions.Any(divId => allDivisionIds.Contains(divId)))
                        .ToList();

                    var newGrade = gradeForManyDivisions.FirstOrDefault(e => weighted >= e.MinScore && weighted <= e.MaxScore);

                    divisionResults.Add(new
                    {
                        divisionId = divisionName,
                        average = Math.Round(avgScore, 2),
                        adjusted = adjusted,
                        weighted = Math.Round((decimal)weighted, 2),
                        ratio = percentSum,
                        grade = newGrade?.Grade ?? "-"
                    });
                }

                var finalGradeScore = finalScore.FirstOrDefault(e =>
                    totalFinalScore >= e.MinScore &&
                    totalFinalScore <= e.MaxScore);


                string finalGrade = finalGradeScore?.Grade ?? "-";

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
                    finalGrade,
                    divisions = divisionResults
                };
            })
            .ToList());

            var evaluationPeriods = allDivisionNames
                 .Select(div => new
                 {
                     id = div.Id,
                     name = div.Name,
                     percentage = div.Ratio,
                     periods = Enumerable.Range(1, 12).ToList(),
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

            var divisions = await goalService.GetAll(x => x.CreatedAt.Year == year && x.CreatedBy.Role == Role.Ceo)
                .Include(x => x.Divisions)
                .FirstOrDefaultAsync();

            if (divisions is null) throw new KpiException(404, "goal_not_found");

            var evaluations = await evaluationService.GetAll(x => x.Year == year && x.User.TeamId != null && x.User.RoomId != null && x.Status == GoalStatus.Approved)
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

            var complexScores = await scoreManagementService.GetAll(x => x.IsMoreDivisions).ToListAsync();

            var finalScore = await scoreManagementService.GetAll(x => x.IsFinalScore).ToListAsync();

            var studentWithFinals = await Task.WhenAll(evaluations
            .GroupBy(e => e.UserId)
            .Select(async group =>
            {
                var first = group.First();

                var grades = allDivisionNames.ToDictionary(
                 div => div.Id,
                 div => Enumerable.Range(1, 12).ToDictionary(
                     month => month.ToString(),
                     month =>
                     {
                         var match = group.FirstOrDefault(e =>
                             e.KpiDivisionId == div.Id &&
                             e.Month == month);
                         return match?.ScoreManagement?.Grade?.ToString();
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
                             e.Month == month)?.ScoreManagement?.MaxScore)
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

                var addedComplexIds = new HashSet<string>();
                foreach (var complexScore in complexScores)
                {
                    var relatedDivisionIds = complexScore?.Divisions ?? new int[0];
                    var complexKey = string.Join("_", relatedDivisionIds);
                    if (!addedComplexIds.Add(complexKey)) continue;

                    var relatedRatios = allDivisionNames
                        .Where(d => relatedDivisionIds.Contains(d.Id))
                        .ToList();

                    double percentSum = (double)relatedRatios.Sum(r => r.Ratio);

                    var relatedScores = relatedRatios.SelectMany(r =>
                        Enumerable.Range(1, 12)
                            .Select(month => group.FirstOrDefault(e =>
                                e.KpiDivisionId == r.Id && e.Month == month)?.ScoreManagement?.MaxScore)
                            .Where(score => score.HasValue)
                            .Select(score => score.Value)
                    ).ToList();

                    double avgScore = relatedScores.Any() ? relatedScores.Average() : 0;
                    double adjusted = Math.Round(avgScore);
                    double weighted = adjusted * (percentSum / 100.0);

                    var allDivisionIds = allDivisionNames.Select(d => d.Id).ToHashSet();

                    var gradeForManyDivisions = complexScores
                        .Where(x => x.Divisions.Any(divId => allDivisionIds.Contains(divId)))
                        .ToList();

                    var newGrade = gradeForManyDivisions.FirstOrDefault(e => weighted >= e.MinScore && weighted<= e.MaxScore);

                    var divisionName = string.Join(", ", relatedRatios.Select(x => $"{x.Name} ({x.Ratio})"));

                    divisionResults.Add(new
                    {
                        divisionId = divisionName,
                        average = Math.Round(avgScore, 2),
                        adjusted = adjusted,
                        weighted = Math.Round((decimal)weighted, 2),
                        ratio = percentSum,
                        grade = newGrade?.Grade ?? "-"
                    });
                }

                var finalGradeScore = finalScore.FirstOrDefault(e =>
                    totalFinalScore >= e.MinScore &&
                    totalFinalScore <= e.MaxScore);


                string finalGrade = finalGradeScore?.Grade ?? "-";

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
                    finalGrade,
                    divisions = divisionResults
                };
            })
            .ToList()); 

            var evaluationPeriods = allDivisionNames
                 .Select(div => new
                 {
                     id = div.Id,
                     name = div.Name,
                     percentage = div.Ratio,
                     periods = Enumerable.Range(1, 12).ToList(),
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
            var evaluations = await scoreManagementService.GetAll(x => x.CreatedAt.Year == year)
                .Include(x => x.Division)
                .ToListAsync();

            var divisionGradeStats = evaluations
                .Select(g => new
                {
                    divisionId = g.DivisionId,
                    divisionName = g?.Division?.Name + " " + g?.Division?.Ratio,
                    grade = g.Grade,
                    score = g.Id,
                    scoreId = g.Id
                })
                .Cast<object>()
                .ToList();

            return divisionGradeStats;
        }

        public async ValueTask<List<object>> GetEvaluationScoreManagement(int year)
        {
            // 1. Получаем все оценки
            var evaluations = await scoreManagementService.GetAll(x => x.CreatedAt.Year == year)
                .Include(x => x.Division)
                .ToListAsync();

            // 2. Один вызов к goalService
            var goal = await goalService.GetAll(x => x.CreatedAt.Year == year && x.CreatedBy.Role == Role.Ceo)
                .Include(x => x.Divisions)
                .FirstOrDefaultAsync();

            var allDivisionNames = goal?.Divisions?
                .Where(d => !string.IsNullOrWhiteSpace(d.Name))
                .Select(d => new DivisionInfo
                {
                    Id = d.Id,
                    Name = d.Name,
                    Ratio = d.Ratio
                })
                .ToList() ?? new List<DivisionInfo>();

            // 3. Без асинхронности внутри Select
            var result = evaluations.OrderBy(x => x.Id).Select(g =>
            {
                var divisionName = GetDivisionName(g, allDivisionNames);
                return new
                {
                    divisionId = g.DivisionId,
                    divisionName = divisionName,
                    grade = g.Grade,
                    maxScore = g.MaxScore,
                    minScore = g.MinScore,
                    scoreId = g.Id,
                    divisions = g.Divisions,
                    isFinalScore = g.IsFinalScore,
                    isMoreDivisions = g.IsMoreDivisions
                } as object;
            }).ToList();

            return result;
        }

        private string GetDivisionName(ScoreManagement entity, List<DivisionInfo> allDivisionNames)
        {
            if (entity.IsMoreDivisions)
            {
                var relatedRatios = allDivisionNames
                    .Where(d => entity.Divisions.Contains(d.Id))
                    .ToList();

                return string.Join(", ", relatedRatios.Select(x => $"{x.Name} ({x.Ratio})"));
            }
            else if (entity.IsFinalScore)
            {
                return "Final Result (100%)";
            }
            else
            {
                return $"{entity.Division.Name} {entity.Division.Ratio}";
            }
        }

        public class DivisionInfo
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public double? Ratio { get; set; }
        }

        public async ValueTask<List<object>> GetDivisionName(int year)
        {
            var goal = await goalService.GetAll(x => x.CreatedAt.Year == year && x.CreatedBy.Role == Role.Ceo).Include(x => x.Divisions).FirstOrDefaultAsync();

            if (goal is null) throw new KpiException(400, "goal_not_found");

            var divisionGradeStats = goal.Divisions.Select(g => new
            {
                Id = g.Id,
                Name = g.Name + " " + g.Ratio,

            })
            .Cast<object>()
            .ToList();

            return divisionGradeStats;
        }

        public async ValueTask<bool> CreateScore(ScoreForCreationDTO dto)
        {
            var grade = new Domain.Entities.ScoreManagement();

            if (dto.IsMoreDivisions)
            {
                if(dto?.Divisions != null && (dto?.Divisions?.Length == 0 || dto?.Divisions?.Length == 1)) throw new KpiException(400, "please_choose_division");

                foreach (var item in dto?.Divisions)
                {
                    var existScore = await scoreManagementService.GetAsync(x => x.Grade == dto.Grade && x.IsMoreDivisions && x.CreatedAt.Year == dto.Year && x.Divisions.Contains(item));
                    if (existScore is not null) throw new KpiException(400, "exist_grade_this_divison");
                }

                grade = new Domain.Entities.ScoreManagement
                {
                    Grade = dto.Grade,
                    MaxScore = dto.MaxScore,
                    MinScore = dto.MinScore,
                    IsMoreDivisions = true,
                    CreatedAt = DateTime.Parse($"{dto.Year}-01-01"),
                    Divisions = dto.Divisions
                };

            }
            else if (dto.IsFinalScore)
            {
                var existScore = await scoreManagementService.GetAsync(x => x.Grade == dto.Grade && x.IsFinalScore && x.CreatedAt.Year == dto.Year);

                if (existScore is not null) throw new KpiException(400, "exist_grade_this_divison");

                grade = new Domain.Entities.ScoreManagement
                {
                    Grade = dto.Grade,
                    MaxScore = dto.MaxScore,
                    MinScore = dto.MinScore,
                    IsFinalScore = true,
                    CreatedAt = DateTime.Parse($"{dto.Year}-01-01")
                };
            }
            else
            {
                var existScore = await scoreManagementService.GetAsync(x => x.Grade == dto.Grade && x.DivisionId == dto.DivisionId && x.CreatedAt.Year == dto.Year);

                if (existScore is not null) throw new KpiException(400, "exist_grade_this_divison");

                grade = new Domain.Entities.ScoreManagement
                {
                    DivisionId = dto.DivisionId,
                    Grade = dto.Grade,
                    MaxScore = dto.MaxScore,
                    MinScore = dto.MinScore,
                    CreatedAt = DateTime.Parse($"{dto.Year}-01-01")
                };
            }
                

            await scoreManagementService.CreateAsync(grade);
            await scoreManagementService.SaveChangesAsync();
            return true;
        }

        public async ValueTask<bool> UpdateScore(ScoreForUpdateDTO dto)
        {
            var existScore = await scoreManagementService.GetAsync(x => x.Id == dto.Id);

            if (existScore is null) throw new KpiException(404, "score_not_found");

            existScore.MaxScore = dto.MaxScore;
            existScore.MinScore = dto.MinScore;

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
