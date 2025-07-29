using System.ComponentModel.DataAnnotations;

namespace Kpi.Service.DTOs.Evaluation
{
    public class ScoreForUpdateDTO
    {
        [Required]
        public double MinScore { get; set; }

        [Required]
        public double MaxScore { get; set; }

        [Required]
        public int Id { get; set; }
    }
}
