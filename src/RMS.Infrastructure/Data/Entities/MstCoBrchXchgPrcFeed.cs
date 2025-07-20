using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstCoBrchXchgPrcFeed
{
    public string CoCode { get; set; } = null!;

    public string CoBrchCode { get; set; } = null!;

    public string XchgCode { get; set; } = null!;

    public string? PrcFeedSvrAddr { get; set; }

    public int? PrcFeedSvrPort { get; set; }

    public int? PrcFeedHttpPort { get; set; }

    public string? DelayPrcFeedSvrAddr { get; set; }

    public int? DelayPrcFeedSvrPort { get; set; }

    public int? DelayPrcFeedHttpPort { get; set; }
}
