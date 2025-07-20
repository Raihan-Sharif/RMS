using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class OrderStatusFlag
{
    public string Status { get; set; } = null!;

    public string StatusDesc { get; set; } = null!;

    public short NeedUplift { get; set; }
}
