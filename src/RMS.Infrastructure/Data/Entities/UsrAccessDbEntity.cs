using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class UsrAccessDbEntity
{
    public int UsrSeqNo { get; set; }

    public string UsrId { get; set; } = null!;

    public string? UsrRemoteAdd { get; set; }

    /// <summary>
    /// 0-failed; 1-succeed
    /// </summary>
    public int? UsrLoginStat { get; set; }

    public DateTime? UsrLastUpdated { get; set; }

    public string? LoginMsg { get; set; }

    public string? SystemType { get; set; }

    public string? AccessInd { get; set; }

    public int? TwoFactorAuthenticated { get; set; }
}
