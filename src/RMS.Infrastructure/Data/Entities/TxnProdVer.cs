using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class TxnProdVer
{
    public string PlatCode { get; set; } = null!;

    public string? FileName { get; set; }

    public string? Desc { get; set; }

    public string ProgramVer { get; set; } = null!;

    public int ReleaseVer { get; set; }

    public DateTime? ReleaseDate { get; set; }

    public bool? ForceDownload { get; set; }
}
