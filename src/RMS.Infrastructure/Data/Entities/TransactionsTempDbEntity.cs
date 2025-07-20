using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class TransactionsTempDbEntity
{
    public string? Oid { get; set; }

    public string? Category { get; set; }

    public string? TransDate { get; set; }

    public string? ClientCode { get; set; }

    public string? StockCode { get; set; }

    public string? RemisierId { get; set; }

    public string? TransType { get; set; }

    public string? Quantity { get; set; }

    public string? RefNo { get; set; }

    public string? StatusCode { get; set; }

    public string? TransDetail1 { get; set; }

    public string? TransDetail2 { get; set; }

    public string? Price { get; set; }

    public string? UploadDate { get; set; }

    public string? BranchCode { get; set; }

    public string? OrderNo { get; set; }

    public string? TerminalId { get; set; }

    public string? AmendmentTime { get; set; }

    public string? AmendedFrom { get; set; }
}
