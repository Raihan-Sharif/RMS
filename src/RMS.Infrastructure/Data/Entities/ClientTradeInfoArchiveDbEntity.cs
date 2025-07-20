using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ClientTradeInfoArchiveDbEntity
{
    public int TransSeqNo { get; set; }

    public string ClientNo { get; set; } = null!;

    public int DealerId { get; set; }

    public int BranchId { get; set; }

    public int TerminalId { get; set; }

    public int OrderNo { get; set; }

    public int Trsno { get; set; }

    public string StockCode { get; set; } = null!;

    public string SettlementCode { get; set; } = null!;

    public string BuySellCode { get; set; } = null!;

    public int MatchedQty { get; set; }

    public int OrderType { get; set; }

    public decimal MatchedPrice { get; set; }

    public DateTime MatchedTime { get; set; }

    public int CounterBrokerId { get; set; }

    public int ClientCdsno { get; set; }

    public string? AmendedFrom { get; set; }

    public DateTime? AmendmentTime { get; set; }

    public byte? Processed { get; set; }
}
