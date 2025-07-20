using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class AccPymtExcp
{
    public DateTime DtCreateDate { get; set; }

    public string? SClntCode { get; set; }

    public string? SCoBrchCode { get; set; }

    public string? SUsrId { get; set; }

    public string? SBillReferenceNo { get; set; }

    public decimal? DGrandTrustPymtTtl { get; set; }

    public decimal? DGrandBankPymtTtl { get; set; }

    public decimal? DContTrustPymtTtl { get; set; }

    public decimal? DContBankPymtTtl { get; set; }

    public string? SContNo { get; set; }

    public DateTime? DtContDate { get; set; }

    public int? IPymtReconInd { get; set; }

    public string? SRemarks { get; set; }
}
