﻿using Kpi.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace Kpi.Service.DTOs.User
{
    public class UserForUpdateDTO
    {
        [Required]
        public int Id { get; set; }
        public string? Password { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public Role Role { get; set; }
        public int? TeamId { get; set; }

        [Required]
        public int RoomId { get; set; }

        [Required]
        public string UserName { get; set; }
        public int? PositionId { get; set; }
    }
}
