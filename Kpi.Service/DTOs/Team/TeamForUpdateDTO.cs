using System.ComponentModel.DataAnnotations;

namespace Kpi.Service.DTOs.Team
{
    public class TeamForUpdateDTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int Id { get; set; }
    }
}
