﻿using Kpi.Domain.Commons;
using Kpi.Domain.Enum;
using Kpi.Domain.Models.User;

namespace Kpi.Domain.Models.Goal
{
    public class MonthlyPerformanceListModel : Auditable
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public int? RoomId { get; set; }
        public int? TeamId { get; set; }
        public string? Team { get; set; }
        public string? Room { get; set; }
        public GoalStatus Status { get; set; }
        public string? Year { get; set; }
        public string Month { get; set; }
        public PositionModel? Position { get; set; }
        public bool MonthlyFinish { get; set; }
        public bool IsGoalFinish { get; set; }

        public virtual MonthlyPerformanceListModel MapFromEntity(Domain.Entities.User.User entity, string year, string month, bool monthlyFinish)
        {
            int parsedYear = !string.IsNullOrEmpty(year) ? int.Parse(year) : DateTime.UtcNow.Year;
            int parseMonth = !string.IsNullOrEmpty(month) ? int.Parse(month) : DateTime.UtcNow.Month;

            Id = entity.Id;
            UserName = entity.UserName;
            FullName = entity.FullName;
            Room = entity?.Room?.Name;
            Team = entity?.Team?.Name;
            RoomId = entity?.RoomId;
            TeamId = entity?.TeamId;
            Status = entity?.CreatedGoals?.Where(x => x.CreatedAt.Year == int.Parse(year))?.FirstOrDefault()?.MonthlyPerformance?.Where(x => x.Year == int.Parse(year) && x.Month == int.Parse(month) && x.IsSended)?.FirstOrDefault()?.Status
             ?? GoalStatus.NoWritte;
            Year = year;
            Month = month;
            Position = entity?.Position is null ? null : new PositionModel().MapFromEntity(entity.Position);
            MonthlyFinish = monthlyFinish;
            IsGoalFinish = entity?.CreatedGoals?.Where(x => x.CreatedAt.Year == int.Parse(year) && x.Status == GoalStatus.Approved)?.FirstOrDefault() != null ? true : false;
            return this;
        }
    }
}
