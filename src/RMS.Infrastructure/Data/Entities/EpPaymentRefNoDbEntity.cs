using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class EpPaymentRefNoDbEntity
{
    public DateTime? CreateDate { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string PaymentRefNo { get; set; } = null!;

    public string? UsrId { get; set; }

    public string? ClntCode { get; set; }

    public string? CoBrchCode { get; set; }

    public string? Currency { get; set; }

    public decimal? PaymentAmtByTrust { get; set; }

    public decimal? PaymentAmt { get; set; }

    public string? PaymentBank { get; set; }

    public string? PaymentBankBrch { get; set; }

    public string? PaymentBankAccNo { get; set; }

    public string? PaymentBankRefNo { get; set; }

    public string? TrnxRefNo { get; set; }

    public string? ReturnCode { get; set; }

    public string? ReturnMsg { get; set; }

    public DateTime? TrxnDateTime { get; set; }

    public DateTime? DtLastUpdateDate { get; set; }

    public string BankId { get; set; } = null!;
}
