using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class UsrAccess202205171150DbEntity
{
    public int UsrSeqNo { get; set; }

    public string UsrId { get; set; } = null!;

    public string? UsrRemoteAdd { get; set; }

    public int? UsrLoginStat { get; set; }

    public DateTime? UsrLastUpdated { get; set; }

    public string? LoginMsg { get; set; }

    public string? SystemType { get; set; }

    public string? AccessInd { get; set; }
}
