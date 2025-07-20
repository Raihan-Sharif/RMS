using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ScotrssummDbEntity
{
    public string CoBrchCode { get; set; } = null!;

    public string ClntCode { get; set; } = null!;

    public DateTime Date { get; set; }

    public string? Side { get; set; }

    public string StkCode { get; set; } = null!;

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }

    public string? TerminalId { get; set; }

    public string OrderNo { get; set; } = null!;

    public string Trsno { get; set; } = null!;

    public DateTime? LastUpdateDate { get; set; }

    public int? Status { get; set; }

    public string? ErrMsg { get; set; }
}
