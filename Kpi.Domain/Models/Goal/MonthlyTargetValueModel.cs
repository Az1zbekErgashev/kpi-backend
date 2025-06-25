using Kpi.Domain.Commons;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kpi.Domain.Models.Goal
{
    public class MonthlyTargetValueModel : Auditable
    {
        public double? ValueRatio { get; set; }
        public double? ValueRatioStatus { get; set; }
        public double? ValueNumber { get; set; }
        public string? ValueText { get; set; }
        public int TargetValueId { get; set; }
        public int MonthlyPerformanceId { get; set; }

        public virtual MonthlyTargetValueModel MapFromEntity(Domain.Entities.Goal.MonthlyTargetValue entity)
        {
            ValueNumber = entity.ValueNumber;
            ValueRatioStatus = entity.ValueRatioStatus;
            ValueNumber = entity.ValueNumber;
            ValueText = entity.ValueText;
            TargetValueId = entity.TargetValueId;
            MonthlyPerformanceId = entity.MonthlyPerformanceId;
            return this;
        }
    }
}
