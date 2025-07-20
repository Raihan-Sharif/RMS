using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ClntStkSumm
{
    public string CoBrchCode { get; set; } = null!;

    public string ClntCode { get; set; } = null!;

    public string StkCode { get; set; } = null!;

    public decimal? StkAbp { get; set; }

    public decimal? StkAbpti { get; set; }

    public decimal? StkCapPrc { get; set; }

    public string? StkCapLv { get; set; }

    public long? StkPldgQty { get; set; }

    public long? StkPldgPendQty { get; set; }

    public long? StkPldgQtyNoVal { get; set; }

    public long? Cusqty { get; set; }

    public long? Dsqty { get; set; }

    public long? Cupqty { get; set; }

    public long? Dupqty { get; set; }

    public long? StkThirdQty { get; set; }

    public long? FreePurchasedQty { get; set; }

    public long? StkThirdPendQty { get; set; }
}
