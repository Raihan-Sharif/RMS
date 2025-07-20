using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstCoBrchEcm
{
    public string CoCode { get; set; } = null!;

    public string CoBrchCode { get; set; } = null!;

    public string? CoBrchDesc { get; set; }

    public string? CoBrchAddr { get; set; }

    public string? CoBrchPhone { get; set; }

    public string? PrcFeedSvrAddr { get; set; }

    public int? PrcFeedSvrPort { get; set; }

    public int? PrcFeedHttpPort { get; set; }

    public string? BrchGrpCode { get; set; }
}
