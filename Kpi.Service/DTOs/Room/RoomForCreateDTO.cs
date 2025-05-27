using System.ComponentModel.DataAnnotations;

namespace Kpi.Service.DTOs.Room
{
    public class RoomForCreateDTO
    {
        [Required]
        public string Name { get; set; }
    }
}
