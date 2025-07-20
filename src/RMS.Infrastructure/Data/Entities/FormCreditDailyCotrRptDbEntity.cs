using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class FormCreditDailyCotrRptDbEntity
{
    public string FormRefNo { get; set; } = null!;

    public DateTime TrnxDate { get; set; }

    public int SeqNo { get; set; }

    public string? CoBrchCode { get; set; }

    public string? ClntCode { get; set; }

    public string? CotrNo { get; set; }

    public decimal? TasetOffAmt { get; set; }

    public decimal? CasetOffAmt { get; set; }

    public decimal? SdsetOffAmt { get; set; }

    public string? Remarks { get; set; }

    public int? TrxnAge { get; set; }
}
