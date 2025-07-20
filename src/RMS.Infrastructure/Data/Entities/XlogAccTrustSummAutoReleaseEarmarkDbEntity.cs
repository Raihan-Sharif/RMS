using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class XlogAccTrustSummAutoReleaseEarmarkDbEntity
{
    public string? SClientCode { get; set; }

    public string? SCoBrchCode { get; set; }

    public decimal? DTrustAmt { get; set; }

    public DateTime? LogDate { get; set; }

    public decimal? OldTrustAmtEarmark { get; set; }

    public decimal? ReleaseEarmarkAmt { get; set; }

    public decimal? NewTrustAmtEarmark { get; set; }

    public string? Remark { get; set; }
}
