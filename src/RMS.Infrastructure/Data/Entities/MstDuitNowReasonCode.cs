using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstDuitNowReasonCode
{
    public string ReasonCode { get; set; } = null!;

    public string StatusCode { get; set; } = null!;

    public string ReasonDescription { get; set; } = null!;
}
