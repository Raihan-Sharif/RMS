using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class EcmEfMrgnCallDbEntity
{
    public DateTime? DtCreateDate { get; set; }

    public string? SCoBrchCode { get; set; }

    public string? ClientNo { get; set; }

    public string? TdrCode { get; set; }

    public DateTime? ReportingDate { get; set; }

    public DateTime? RectifyDate { get; set; }

    public decimal? MarginLimit { get; set; }

    public decimal? AdjBal { get; set; }

    public decimal? ReqValue { get; set; }

    public decimal? MarginCallPercent { get; set; }

    public decimal? EquityAmt { get; set; }

    public decimal? Shortfall { get; set; }

    public decimal? ConcentratedCurMargin { get; set; }

    public decimal? ConcentratedAvailableLimit { get; set; }

    public decimal? ConcentratedShares { get; set; }

    public decimal? Amount1 { get; set; }
}
