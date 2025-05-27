using System.ComponentModel.DataAnnotations;

namespace Kpi.Service.DTOs.Team
{
    public class TeamForCreateDTO
    {
        [Required]
        public string Name { get; set; }
    }
}
