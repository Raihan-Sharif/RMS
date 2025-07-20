using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstCoExpsBak20221209
{
    public string CoCode { get; set; } = null!;

    public bool? CoExpsCheckBuy { get; set; }

    public decimal? CoExpsBuyAmt { get; set; }

    public decimal? CoExpsBuyAmtTopUp { get; set; }

    public decimal? CoExpsBuyDayOrder { get; set; }

    public decimal? CoExpsBuyPrevDayOrder { get; set; }

    public bool? CoExpsCheckSell { get; set; }

    public decimal? CoExpsSellAmt { get; set; }

    public decimal? CoExpsSellAmtTopUp { get; set; }

    public decimal? CoExpsSellDayOrder { get; set; }

    public decimal? CoExpsSellPrevDayOrder { get; set; }

    public bool? CoExpsCheckTotal { get; set; }

    public decimal? CoExpsTotalAmt { get; set; }

    public decimal? CoExpsTotalAmtTopUp { get; set; }

    public decimal? CoExpsTotalDayOrder { get; set; }

    public decimal? CoExpsTotalPrevDayOrder { get; set; }

    public bool? CoExpsCheckNet { get; set; }

    public decimal? CoExpsNetAmt { get; set; }

    public decimal? CoExpsNetAmtTopUp { get; set; }

    public decimal? CoExpsNetDayOrder { get; set; }

    public decimal? CoExpsNetPrevDayOrder { get; set; }

    public bool? CoExpsWithLimit { get; set; }

    public decimal? CoExpsAddLimitPctg { get; set; }

    public decimal? CoNormalUpLimitPctg { get; set; }

    public decimal? CoNormalLowLimitPctg { get; set; }

    public decimal? CoDbtupLimitPctg { get; set; }

    public decimal? CoDbtlowLimitPctg { get; set; }

    public int? CoTradeStatus { get; set; }

    public decimal? CoBuyTrnxLimit { get; set; }

    public int? CoBuyLotLimit { get; set; }

    public int? CoBuyBidLimitNormal { get; set; }

    public int? CoBuyBidLimitOdd { get; set; }

    public int? CoBuyBidLimitNormalOdd { get; set; }

    public int? CoDbtbuyShrLimit { get; set; }

    public decimal? CoSellTrnxLimit { get; set; }

    public int? CoSellLotLimit { get; set; }

    public int? CoSellBidLimitNormal { get; set; }

    public int? CoSellBidLimitOdd { get; set; }

    public int? CoSellBidLimitNormalOdd { get; set; }

    public int? CoDbtsellShrLimit { get; set; }

    public string? CoRemarks { get; set; }

    public bool? CoDbtcancel { get; set; }

    public string? CoExpsRemarks { get; set; }

    public bool? CoExpsCheckIdssgross { get; set; }

    public decimal? CoExpsIdssgrossAmt { get; set; }

    public decimal? CoExpsIdssgrossAmtTopUp { get; set; }

    public decimal? CoExpsIdssgrossDayOrder { get; set; }

    public decimal? CoExpsIdssgrossPrevDayOrder { get; set; }

    public bool? CoExpsCheckIdss { get; set; }

    public decimal? CoExpsIdssamt { get; set; }

    public decimal? CoExpsIdssamtTopUp { get; set; }

    public decimal? CoExpsIdssdayOrder { get; set; }

    public decimal? CoExpsIdssprevDayOrder { get; set; }
}
