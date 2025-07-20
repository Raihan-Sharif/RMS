using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class InsageSettingDbEntity
{
    public string Code { get; set; } = null!;

    public string? CodeDesc { get; set; }

    public int? SeqNo { get; set; }

    public string? ParentCode { get; set; }

    public string? ActiveIconUrl { get; set; }

    public string? InActiveIconUrl { get; set; }

    public string? DefaultCode { get; set; }

    public int? SystemCode { get; set; }
}
