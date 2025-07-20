using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ExecutionReport
{
    public DateTime Timestamp { get; set; }

    public string ClOrdId { get; set; } = null!;

    public string? OrigClOrdId { get; set; }

    public string? ExecId { get; set; }

    public string? HandInstr { get; set; }

    public string? OrderId { get; set; }

    public string? TimeInForce { get; set; }

    public decimal? AvgPx { get; set; }

    public string? Account { get; set; }

    public int? OrderQty { get; set; }

    public int? CumQty { get; set; }

    public int? LeavesQty { get; set; }

    public decimal? LastPx { get; set; }

    public int? LastQty { get; set; }

    public string? TradeDate { get; set; }

    public int? TradeNumber { get; set; }

    public string? TransactTime { get; set; }

    public string? ExpireDate { get; set; }

    public int? MultiLegReportType { get; set; }

    public string? Symbol { get; set; }

    public string? SecurityId { get; set; }

    public string? Idsource { get; set; }

    public string? OrdStatus { get; set; }

    public string? ExecType { get; set; }

    public string? OrdType { get; set; }

    public decimal? Price { get; set; }

    public string? Side { get; set; }

    public string? Text { get; set; }

    public string? OrdRejReason { get; set; }
}
