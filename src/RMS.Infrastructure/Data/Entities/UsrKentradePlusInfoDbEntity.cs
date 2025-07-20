using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class UsrKentradePlusInfoDbEntity
{
    public string UsrId { get; set; } = null!;

    public string? Enabled { get; set; }

    public string? SubType { get; set; }

    public DateTime? StartDateTrial { get; set; }

    public DateTime? EndDateTrial { get; set; }

    public DateTime? StartDateMth { get; set; }

    public DateTime? EndDateMth { get; set; }

    public string? MthSubMode { get; set; }

    public string? TrialFlag { get; set; }

    public int? UsrTradeCtrl { get; set; }

    public int? UsrTrdSplitMaxQty { get; set; }

    public string SubWrnPptStat { get; set; } = null!;

    public int MthSubPer { get; set; }
}
