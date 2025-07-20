using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class FpxresEmailSettDbEntity
{
    public int SeqNo { get; set; }

    public string From { get; set; } = null!;

    public string To { get; set; } = null!;

    public string Cc { get; set; } = null!;

    public string Subject { get; set; } = null!;
}
