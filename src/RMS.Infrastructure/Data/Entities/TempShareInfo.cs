using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class TempShareInfo
{
    public string? RecordType { get; set; }

    public int? BranchId { get; set; }

    public string? ClntCode { get; set; }

    public string? XchgCode { get; set; }

    public string? StkCode { get; set; }

    public int? OpenFreeQty { get; set; }

    public int? OpenPurchaseQty { get; set; }

    public int? OpenSaleQty { get; set; }
}
