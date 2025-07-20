using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class EpDuitNowInitPymtMsgDtlDbEntity
{
    public int SeqNo { get; set; }

    public DateTime InitPymtDt { get; set; }

    public string PymtRefNo { get; set; } = null!;

    public string MsgId { get; set; } = null!;

    public string TrxId { get; set; } = null!;

    public string EndToEndId { get; set; } = null!;

    public string UsrId { get; set; } = null!;

    public string ClntCode { get; set; } = null!;

    public string CoBrchCode { get; set; } = null!;

    public string ClientId { get; set; } = null!;

    public string Currency { get; set; } = null!;

    public decimal Amount { get; set; }

    public string ProductId { get; set; } = null!;

    public string CustomerName { get; set; } = null!;

    public string BankId { get; set; } = null!;

    public string BankType { get; set; } = null!;

    public string MerchantName { get; set; } = null!;

    public string AccountType { get; set; } = null!;

    public string SourceOfFunds { get; set; } = null!;

    public string RecipientReference { get; set; } = null!;

    public string? PaymentDescription { get; set; }
}
