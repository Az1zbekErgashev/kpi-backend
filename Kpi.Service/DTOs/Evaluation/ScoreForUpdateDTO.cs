using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
