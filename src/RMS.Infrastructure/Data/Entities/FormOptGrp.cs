using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class FormOptGrp
{
    /// <summary>
    /// D - Daily; W - Weekly; M - Monthly; Q - Quaterly; H - Half-Yearly; Y - Yearly;
    /// </summary>
    public string FormGrpId { get; set; } = null!;

    public string? FormGrpDesc { get; set; }

    public string? ActiveIconUrl { get; set; }

    public string? InActiveIconUrl { get; set; }

    public string? DefaultFormId { get; set; }
}
