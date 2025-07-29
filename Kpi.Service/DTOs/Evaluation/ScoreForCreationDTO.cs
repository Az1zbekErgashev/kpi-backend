using System.ComponentModel.DataAnnotations;

namespace Kpi.Service.DTOs.Evaluation
{
    public class ScoreForCreationDTO
    {
        [Required]
        public double MinScore { get; set; }

        [Required]
        public double MaxScore { get; set; }

        [Required]
        public string Grade { get; set; }

        public int DivisionId { get; set; }

        [Required]
        public int Year { get; set; }

        public bool IsFinalScore { get; set; } = false;
        public bool IsMoreDivisions { get; set; } = false;
        public int[]? Divisions { get; set; } = [];
    }
}
