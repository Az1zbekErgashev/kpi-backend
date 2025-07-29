namespace Kpi.Service.DTOs.User
{
    public class UserForUpdateByTokenDTO
    {
        public int PositionId { get; set; }
        public string FullName { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public bool UpdatePassword { get; set; } = false;
    }
}
