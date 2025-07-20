using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class XlogMstGlobalLimitExpDbEntity
{
    public int SeqNo { get; set; }

    public DateTime LogDate { get; set; }

    public string LogAction { get; set; } = null!;

    public string? LogUsr { get; set; }

    public string? ExpsType { get; set; }

    public decimal? LimitChangePctg { get; set; }

    public string? Type { get; set; }

    public string? Module { get; set; }

    public string Item { get; set; } = null!;

    public string? ExstVal { get; set; }

    public string? NewVal { get; set; }

    public string? DataCode { get; set; }

    public string? ShortModule { get; set; }
}
