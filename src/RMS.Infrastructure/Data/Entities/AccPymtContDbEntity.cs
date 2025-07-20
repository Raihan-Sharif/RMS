using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class AccPymtContDbEntity
{
    public DateTime DtCreateDate { get; set; }

    public string SPymtRefNo { get; set; } = null!;

    public string SClientCode { get; set; } = null!;

    public string SCoBrchCode { get; set; } = null!;

    public string SContNo { get; set; } = null!;

    public DateTime DtContDate { get; set; }

    public string? SContType { get; set; }

    public decimal? DPymtAmtByTrust { get; set; }

    public decimal? DPymtAmt { get; set; }

    public string? SPymtBosstatus { get; set; }

    public string? SPymtProcInd { get; set; }

    public DateTime DtLastUpdateDate { get; set; }
}
