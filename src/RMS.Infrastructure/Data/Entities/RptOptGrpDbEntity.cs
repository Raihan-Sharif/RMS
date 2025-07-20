using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class RptOptGrpDbEntity
{
    /// <summary>
    /// D - Daily; W - Weekly; M - Monthly; Q - Quaterly; H - Half-Yearly; Y - Yearly;
    /// </summary>
    public string RptGrpId { get; set; } = null!;

    public string? RptGrpDesc { get; set; }

    public string? RptFolderPath { get; set; }
}
