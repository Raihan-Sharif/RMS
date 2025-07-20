using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class PaymentRefNoArchiveDbEntity
{
    public DateTime? PaymentDate { get; set; }

    public string PaymentRefNo { get; set; } = null!;

    public string? CustId { get; set; }

    public string? PaymentType { get; set; }

    public decimal? PaymentAmt { get; set; }

    public string? PaymentStatus { get; set; }

    public string? PaymentBank { get; set; }

    public string? AddRefNo { get; set; }

    public string? TrnxRefNo { get; set; }

    public string? ReturnCode { get; set; }

    public string? ReturnMsg { get; set; }

    public string? Request { get; set; }

    public decimal? ReqAmount { get; set; }

    public DateTime? ReqDate { get; set; }

    public string? TransDate { get; set; }

    public string? CoBrchCode { get; set; }
}
