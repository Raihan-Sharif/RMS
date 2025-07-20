using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class TncmaintDbEntity
{
    public int Id { get; set; }

    public DateTime? DateTime { get; set; }

    public string? FileName { get; set; }

    public string? Desc { get; set; }

    public string? UsrId { get; set; }

    public string? Compulsory { get; set; }

    public string? Version { get; set; }

    public string? Type { get; set; }

    public string XchgCode { get; set; } = null!;
}
