using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class PaymentRefNoExceptionArchiveDbEntity
{
    public string PaymentRefNo { get; set; } = null!;

    public decimal? PaymentAmt { get; set; }

    public DateTime? LastUpdateDate { get; set; }
}
