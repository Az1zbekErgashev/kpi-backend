using System.ComponentModel.DataAnnotations;

namespace Kpi.Service.DTOs.Evaluation
{
    public class ScoreForUpdateDTO
    {
        [Required]
        public double Score { get; set; }

        [Required]
        public int Id { get; set; }
    }
}
