using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class SsokeyGenDbEntity
{
    public DateTime GenDate { get; set; }

    public string SsoKey { get; set; } = null!;

    public string? SsoSite { get; set; }
}
