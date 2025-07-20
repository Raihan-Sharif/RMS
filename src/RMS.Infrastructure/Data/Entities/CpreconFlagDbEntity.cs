using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class CpreconFlagDbEntity
{
    public byte? CpFlag { get; set; }

    public DateTime? CpOnTime { get; set; }

    public DateTime? CpOffTime { get; set; }

    public byte? ReconDoneFlag { get; set; }

    public DateTime? ReconDoneOnTime { get; set; }

    public DateTime? ReconDoneOffTime { get; set; }
}
