using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class EpFpxdepBk
{
    public int SeqNo { get; set; }

    public string BkCd { get; set; } = null!;

    public string BkDesc { get; set; } = null!;

    public string TestInd { get; set; } = null!;
}
