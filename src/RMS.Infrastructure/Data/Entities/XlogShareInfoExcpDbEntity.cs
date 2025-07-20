using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class XlogShareInfoExcpDbEntity
{
    public int SeqNo { get; set; }

    public DateTime LogDate { get; set; }

    public string BranchId { get; set; } = null!;

    public string? Cdsno { get; set; }

    public string ClntCode { get; set; } = null!;

    public string? StkCode { get; set; }

    public string? StkSname { get; set; }

    public int? BtxbalanceQty { get; set; }

    public int? BackOfficeBalanceQty { get; set; }

    public int? BalanceDiffQty { get; set; }

    public string? Remarks { get; set; }

    public string? XchgCode { get; set; }
}
