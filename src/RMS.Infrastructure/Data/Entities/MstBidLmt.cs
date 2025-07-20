using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstBidLmt
{
    public int Id { get; set; }

    public string? DataCode { get; set; }

    public string? OrderType { get; set; }

    public int? OrderSrc { get; set; }

    public int? CtrlType { get; set; }
}
