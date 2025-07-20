using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class FormBizProcNotCotrDbEntity
{
    public string FormRefNo { get; set; } = null!;

    public DateTime TrnxDate { get; set; }

    public int SeqNo { get; set; }

    public string? CoBrchCode { get; set; }

    public string? ClntCode { get; set; }

    public string? StkCode { get; set; }

    public string? Type { get; set; }

    public string? TrxnType { get; set; }

    public string? Reason { get; set; }

    public DateTime? ContDate { get; set; }

    public string? ContNo { get; set; }

    public int? Qty { get; set; }

    public decimal? Price { get; set; }

    public string? BuyTerminalId { get; set; }

    public string? BuyOrderNo { get; set; }

    public int? BuyQty { get; set; }

    public decimal? BuyPrice { get; set; }

    public string? SellTerminalId { get; set; }

    public string? SellOrderNo { get; set; }

    public int? SellQty { get; set; }

    public decimal? SellPrice { get; set; }
}
