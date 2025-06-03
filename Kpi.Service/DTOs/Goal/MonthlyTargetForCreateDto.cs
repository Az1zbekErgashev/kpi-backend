namespace Kpi.Service.DTOs.Goal
{
    public class MonthlyTargetForCreateDto
    {
        public int Id { get; set; }
        public int Month { get; set; }
        public string TargetValueText { get; set; }
        public double? TargetValueNumber { get; set; }
        public double? TargetValueRatio { get; set; }
    }
}
