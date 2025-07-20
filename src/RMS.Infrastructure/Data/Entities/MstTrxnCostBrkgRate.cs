using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstTrxnCostBrkgRate
{
    public int SeqNo { get; set; }

    public decimal? AmtFrom { get; set; }

    public decimal? AmtTo { get; set; }

    public decimal? Rate { get; set; }
}
