using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class SsokeyGen
{
    public DateTime GenDate { get; set; }

    public string SsoKey { get; set; } = null!;

    public string? SsoSite { get; set; }
}
