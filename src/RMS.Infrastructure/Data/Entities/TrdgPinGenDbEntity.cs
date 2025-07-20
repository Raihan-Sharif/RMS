using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class TrdgPinGenDbEntity
{
    public int SeqNo { get; set; }

    public string UsrId { get; set; } = null!;

    public string? UsrTrdgPin { get; set; }

    public DateTime? UsrTrdgPinChngDate { get; set; }
}
