using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstCountryExpDbEntity
{
    public string CountryCode { get; set; } = null!;

    public bool? CountryExpsCheckBuy { get; set; }

    public decimal? CountryExpsBuyAmt { get; set; }

    public decimal? CountryExpsBuyAmtTopUp { get; set; }

    public decimal? CountryExpsBuyDayOrder { get; set; }

    public decimal? CountryExpsBuyPrevDayOrder { get; set; }

    public bool? CountryExpsCheckSell { get; set; }

    public decimal? CountryExpsSellAmt { get; set; }

    public decimal? CountryExpsSellAmtTopUp { get; set; }

    public decimal? CountryExpsSellDayOrder { get; set; }

    public decimal? CountryExpsSellPrevDayOrder { get; set; }

    public bool? CountryExpsCheckTotal { get; set; }

    public decimal? CountryExpsTotalAmt { get; set; }

    public decimal? CountryExpsTotalAmtTopUp { get; set; }

    public decimal? CountryExpsTotalDayOrder { get; set; }

    public decimal? CountryExpsTotalPrevDayOrder { get; set; }

    public bool? CountryExpsCheckNet { get; set; }

    public decimal? CountryExpsNetAmt { get; set; }

    public decimal? CountryExpsNetAmtTopUp { get; set; }

    public decimal? CountryExpsNetDayOrder { get; set; }

    public decimal? CountryExpsNetPrevDayOrder { get; set; }

    public bool? CountryExpsWithLimit { get; set; }

    public decimal? CountryExpsAddLimitPctg { get; set; }

    public decimal? CountryNormalUpLimitPctg { get; set; }

    public decimal? CountryNormalLowLimitPctg { get; set; }

    public decimal? CountryDbtupLimitPctg { get; set; }

    public decimal? CountryDbtlowLimitPctg { get; set; }

    public int? CountryTradeStatus { get; set; }

    public decimal? CountryBuyTrnxLimit { get; set; }

    public int? CountryBuyLotLimit { get; set; }

    public int? CountryBuyBidLimitNormal { get; set; }

    public int? CountryBuyBidLimitOdd { get; set; }

    public int? CountryBuyBidLimitNormalOdd { get; set; }

    public int? CountryDbtbuyShrLimit { get; set; }

    public decimal? CountrySellTrnxLimit { get; set; }

    public int? CountrySellLotLimit { get; set; }

    public int? CountrySellBidLimitNormal { get; set; }

    public int? CountrySellBidLimitOdd { get; set; }

    public int? CountrySellBidLimitNormalOdd { get; set; }

    public int? CountryDbtsellShrLimit { get; set; }

    public string? CountryRemarks { get; set; }

    public bool? CountryDbtcancel { get; set; }
}
