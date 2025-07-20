using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class Csprofile
{
    public string ClntCode { get; set; } = null!;

    public string? Race { get; set; }

    public string? Location { get; set; }

    public string? Country { get; set; }

    public string? Occupation { get; set; }

    public string? AnnualIncome { get; set; }

    public string? OfflineBrkRateCode { get; set; }

    public string? OnlineBrkRateCode { get; set; }

    public int? TotalCdsshareHldg { get; set; }

    public decimal? TotalTrustAcc { get; set; }

    public decimal? TotalGrossTrds { get; set; }

    public decimal? TotalOfflineBrk { get; set; }

    public decimal? TotalOnlineBrk { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
