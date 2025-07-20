using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class SiteMenuGrp
{
    public int NodeGrpSystemType { get; set; }

    public int NodeGrpId { get; set; }

    public string? NodeGrpDesc { get; set; }

    public string? NodeGrpDescVn { get; set; }

    public string? NodeGrpActiveIconUrl { get; set; }

    public string? NodeGrpInActiveIconUrl { get; set; }

    public string? NodeGrpActiveTabUrl { get; set; }

    public string? NodeGrpInActiveTabUrl { get; set; }

    public int? NodeGrpTabWidth { get; set; }
}
