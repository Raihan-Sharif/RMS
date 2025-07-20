using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class OrderGroupDtl
{
    public int GroupCode { get; set; }

    public string UsrId { get; set; } = null!;

    public bool? ViewOrder { get; set; }

    public bool? PlaceOrder { get; set; }

    public bool? ViewClient { get; set; }

    public bool? ModifyOrder { get; set; }
}
