namespace Kpi.Service.DTOs.Evaluation
{
    public class UpdateGradeRuleDTO
    {
        public int? Id { get; set; }  
        public string GradeLabel { get; set; }
        public decimal Score { get; set; }
    }
}
