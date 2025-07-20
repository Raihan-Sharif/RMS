using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class FormBizProcMailUplift
{
    public string FormRefNo { get; set; } = null!;

    public DateTime TrnxDate { get; set; }

    public string? CoBrchCode { get; set; }

    public string? ClntCode { get; set; }

    public string? ReasonType { get; set; }

    public string? Reason { get; set; }
}
