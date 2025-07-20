using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class WinscoreSettingDbEntity
{
    public string BrkCode { get; set; } = null!;

    public string? UsrIdmode { get; set; }

    public string? BranchIdmode { get; set; }

    public string BranchToVarchar { get; set; } = null!;

    public string FormatVersion { get; set; } = null!;

    public string TrimClntCode { get; set; } = null!;

    public string DateIntFormat { get; set; } = null!;

    public string ReuseDlrCode { get; set; } = null!;

    public string? DefChannel { get; set; }
}
