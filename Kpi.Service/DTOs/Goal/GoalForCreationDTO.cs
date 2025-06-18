using System.ComponentModel.DataAnnotations;

namespace Kpi.Service.DTOs.Goal
{
    public class GoalForCreationDTO
    {
        [Required]
        public List<DivisionForCreateDto> Divisions { get; set; }
        public string? Comment { get; set; }
        public int? GoalId { get; set; }
        public DateTime? CreatetAt { get; set; }
    }
}
