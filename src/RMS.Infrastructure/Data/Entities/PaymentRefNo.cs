using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class PaymentRefNo
{
    public DateTime? PaymentDate { get; set; }

    public string PaymentRefNo1 { get; set; } = null!;

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

    public string? PayeeCode { get; set; }

    public string? ChannelId { get; set; }

    public string? RetUsrName { get; set; }

    public string? TransTime { get; set; }

    public DateTime? DtLastUpdateDate { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? Currency { get; set; }

    public decimal? PaymentAmtByTrust { get; set; }

    public string? SettlementStatus { get; set; }

    public string? SReturnBankRef { get; set; }

    public DateTime? TrxnDateTime { get; set; }

    public string? PaymentBankBranch { get; set; }
}
