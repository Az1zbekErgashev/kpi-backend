using Kpi.Service.DTOs.Evaluation;

namespace Kpi.Service.Interfaces.Evaluation
{
    public interface IEvaluationService
    {
        ValueTask<IEnumerable<EmployeeEvaluationFroCreateDTO>> CreateOrUpdateAsync(List<EmployeeEvaluationFroCreateDTO> dtos);
        Task<IEnumerable<EmployeeEvaluationFroCreateDTO>> UpdateAsync(IEnumerable<EmployeeEvaluationFroCreateDTO> dtos);
        Task<IEnumerable<TeamEvaluationResultDto>> GetTeamEvaluationsAsync(EvaluationsForFilterDTO dto);
        ValueTask<List<object>> GetAllEvaluation(int year);
        ValueTask<List<object>> GetAllEvaluationByTeam(int year, int team);

    }
}
