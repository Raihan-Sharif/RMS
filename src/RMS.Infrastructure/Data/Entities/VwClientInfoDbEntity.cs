using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class VwClientInfoDbEntity
{
    public string? Gcif { get; set; }

    public string ClientCode { get; set; } = null!;

    public string? ClientName { get; set; }

    public string ClntSname { get; set; } = null!;

    public string? ClientType { get; set; }

    public string BranchCode { get; set; } = null!;

    public string? RemisierId { get; set; }

    public DateTime? AcctOpeningDate { get; set; }

    public DateTime? AcctActivationDate { get; set; }

    public DateTime? AcctSuspendDate { get; set; }

    public string? Status { get; set; }

    public string? ClientAddress { get; set; }

    public DateTime? LastClientInfoUpdateDate { get; set; }

    public string? Cdsno { get; set; }

    public bool? DealerRefCode { get; set; }

    public string? PhoneNo { get; set; }

    public string? MobileNo { get; set; }

    public string? OfficeNo { get; set; }

    public string? FaxNo { get; set; }

    public string? BankName { get; set; }

    public string? NewIcno { get; set; }

    public string? OldIcno { get; set; }

    public int CallWarrantAllowed { get; set; }

    public int ShortSellAllowed { get; set; }

    public string? Race { get; set; }

    public string? Gender { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public bool AssociateAllowed { get; set; }

    public bool LimitIndicator { get; set; }

    public bool ShrLimitIndicator { get; set; }

    public DateTime? AcctReActivationDate { get; set; }

    public string? SuspendReason { get; set; }

    public DateTime? AcctDormantDate { get; set; }

    public string AcctLegalStatus { get; set; } = null!;

    public int? Nationality { get; set; }

    public bool? DelinquentStatus { get; set; }

    public string? InternalAccountType { get; set; }

    public int Bumiputera { get; set; }

    public bool? DormantStatus { get; set; }

    public decimal ClntExpsBuyAmt { get; set; }

    public decimal ClntExpsSellAmt { get; set; }

    public decimal ClntExpsNetAmt { get; set; }

    public decimal ClntExpsTotalAmt { get; set; }

    public decimal ClntExpsAddLimitPctg { get; set; }

    public decimal ClntExpsBuyAmtTopUp { get; set; }

    public decimal ClntExpsSellAmtTopUp { get; set; }

    public decimal ClntExpsNetAmtTopUp { get; set; }

    public decimal ClntExpsTotalAmtTopUp { get; set; }

    public decimal ClntExpsBuyPrevDayOrder { get; set; }

    public decimal ClntExpsSellPrevDayOrder { get; set; }

    public decimal ClntExpsNetPrevDayOrder { get; set; }

    public decimal ClntExpsTotalPrevDayOrder { get; set; }

    public int ClntTradeStatus { get; set; }

    public decimal ClntMarginOs { get; set; }

    public decimal ClntMarginEq { get; set; }

    public decimal ClntExpsBuyDayOrder { get; set; }

    public decimal ClntExpsSellDayOrder { get; set; }

    public decimal ClntExpsTotalDayOrder { get; set; }

    public decimal ClntExpsNetDayOrder { get; set; }

    public decimal ClntExpsFstamt { get; set; }

    public decimal ClntExpsFstamtTopUp { get; set; }

    public decimal ClntExpsFstdayOrder { get; set; }

    public decimal ClntExpsFstprevDayOrder { get; set; }

    public decimal EcosbuyPct { get; set; }

    public decimal EcossellPct { get; set; }

    public decimal EcostotalPct { get; set; }

    public decimal EcosnetPct { get; set; }

    public decimal EcosbuyAmt { get; set; }

    public decimal EcossellAmt { get; set; }

    public decimal EcostotalAmt { get; set; }

    public decimal EcosnetAmt { get; set; }

    public decimal EcosbuyAmtTopUp { get; set; }

    public decimal EcossellAmtTopUp { get; set; }

    public decimal EcostotalAmtTopUp { get; set; }

    public decimal EcosnetAmtTopUp { get; set; }

    public int ClntTradeAllowIdss { get; set; }
}
