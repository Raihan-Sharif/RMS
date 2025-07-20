using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ClntChgBrch
{
    public string ClntCode { get; set; } = null!;

    public string OldBrchCode { get; set; } = null!;

    public string NewBrchCode { get; set; } = null!;

    public string DealerCode { get; set; } = null!;

    public string UsrId { get; set; } = null!;

    public int CancelInd { get; set; }
}
