using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class UsrKentradePlusMonthlyReportBak20240628
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

    public int MthSubPer { get; set; }

    public DateTime LastUpdatedDate { get; set; }
}
