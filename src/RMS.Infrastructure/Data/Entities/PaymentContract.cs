using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class PaymentContract
{
    public string PaymentRefNo { get; set; } = null!;

    public string ContractNo { get; set; } = null!;

    public DateTime? LastUpdateDate { get; set; }

    public string? PaymentType { get; set; }

    public decimal? PymtAmt { get; set; }

    public string? PymtInd { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? ContractDate { get; set; }

    public string? ContractType { get; set; }

    public string? TransType { get; set; }

    public string? ClntCode { get; set; }

    public string? CoBrchCode { get; set; }

    /// <summary>
    /// F - Full Payment  
    /// P - Partially Payment
    /// </summary>
    public string? PayAction { get; set; }

    public int? PayQty { get; set; }

    public decimal? PayInterest { get; set; }

    public decimal? PymtAmtByTrust { get; set; }

    public string? Remark { get; set; }

    public string? DepositType { get; set; }

    public bool? UpdateBosflag { get; set; }
}
