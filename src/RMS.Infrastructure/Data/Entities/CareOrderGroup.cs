using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class CareOrderGroup
{
    public int GroupId { get; set; }

    public string? GroupDesc { get; set; }

    public string? ClientCode { get; set; }

    public string? BranchId { get; set; }

    public int? MinQty { get; set; }

    public int? MaxQty { get; set; }

    public string? DealerId { get; set; }

    public string? UserId { get; set; }

    public int? Shariah { get; set; }
}
