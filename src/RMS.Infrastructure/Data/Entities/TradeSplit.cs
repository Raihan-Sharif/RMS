using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class TradeSplit
{
    public long TokenNo { get; set; }

    public int OriginateSeqNo { get; set; }

    public string OriginateClientCode { get; set; } = null!;

    public string ReceivedClientCode { get; set; } = null!;

    public int LotSize { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public string OrderSource { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string ReceivedBranchCode { get; set; } = null!;

    public string? ApprovalAdminId { get; set; }

    public DateTime? ApprovalTimestamp { get; set; }

    public string? Validation { get; set; }

    public string LastStatus { get; set; } = null!;

    public string Remarks { get; set; } = null!;

    public DateTime SplitTimestamp { get; set; }
}
