using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class EnqConcerrentUser
{
    public DateTime Date { get; set; }

    public string BranchCode { get; set; } = null!;

    public int? Ecosclient { get; set; }

    public int? Ecosguest { get; set; }

    public int? MobileClient { get; set; }

    public int? MobileGuest { get; set; }

    public int? Btx { get; set; }

    public int? Btxmobility { get; set; }
}
