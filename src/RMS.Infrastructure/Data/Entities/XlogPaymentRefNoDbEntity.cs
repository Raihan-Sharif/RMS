using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class XlogPaymentRefNoDbEntity
{
    public DateTime LogDate { get; set; }

    public string PaymentRefNo { get; set; } = null!;

    public string ContractNo { get; set; } = null!;

    public string? CustId { get; set; }

    public string? PaymentType { get; set; }

    public decimal? PaymentAmt { get; set; }

    public string? PaymentStatus { get; set; }

    public string? PaymentBank { get; set; }

    public string? ClntCode { get; set; }

    public string? ReturnCode { get; set; }

    public string? ReturnMsg { get; set; }

    public string? BRequest { get; set; }

    public decimal? ReqAmount { get; set; }

    public DateTime? ReqDate { get; set; }

    public string? ConfNum { get; set; }

    public string? TransDate { get; set; }

    public string? PymtInd { get; set; }
}
