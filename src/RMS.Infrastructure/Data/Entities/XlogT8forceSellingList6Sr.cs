using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class XlogT8forceSellingList6Sr
{
    public int SeqNo { get; set; }

    public DateTime LogDate { get; set; }

    public string? BranchId { get; set; }

    public string? ClntCode { get; set; }

    public string? DealerId { get; set; }

    public int? BrokerId { get; set; }

    public string? StkCode { get; set; }

    public string? BuySellCode { get; set; }

    public int? OrderQty { get; set; }

    public string? OrderType { get; set; }

    public int? DatePurchased { get; set; }

    public int? TransactionNo { get; set; }

    public decimal? OrderPrice { get; set; }
}
