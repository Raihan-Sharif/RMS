using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class XlogEpAccountSummaryTrust202403011751DbEntity
{
    public int ISeqNo { get; set; }

    public DateTime DtLogDate { get; set; }

    public string SLogAction { get; set; } = null!;

    public string? SLogUsr { get; set; }

    public string SClientCode { get; set; } = null!;

    public string SCoBrchCode { get; set; } = null!;

    public string SCurrency { get; set; } = null!;

    public string? SPaymentRefNo { get; set; }

    public string? SContractNo { get; set; }

    public DateTime? DtContractDate { get; set; }

    public string STransType { get; set; } = null!;

    public decimal? DB4trustAccount { get; set; }

    public decimal? DB4trustAmtEarmark { get; set; }

    public decimal? DB4availableTrustAmt { get; set; }

    public decimal? DB4availableTrustAmtBalance { get; set; }

    public decimal? DUpdateTrustAmtEarmark { get; set; }

    public decimal? DUpdateAvailableTrustAmt { get; set; }

    public decimal? DAfterTrustAccount { get; set; }

    public decimal? DAfterTrustAmtEarmark { get; set; }

    public decimal? DAfterAvailableTrustAmt { get; set; }

    public decimal? DAfterAvailableTrustAmtBalance { get; set; }

    public string? SRemark { get; set; }

    public string? SProcName { get; set; }
}
