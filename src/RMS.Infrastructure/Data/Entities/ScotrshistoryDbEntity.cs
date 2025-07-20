using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ScotrshistoryDbEntity
{
    public DateTime CreateDate { get; set; }

    public string? SellerCoCode { get; set; }

    public string? SellerCoBrchCode { get; set; }

    public string SellerCdsno { get; set; } = null!;

    public DateTime? Date { get; set; }

    public string Trsno { get; set; } = null!;

    public string? BuyerCoCode { get; set; }

    public string? BuyerCoBrchCode { get; set; }

    public string BuyerCdsno { get; set; } = null!;

    public string? Side { get; set; }

    public string? StkCode { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }

    public string? LotCode { get; set; }

    public string? TerminalId { get; set; }

    public string OrderNo { get; set; } = null!;

    public string? ShortSell { get; set; }

    public string? ClOrdId { get; set; }

    public DateTime? LastUpdateDate { get; set; }

    public int? Status { get; set; }

    public string? ErrMsg { get; set; }
}
