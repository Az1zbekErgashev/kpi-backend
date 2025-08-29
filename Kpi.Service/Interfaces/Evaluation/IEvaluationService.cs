using Kpi.Service.DTOs.Evaluation;

namespace Kpi.Service.Interfaces.Evaluation
{
    public interface IEvaluationService
    {
        ValueTask<IEnumerable<EmployeeEvaluationFroCreateDTO>> CreateOrUpdateAsync(List<EmployeeEvaluationFroCreateDTO> dtos);
        Task<IEnumerable<TeamEvaluationResultDto>> GetTeamEvaluationsAsync(EvaluationsForFilterDTO dto);
        Task<IEnumerable<TeamEvaluationResultDto>> GetTeamLeaderEvaluationsAsync(EvaluationsForFilterDTO dto);
        ValueTask<object> GetAllEvaluationByYearForTeam(int year);
        ValueTask<object> GetEvaluationByYearForTeam(int year);
        Task<IEnumerable<TeamEvaluationResultDto>> GetUserEvaluationsAsync(EvaluationsForFilterDTO dto);
        ValueTask<object> GetAllEvaluation(int year);
        ValueTask<object> GetAllEvaluationByTeam(int year, int team);
        ValueTask<object> GetAllEvaluationByCheckTeam(int year, int teamId);
        ValueTask<object> GetAllEvaluationByYear(int year);
        ValueTask<List<object>> GetEvaluationScores(int year);
        ValueTask<List<object>> GetEvaluationScoreManagement(int year);
        ValueTask<List<object>> GetDivisionName(int year);
        ValueTask<bool> CreateScore(ScoreForCreationDTO dto);
        ValueTask<bool> UpdateScore(ScoreForUpdateDTO dto);
        ValueTask<bool> DeleteScore(int id);

    }
}
