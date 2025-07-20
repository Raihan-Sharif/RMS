using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class RsaSettingDbEntity
{
    public bool? EnableRsa { get; set; }

    public string? WebProxy { get; set; }

    public bool? WebProxyBypassOnLocal { get; set; }

    public bool? WebProxyNeedCredential { get; set; }

    public string? WebProxyLogin { get; set; }

    public string? WebProxyPassword { get; set; }

    public string? WebProxyDomain { get; set; }

    public bool? BypassProxy { get; set; }

    public bool? EnableMobileRsa { get; set; }
}
