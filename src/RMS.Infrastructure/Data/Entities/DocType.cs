using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class DocType
{
    public string DocType1 { get; set; } = null!;

    public string? DocDesc { get; set; }

    public long? DocNextNo { get; set; }

    public long? DocMaxNo { get; set; }

    public bool? IncludeDocType { get; set; }

    public bool? IncludeDate { get; set; }
}
