using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class PymtRefUnknownEmailLog
{
    public DateTime ProcessDateTime { get; set; }

    public string SPymtRefNo { get; set; } = null!;
}
