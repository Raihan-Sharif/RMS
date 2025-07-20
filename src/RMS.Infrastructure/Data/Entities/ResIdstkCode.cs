using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ResIdstkCode
{
    public int ResId { get; set; }

    public string? XchgCode { get; set; }

    public string? StkCode { get; set; }
}
