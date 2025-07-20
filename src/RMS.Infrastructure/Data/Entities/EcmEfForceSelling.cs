using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class EcmEfForceSelling
{
    public DateTime? DtCreateDate { get; set; }

    public string? SCoBrchCode { get; set; }

    public string? ConNo { get; set; }

    public string? DfFlag { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime? TranDate { get; set; }

    public string? ClientNo { get; set; }

    public string? StockNo { get; set; }

    public decimal? Price { get; set; }

    public decimal? OstQty { get; set; }

    public decimal? OstAmt { get; set; }

    public string? AutoCtr { get; set; }

    public decimal? SoldItem { get; set; }

    public string? Suspended { get; set; }

    public DateTime? SuspendDate { get; set; }

    public decimal? OsTrust { get; set; }

    public decimal? NetCtr { get; set; }

    public decimal? OstDay { get; set; }

    public string? Status { get; set; }
}
