using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class TransactionsHistoryBak20240910DbEntity
{
    public int Oid { get; set; }

    public string? Category { get; set; }

    public DateTime TransDate { get; set; }

    public string ClientCode { get; set; } = null!;

    public string? StockCode { get; set; }

    public string? RemisierId { get; set; }

    public string? TransType { get; set; }

    public int? Quantity { get; set; }

    public string? RefNo { get; set; }

    public string? StatusCode { get; set; }

    public string? TransDetail1 { get; set; }

    public string? TransDetail2 { get; set; }

    public decimal? Price { get; set; }

    public DateTime? UploadDate { get; set; }

    public string? BranchCode { get; set; }

    public string? OrderNo { get; set; }

    public string? TerminalId { get; set; }

    public DateTime? AmendmentTime { get; set; }

    public string? AmendedFrom { get; set; }

    public string? XchgCode { get; set; }

    public string? CurcyCode { get; set; }
}
