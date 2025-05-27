using System.ComponentModel.DataAnnotations;

namespace Kpi.Service.DTOs.Room
{
    public class RoomForUpdateDTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int Id { get; set; }
    }
}
