using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ClntStkDtlDbEntity
{
    public DateTime? CsrecDate { get; set; }

    public string CstrxnDocType { get; set; } = null!;

    public string? CstrxnRecType { get; set; }

    public string ClntCode { get; set; } = null!;

    public string StkCode { get; set; } = null!;

    public DateTime? CstrxnDate { get; set; }

    public int CstrxnDocNo { get; set; }

    public int? CstrxnQty { get; set; }

    public string? CstrxnUsr { get; set; }

    public DateTime? CstrxnCretDate { get; set; }

    public string? CstrxnRmk { get; set; }

    public string? CstrxnStat { get; set; }

    public string? CsverifyBy { get; set; }

    public DateTime? CsverifyDate { get; set; }
}
