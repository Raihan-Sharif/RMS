using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class AccContDtlHistory
{
    public DateTime DtCreateDate { get; set; }

    public string SClientCode { get; set; } = null!;

    public string SCoBrchCode { get; set; } = null!;

    public string SContType { get; set; } = null!;

    public string SContNo { get; set; } = null!;

    public DateTime DtContDate { get; set; }

    public DateTime? DtSettleDate { get; set; }

    public string? SPymtStatus { get; set; }

    public string? SStkCode { get; set; }

    public decimal? DUnitPrice { get; set; }

    public decimal? DContQty { get; set; }

    public decimal? DContAmt { get; set; }

    public decimal? DInterestAmt { get; set; }

    public decimal? DDffee { get; set; }

    public decimal? DOsqty { get; set; }

    public decimal DOsamt { get; set; }

    public byte? IProcInd { get; set; }

    public string? STag { get; set; }

    public DateTime? DtLastUpdatedDate { get; set; }

    public DateTime? DtRollOverDate { get; set; }
}
