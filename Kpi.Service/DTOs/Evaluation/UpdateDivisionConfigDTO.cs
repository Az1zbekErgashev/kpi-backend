namespace Kpi.Service.DTOs.Evaluation
{
    public class UpdateDivisionConfigDTO
    {
        public int DivisionConfigId { get; set; }
        public decimal WeightPercent { get; set; }
        public bool IsIncludedInMainScore { get; set; }
        public List<UpdateGradeRuleDTO> GradeRules { get; set; }
    }
}
