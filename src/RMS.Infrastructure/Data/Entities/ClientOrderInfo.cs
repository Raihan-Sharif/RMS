using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ClientOrderInfo
{
    public int TransSeqNo { get; set; }

    public string ClientNo { get; set; } = null!;

    public int DealerId { get; set; }

    public int BranchId { get; set; }

    public int TerminalId { get; set; }

    public int OrderNo { get; set; }

    public string ActionCode { get; set; } = null!;

    public string StockCode { get; set; } = null!;

    public string SettlementCode { get; set; } = null!;

    public string BuySellCode { get; set; } = null!;

    public int QuantityBefore { get; set; }

    public int OrderQuantity { get; set; }

    public int OrderType { get; set; }

    public decimal OrderPrice { get; set; }

    public DateTime OrderDateTime { get; set; }

    public string ClientCdsno { get; set; } = null!;

    public string? AmendedFrom { get; set; }

    public DateTime? AmendmentTime { get; set; }

    public DateTime? OrgOrderDateTime { get; set; }
}
