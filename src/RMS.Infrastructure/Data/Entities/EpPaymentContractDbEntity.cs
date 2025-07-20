using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class EpPaymentContractDbEntity
{
    public DateTime? CreateDate { get; set; }

    public string PaymentRefNo { get; set; } = null!;

    public string ContractNo { get; set; } = null!;

    public DateTime? ContractDate { get; set; }

    public string? ContractType { get; set; }

    public string? TransType { get; set; }

    public string? DepositType { get; set; }

    public string? ClntCode { get; set; }

    public string? CoBrchCode { get; set; }

    public string? Currency { get; set; }

    public string? ConvertToMyr { get; set; }

    /// <summary>
    /// F -  Full Payment   
    /// P - Partial Payment  
    /// 
    /// </summary>
    public string? PayAction { get; set; }

    public int? PayQty { get; set; }

    /// <summary>
    /// This column is for Payable Contract Module to calculate remaining OSInterest to be paid. Applicable to Purchase Contract &amp; Contra.
    /// </summary>
    public decimal? PayInterest { get; set; }

    /// <summary>
    /// This column is for Payable Contract Module to calculate remaining OSAmount to be paid. Applicable to Purchase Contract &amp; Contra.
    /// </summary>
    public decimal? PayOsamt { get; set; }

    /// <summary>
    /// Withdraw: TT  
    /// Deposit: ATM, BC - Bank Counter
    /// 
    /// </summary>
    public string? PaymentMethod { get; set; }

    public decimal? PymtAmtByTrust { get; set; }

    public decimal? PymtAmt { get; set; }

    public string? PymtInd { get; set; }

    public string? Remark { get; set; }

    /// <summary>
    /// E - Earmark     R - Release Earmark
    /// </summary>
    public string? TrustAmtEarmarkFlag { get; set; }

    public bool? UpdateBosflag { get; set; }

    public string? PaymentStatus { get; set; }

    /// <summary>
    /// E - Earmark   R - Release Earmark
    /// </summary>
    public string? SettlementStatus { get; set; }

    public string? Bosremark { get; set; }

    public DateTime? BossystemDateTime { get; set; }

    public DateTime? LastUpdateDate { get; set; }

    public string? ProofOfPymt { get; set; }

    public bool? WdrwlK2flag { get; set; }

    public int? WdrwlK2numOfRetry { get; set; }
}
