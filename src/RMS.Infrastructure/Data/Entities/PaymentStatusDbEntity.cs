using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class PaymentStatusDbEntity
{
    public DateTime? DateTime { get; set; }

    public string? MerchantId { get; set; }

    public string? OrderNo { get; set; }

    public string? AddRefNo { get; set; }
}
