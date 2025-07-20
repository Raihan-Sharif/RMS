using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class AccPymtRefNoDbEntity
{
    public DateTime? DtCreateDate { get; set; }

    public string SPymtRefNo { get; set; } = null!;

    public DateTime DtPymtDate { get; set; }

    public string SUsrId { get; set; } = null!;

    public string SClntCode { get; set; } = null!;

    public string SCoBrchCode { get; set; } = null!;

    public decimal DPymtAmtByTrust { get; set; }

    public decimal DPymtAmt { get; set; }

    public string SPymtStatus { get; set; } = null!;

    public string? SPymtBank { get; set; }

    public string? SMerchantId { get; set; }

    public string? SPymtAccNo { get; set; }

    public string? SChannelId { get; set; }

    public decimal? DRetAmt { get; set; }

    public string? SRetBankRef { get; set; }

    public DateTime? DtRetDate { get; set; }

    public string? SRetUsrName { get; set; }

    public string? SReturnCode { get; set; }

    public string? SReturnMsg { get; set; }

    public byte? IPymtReconInd { get; set; }

    public DateTime DtLastUpdateDate { get; set; }
}
