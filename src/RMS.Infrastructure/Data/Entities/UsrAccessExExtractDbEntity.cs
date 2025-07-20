using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class UsrAccessExExtractDbEntity
{
    public int UsrSeqNo { get; set; }

    public string? UsrId { get; set; }

    public DateTime? UsrLastUpdated { get; set; }
}
