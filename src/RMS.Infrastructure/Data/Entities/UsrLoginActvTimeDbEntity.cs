using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class UsrLoginActvTimeDbEntity
{
    public string UsrId { get; set; } = null!;

    public int UsrLogin { get; set; }

    public DateTime? UsrActiveTime { get; set; }
}
