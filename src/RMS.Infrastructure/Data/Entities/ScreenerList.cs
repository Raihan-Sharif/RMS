using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ScreenerList
{
    public int ScreenerGrpId { get; set; }

    public int ScreenerId { get; set; }

    public string? ScreenerDesc { get; set; }

    public int? IWholeMrkt { get; set; }

    public int? IMainMrkt { get; set; }

    public int? IAceMrkt { get; set; }

    public string? PreCondition { get; set; }
}
