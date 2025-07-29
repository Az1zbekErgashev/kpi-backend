using System.ComponentModel.DataAnnotations;

namespace Kpi.Service.DTOs.Evaluation
{
    public class ScoreForCreationDTO
    {
        [Required]
        public double Score { get; set; }

        [Required]
        public string Grade { get; set; }

        [Required]
        public int DivisionId { get; set; }

        [Required]
        public int Year { get; set; }
    }
}
