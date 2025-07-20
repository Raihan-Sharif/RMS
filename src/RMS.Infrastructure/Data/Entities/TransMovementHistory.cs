using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class TransMovementHistory
{
    public string ClientCode { get; set; } = null!;

    public string CoBrchCode { get; set; } = null!;

    public string? ClntDlrCode { get; set; }

    public string PrinTrxNo { get; set; } = null!;

    public string TrxRefNo { get; set; } = null!;

    public string? TrxDesc { get; set; }

    public int? LstSeqNo { get; set; }

    public int MvmSeqNo { get; set; }

    public string? TrxRefType { get; set; }

    public string? PrinTrxType { get; set; }

    public string? TrxDate { get; set; }

    public DateTime TrxDateTime { get; set; }

    public decimal? Price { get; set; }

    public int? Quantity { get; set; }

    public string Currency { get; set; } = null!;

    public decimal? ExchangeRate { get; set; }

    public decimal? ForeignAmt { get; set; }

    public decimal? Debit { get; set; }

    public decimal? Credit { get; set; }

    public decimal? OverdueInterest { get; set; }

    public DateTime LastUpdateDate { get; set; }
}
