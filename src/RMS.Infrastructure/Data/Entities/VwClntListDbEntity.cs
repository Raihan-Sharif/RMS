using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwClntListDbEntity
{
    public string? Gcif { get; set; }

    public string? ClntStat { get; set; }

    public string? UsrId { get; set; }

    public string CoBrchCode { get; set; } = null!;

    public string ClntCode { get; set; } = null!;

    public string? OriginateId { get; set; }

    public int IsAssociated { get; set; }

    public bool AllowAssociate { get; set; }

    public string? ClntName { get; set; }

    public string? ClntSname { get; set; }

    public string? ClntCdsno { get; set; }
}
