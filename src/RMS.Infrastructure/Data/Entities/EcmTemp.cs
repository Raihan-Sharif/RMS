using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class EcmTemp
{
    public string? ClientNo { get; set; }

    public string? ConNo { get; set; }

    public string? ApplyTo { get; set; }

    public DateTime? ConDate { get; set; }

    public DateTime? DueDate { get; set; }

    public string? ConRef { get; set; }

    public string? TranType { get; set; }

    public string? StockNo { get; set; }

    public decimal? Price { get; set; }

    public decimal? TranQty { get; set; }

    public decimal? OstQty { get; set; }

    public decimal? TranAmt { get; set; }

    public decimal? OstAmt { get; set; }

    public decimal? PayTrust { get; set; }

    public decimal? PayBank { get; set; }

    public decimal? TotalTrust { get; set; }

    public decimal? TotalBank { get; set; }

    public decimal? TrustBal { get; set; }

    public decimal? AccrInt { get; set; }

    public decimal? NoOfRec { get; set; }

    public string? EfRefNo { get; set; }

    public string? BankRefNo { get; set; }

    public string? BankCode { get; set; }

    public string? RecMode { get; set; }

    public string? RecStatus { get; set; }

    public string? Remarks { get; set; }

    public DateTime? SysTime { get; set; }
}
