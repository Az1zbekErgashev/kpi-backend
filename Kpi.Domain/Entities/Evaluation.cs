﻿using Kpi.Domain.Commons;
using Kpi.Domain.Enum;
namespace Kpi.Domain.Entities
{
    public class Evaluation : Auditable
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int UserId { get; set; }
        public virtual User.User User { get; set; }
        public string? Comment { get; set; }
        public int KpiDivisionId { get; set; }
        public Goal.Division KpiDivision { get; set; }
        public int? ScoreManagementId { get; set; }
        public ScoreManagement? ScoreManagement { get; set; }
        public GoalStatus Status { get; set; }
    }
}
