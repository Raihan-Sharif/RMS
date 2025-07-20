using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class DbtqueueDbEntity
{
    public int DbtseqNo { get; set; }

    public string? MemberRef { get; set; }

    public string? Tcsref { get; set; }

    public string UserId { get; set; } = null!;

    public DateTime PlaceDbttime { get; set; }

    public string Dbtflag { get; set; } = null!;

    public string? XchgCode { get; set; }

    public int? OrderSeqNo { get; set; }
}
