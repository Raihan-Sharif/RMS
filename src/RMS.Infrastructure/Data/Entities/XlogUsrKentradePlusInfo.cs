using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class XlogUsrKentradePlusInfo
{
    public int SeqNo { get; set; }

    public DateTime LogDate { get; set; }

    /// <summary>
    /// A-Add; E-Edit; D-Delete
    /// </summary>
    public string LogAction { get; set; } = null!;

    public string? LogUsr { get; set; }

    public string? UsrId { get; set; }

    public string Item { get; set; } = null!;

    public string? ExstVal { get; set; }

    public string? NewVal { get; set; }
}
