using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ManualOrderFilled
{
    public DateTime OrderDate { get; set; }

    public string UsrId { get; set; } = null!;

    public string? UsrName { get; set; }

    public string? ClientCode { get; set; }

    public string? ClientName { get; set; }

    public string? StockCode { get; set; }

    public string? StockName { get; set; }

    public string? OrderType { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }

    public int? FQty { get; set; }

    public decimal? FPrice { get; set; }

    public int? RoutedNo { get; set; }

    public string? Remarks { get; set; }
}
