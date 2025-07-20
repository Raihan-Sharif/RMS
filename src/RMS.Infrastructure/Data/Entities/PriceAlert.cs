using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class PriceAlert
{
    public string UsrId { get; set; } = null!;

    public string ClntCode { get; set; } = null!;

    public int AlertId { get; set; }

    public string StkCode { get; set; } = null!;

    public string StkSname { get; set; } = null!;

    public string ConditionType { get; set; } = null!;

    public decimal ConditionValue { get; set; }

    public string AlertStatus { get; set; } = null!;

    public DateTime? LastTriggerTime { get; set; }
}
