using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class XlogThresholdLimitExcp
{
    public int SeqNo { get; set; }

    public DateTime LogDate { get; set; }

    public string LogAction { get; set; } = null!;

    public string? LogUsr { get; set; }

    public string? CoBrchCode { get; set; }

    public string? UsrId { get; set; }

    public string? ClntCode { get; set; }

    public string? XchgCode { get; set; }

    public string? Type { get; set; }

    public decimal? ThresholdLimit { get; set; }

    public string Item { get; set; } = null!;

    public string? ExstVal { get; set; }

    public string? NewVal { get; set; }

    public string? ExpsExstVal { get; set; }

    public string? ExpsNewVal { get; set; }

    public string? TopUpExstVal { get; set; }

    public string? TopUpNewVal { get; set; }
}
