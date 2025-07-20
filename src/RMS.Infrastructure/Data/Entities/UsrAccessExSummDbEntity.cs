using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class UsrAccessExSummDbEntity
{
    public string UsrId { get; set; } = null!;

    public int UsrIdyymm { get; set; }

    public DateTime? UsrLastUpdated { get; set; }
}
