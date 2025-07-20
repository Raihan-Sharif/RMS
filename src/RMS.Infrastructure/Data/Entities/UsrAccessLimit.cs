using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class UsrAccessLimit
{
    public string UsrId { get; set; } = null!;

    /// <summary>
    /// Indication if access of user is limited, checking should be applied if enabled. Y - Yes; N - No 
    /// </summary>
    public string? UsrAccessLimit1 { get; set; }

    /// <summary>
    /// Expiry date of access limit
    /// </summary>
    public DateTime? UsrAccessLimitExprDate { get; set; }

    public DateTime? UsrAccessLimitStartTime { get; set; }

    public DateTime? UsrAccessLimitEndTime { get; set; }

    public int? UsrAccessLimitDays { get; set; }
}
