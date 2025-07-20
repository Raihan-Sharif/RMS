using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwClientshareinfoDbEntity
{
    public string ClientCode { get; set; } = null!;

    public string BranchCode { get; set; } = null!;

    public string StockCode { get; set; } = null!;

    public int? OpenFreeBalance { get; set; }

    public int? OpenPurchaseQty { get; set; }

    public int? OpenSalesQty { get; set; }

    public int? TodayEarmarkedQty { get; set; }

    public int? TodayPurchaseQty { get; set; }

    public int? TodaySoldQty { get; set; }

    public int? TodayTransferOutQty { get; set; }

    public int? MaxWithdrawalQty { get; set; }

    public string? Cdsno { get; set; }
}
