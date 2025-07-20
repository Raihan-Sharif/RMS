using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class EpFpxmsgDtlDbEntity
{
    public int ISeqNo { get; set; }

    public DateTime DtRecDate { get; set; }

    public string SFpxMsgType { get; set; } = null!;

    public string SFpxMsgToken { get; set; } = null!;

    /// <summary>
    /// Direct - Direct AC Message  
    /// Indirect - Indirect AC Message
    /// 
    /// </summary>
    public string? SFpxAcmsgType { get; set; }

    public string? SFpxFpxtxnId { get; set; }

    public string SFpxSellerExId { get; set; } = null!;

    public string SFpxSellerExOrderNo { get; set; } = null!;

    public DateTime? DtFpxFpxtxnTime { get; set; }

    public DateTime DtFpxSellerTxnTime { get; set; }

    public string SFpxSellerOrderNo { get; set; } = null!;

    public string SFpxSellerId { get; set; } = null!;

    public string? SFpxSellerBankCode { get; set; }

    public string SFpxTxnCurrency { get; set; } = null!;

    public decimal DFpxTxnAmount { get; set; }

    public string? SFpxBuyerEmail { get; set; }

    public string SFpxCheckSum { get; set; } = null!;

    public string? SFpxBuyerName { get; set; }

    public string? SFpxBuyerBankId { get; set; }

    public string? SFpxBuyerBankBranch { get; set; }

    public string? SFpxBuyerAccNo { get; set; }

    public string? SFpxBuyerId { get; set; }

    public string? SFpxMakerName { get; set; }

    public string? SFpxBuyerIban { get; set; }

    public string? SFpxProductDesc { get; set; }

    public string? SFpxVersion { get; set; }

    public string? SFpxDebitAuthCode { get; set; }

    public string? SFpxDebitAuthNo { get; set; }

    public string? SFpxCreditAuthCode { get; set; }

    public string? SFpxCreditAuthNo { get; set; }
}
