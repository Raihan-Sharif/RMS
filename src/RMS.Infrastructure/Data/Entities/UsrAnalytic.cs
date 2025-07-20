using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class UsrAnalytic
{
    public DateTime ClickDateTime { get; set; }

    public string UsrId { get; set; } = null!;

    public string? CoBrchCode { get; set; }

    public string NodeTitle { get; set; } = null!;

    public int NodeGrpId { get; set; }
}
