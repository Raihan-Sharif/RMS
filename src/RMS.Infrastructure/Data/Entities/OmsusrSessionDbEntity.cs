using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class OmsusrSessionDbEntity
{
    public string SvrId { get; set; } = null!;

    public int SvrSource { get; set; }

    public string UsrId { get; set; } = null!;

    public string UsrSessionId { get; set; } = null!;

    public DateTime? LastUpdate { get; set; }

    public int UsrLoginFlag { get; set; }
}
