using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ShareConsolidationReconDbEntity
{
    public DateTime TimeStamp { get; set; }

    public string LogUsr { get; set; } = null!;

    public string StkCode { get; set; } = null!;

    public string StkSname { get; set; } = null!;

    public string CorpExerType { get; set; } = null!;

    public string Ratio { get; set; } = null!;

    public int CorpExerSeq { get; set; }

    public DateTime ExDate { get; set; }

    public DateTime BookClsDtEntDt { get; set; }

    public DateTime? ListDate { get; set; }

    public string NewStkCode { get; set; } = null!;

    public string NewStkSname { get; set; } = null!;

    public string DepositorNm { get; set; } = null!;

    public string? AccQualifier1 { get; set; }

    public string? AccQualifier2 { get; set; }

    public string DepositorNric { get; set; } = null!;

    public string DepositorCdsno { get; set; } = null!;

    public long FreeBal { get; set; }

    public long NetTradeExDt2 { get; set; }

    public long NetTradeExDt1 { get; set; }

    public long IndicativeBal { get; set; }
}
