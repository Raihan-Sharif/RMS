using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class UsrAccessFaDbEntity
{
    public string UsrId { get; set; } = null!;

    public DateTime AccessDate { get; set; }
}
