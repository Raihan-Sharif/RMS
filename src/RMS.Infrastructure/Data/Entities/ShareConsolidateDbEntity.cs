using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ShareConsolidateDbEntity
{
    public string XchgCode { get; set; } = null!;

    public string StkCode { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Message { get; set; } = null!;
}
