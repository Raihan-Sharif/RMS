using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class UsrAccessFa
{
    public string UsrId { get; set; } = null!;

    public DateTime AccessDate { get; set; }
}
