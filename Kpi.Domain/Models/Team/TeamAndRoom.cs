namespace Kpi.Domain.Models.Team
{
    public class TeamAndRoom
    {
        public int? TeamId { get; set; }
        public string? Team { get; set; }
        public string? Room { get; set; }
        public int? RoomId { get; set; }

        public virtual TeamAndRoom MapFromEntity(int? teamId, int? roomId, string? teamName, string? roomName)
        {
            Team = teamName;
            Room = roomName;
            RoomId = roomId;
            TeamId = teamId;
            return this;
        }
    }
}
