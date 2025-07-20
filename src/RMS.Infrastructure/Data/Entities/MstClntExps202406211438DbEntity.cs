using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstClntExps202406211438DbEntity
{
    public string CoBrchCode { get; set; } = null!;

    public string ClntCode { get; set; } = null!;

    public bool? ClntExpsCheckBuy { get; set; }

    public decimal? ClntExpsBuyAmt { get; set; }

    public decimal? ClntExpsBuyAmtTopUp { get; set; }

    public decimal? ClntExpsBuyDayOrder { get; set; }

    public decimal? ClntExpsBuyPrevDayOrder { get; set; }

    public bool? ClntExpsCheckSell { get; set; }

    public decimal? ClntExpsSellAmt { get; set; }

    public decimal? ClntExpsSellAmtTopUp { get; set; }

    public decimal? ClntExpsSellDayOrder { get; set; }

    public decimal? ClntExpsSellPrevDayOrder { get; set; }

    public bool? ClntExpsCheckTotal { get; set; }

    public decimal? ClntExpsTotalAmt { get; set; }

    public decimal? ClntExpsTotalAmtTopUp { get; set; }

    public decimal? ClntExpsTotalDayOrder { get; set; }

    public decimal? ClntExpsTotalPrevDayOrder { get; set; }

    public bool? ClntExpsCheckNet { get; set; }

    public decimal? ClntExpsNetAmt { get; set; }

    public decimal? ClntExpsNetAmtTopUp { get; set; }

    public decimal? ClntExpsNetDayOrder { get; set; }

    public decimal? ClntExpsNetPrevDayOrder { get; set; }

    public bool? ClntExpsWithLimit { get; set; }

    public decimal? ClntExpsAddLimitPctg { get; set; }

    public int? ClntTradeStatus { get; set; }

    public int? ClntTradeAllow { get; set; }

    public bool? ClntExpsWithShrLimit { get; set; }

    public decimal? ClntMarginOs { get; set; }

    public decimal? ClntMarginEq { get; set; }

    public decimal? ClntMaxCotrLossAmt { get; set; }

    public string? ClntRemarks { get; set; }

    public string? ClntExpsRemarks { get; set; }

    public bool? ClntExpsCheckFst { get; set; }

    public decimal? ClntExpsFstamt { get; set; }

    public decimal? ClntExpsFstamtTopUp { get; set; }

    public decimal? ClntExpsFstdayOrder { get; set; }

    public decimal? ClntExpsFstprevDayOrder { get; set; }

    public int? ClntTradeAllowOrd { get; set; }

    public int? ClntTradeAllowMod { get; set; }

    public int? ClntTradeBuyBidLimit { get; set; }

    public int? ClntTradeSellBidLimit { get; set; }

    public bool? ClntExpsDlrAuthShare { get; set; }

    public int? EcosbuyBidLimit { get; set; }

    public int? EcossellBidLimit { get; set; }

    public decimal? ClntExpsEcosamt { get; set; }

    public bool? ClntPortfolioInd { get; set; }

    public decimal? EcosbuyPct { get; set; }

    public decimal? EcossellPct { get; set; }

    public decimal? EcostotalPct { get; set; }

    public decimal? EcosnetPct { get; set; }

    public decimal? EcosbuyAmt { get; set; }

    public decimal? EcossellAmt { get; set; }

    public decimal? EcostotalAmt { get; set; }

    public decimal? EcosnetAmt { get; set; }

    public decimal? EcosbuyAmtTopUp { get; set; }

    public decimal? EcossellAmtTopUp { get; set; }

    public decimal? EcostotalAmtTopUp { get; set; }

    public decimal? EcosnetAmtTopUp { get; set; }

    public bool? ClntExpsCheckIdssgross { get; set; }

    public decimal? ClntExpsIdssgrossAmt { get; set; }

    public decimal? ClntExpsIdssgrossAmtTopUp { get; set; }

    public decimal? ClntExpsIdssgrossDayOrder { get; set; }

    public decimal? ClntExpsIdssgrossPrevDayOrder { get; set; }

    public bool? ClntExpsCheckIdss { get; set; }

    public decimal? ClntExpsIdssamt { get; set; }

    public decimal? ClntExpsIdssamtTopUp { get; set; }

    public decimal? ClntExpsIdssdayOrder { get; set; }

    public decimal? ClntExpsIdssprevDayOrder { get; set; }
}
