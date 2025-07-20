using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class AccPymtContHistoryDbEntity
{
    public DateTime DtCreateDate { get; set; }

    public string SPymtRefNo { get; set; } = null!;

    public string SClientCode { get; set; } = null!;

    public string SCoBrchCode { get; set; } = null!;

    public string SContNo { get; set; } = null!;

    public DateTime DtContDate { get; set; }

    public decimal? DPymtAmtByTrust { get; set; }

    public decimal? DPymtAmt { get; set; }

    public string? SPymtBosstatus { get; set; }
}
