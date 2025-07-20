using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class EnqUsrActvn
{
    public int? SeqNo { get; set; }

    public DateTime? LogDate { get; set; }

    public string? UsrId { get; set; }

    public int? ActivateFlag { get; set; }

    public string? ActvnEmail { get; set; }

    public DateTime? ActvnDate { get; set; }
}
