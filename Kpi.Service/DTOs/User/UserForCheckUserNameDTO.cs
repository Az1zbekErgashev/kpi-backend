using System.ComponentModel.DataAnnotations;

namespace Kpi.Service.DTOs.User
{
    public class UserForCheckUserNameDTO
    {
        [Required]
        public string UserName { get; set; }
    }
}
