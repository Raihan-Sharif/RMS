using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ItrBosshareInfo
{
    public string CoBrchCode { get; set; } = null!;

    public string ClntCode { get; set; } = null!;

    public string XchgCode { get; set; } = null!;

    public string StkCode { get; set; } = null!;

    public int? FreeQty { get; set; }

    public int? OpenPqty { get; set; }

    public int? OpenSqty { get; set; }

    public DateTime? LastUpdateDate { get; set; }
}
