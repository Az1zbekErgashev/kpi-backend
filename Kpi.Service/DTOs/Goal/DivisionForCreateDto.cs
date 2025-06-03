using System.ComponentModel.DataAnnotations;

namespace Kpi.Service.DTOs.Goal
{
    public class DivisionForCreateDto
    {
        public int? Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public List<KpiGoalForCreateDto> Goals { get; set; }
    }
}
