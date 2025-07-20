using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstTrxnCost20190124
{
    public decimal? StampDutyMin { get; set; }

    public decimal? StampDutyVal { get; set; }

    public decimal? StampDutyMax { get; set; }

    public decimal? ClearingFee { get; set; }

    public decimal? MaxClearingFee { get; set; }

    public decimal? MinBrkgNormal { get; set; }

    public decimal? MinBrkgOther { get; set; }

    public decimal? GstbrokerageFee { get; set; }

    public decimal? GstclearingFee { get; set; }

    public decimal? GststampDuty { get; set; }
}
