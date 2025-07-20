using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class XlogMstStkTradeSettBrchCtrl
{
    public int SeqNo { get; set; }

    public DateTime LogDate { get; set; }

    public string LogAction { get; set; } = null!;

    public string? LogUsr { get; set; }

    public string? XchgCode { get; set; }

    public string? StkCode { get; set; }

    public string Item { get; set; } = null!;

    public string? ExstVal { get; set; }

    public string? NewVal { get; set; }

    public string? CoBrchCode { get; set; }
}
