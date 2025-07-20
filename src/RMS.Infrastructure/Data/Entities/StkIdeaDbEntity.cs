using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class StkIdeaDbEntity
{
    public int SeqNo { get; set; }

    public string XchgCode { get; set; } = null!;

    public string StkCode { get; set; } = null!;

    public bool? Publish { get; set; }

    public DateTime? DateFrom { get; set; }

    public DateTime? DateTo { get; set; }

    public string LogUsr { get; set; } = null!;

    public DateTime? LastUpdateDate { get; set; }

    public string? Category { get; set; }

    public string? Boval { get; set; }
}
