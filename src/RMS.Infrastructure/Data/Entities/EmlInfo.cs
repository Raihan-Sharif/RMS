using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class EmlInfo
{
    public int EmlId { get; set; }

    public string? EmlDesc { get; set; }

    public string? EmlSubject { get; set; }

    public string? EmlBody { get; set; }
}
