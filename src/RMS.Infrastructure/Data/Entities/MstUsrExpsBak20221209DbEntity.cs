using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstUsrExpsBak20221209DbEntity
{
    public string UsrId { get; set; } = null!;

    public bool? UsrExpsCheckBuy { get; set; }

    public decimal? UsrExpsBuyAmt { get; set; }

    public decimal? UsrExpsBuyAmtTopUp { get; set; }

    public decimal? UsrExpsBuyDayOrder { get; set; }

    public decimal? UsrExpsBuyPrevDayOrder { get; set; }

    public bool? UsrExpsCheckSell { get; set; }

    public decimal? UsrExpsSellAmt { get; set; }

    public decimal? UsrExpsSellAmtTopUp { get; set; }

    public decimal? UsrExpsSellDayOrder { get; set; }

    public decimal? UsrExpsSellPrevDayOrder { get; set; }

    public bool? UsrExpsCheckTotal { get; set; }

    public decimal? UsrExpsTotalAmt { get; set; }

    public decimal? UsrExpsTotalAmtTopUp { get; set; }

    public decimal? UsrExpsTotalDayOrder { get; set; }

    public decimal? UsrExpsTotalPrevDayOrder { get; set; }

    public bool? UsrExpsCheckNet { get; set; }

    public decimal? UsrExpsNetAmt { get; set; }

    public decimal? UsrExpsNetAmtTopUp { get; set; }

    public decimal? UsrExpsNetDayOrder { get; set; }

    public decimal? UsrExpsNetPrevDayOrder { get; set; }

    public bool? UsrExpsWithLimit { get; set; }

    public decimal? UsrExpsAddLimitPctg { get; set; }

    public int? UsrTradeStatus { get; set; }

    public decimal? UsrBuyTrnxLimit { get; set; }

    public int? UsrBuyLotLimit { get; set; }

    public int? UsrBuyBidLimit { get; set; }

    public decimal? UsrSellTrnxLimit { get; set; }

    public int? UsrSellLotLimit { get; set; }

    public int? UsrSellBidLimit { get; set; }

    public string? UsrRemarks { get; set; }

    public int? UsrNormalOrderType { get; set; }

    public int? UsrOddOrderType { get; set; }

    public int? UsrMrktOrder { get; set; }

    public bool? UsrDbtoverride { get; set; }

    public decimal? UsrCollateralCashValue { get; set; }

    public decimal? UsrCollateralCashMultiplier { get; set; }

    public decimal? UsrCollateralStockValue { get; set; }

    public decimal? UsrCollateralStockMultiplier { get; set; }

    public decimal? UsrCollateralNonStockValue { get; set; }

    public decimal? UsrCollateralNonStockMultiplier { get; set; }

    public string? UsrExpsRemarks { get; set; }

    public int? UsrTradeCtrl { get; set; }

    public int? UsrOrderModality { get; set; }

    public bool? UsrTdayAmend { get; set; }

    public int? UsrOrderType { get; set; }

    public bool? UsrIrregularTrds { get; set; }

    public bool? UsrExpsWithShrLimit { get; set; }

    public int? CtrlType { get; set; }

    public int? UsrBuyBidLmtLwr { get; set; }

    public int? UsrBuyBidLmtHgh { get; set; }

    public decimal? UsrBuyBidLmtPctLwr { get; set; }

    public decimal? UsrBuyBidLmtPctHgh { get; set; }

    public int? UsrSellBidLmtLwr { get; set; }

    public int? UsrSellBidLmtHgh { get; set; }

    public decimal? UsrSellBidLmtPctLwr { get; set; }

    public decimal? UsrSellBidLmtPctHgh { get; set; }

    public int? CtrlTypeEcos { get; set; }

    public int? UsrBuyEcosbidLmtLwr { get; set; }

    public int? UsrBuyEcosbidLmtHgh { get; set; }

    public decimal? UsrBuyEcosbidLmtPctLwr { get; set; }

    public decimal? UsrBuyEcosbidLmtPctHgh { get; set; }

    public int? UsrSellEcosbidLmtLwr { get; set; }

    public int? UsrSellEcosbidLmtHgh { get; set; }

    public decimal? UsrSellEcosbidLmtPctLwr { get; set; }

    public decimal? UsrSellEcosbidLmtPctHgh { get; set; }

    public long? UsrSplitOrderMaxQty { get; set; }

    public bool UsrBidLmtOddCheck { get; set; }

    public bool UsrEcosbidLmtOddCheck { get; set; }

    public bool? UsrExpsCheckIdssgross { get; set; }

    public decimal? UsrExpsIdssgrossAmt { get; set; }

    public decimal? UsrExpsIdssgrossAmtTopUp { get; set; }

    public decimal? UsrExpsIdssgrossDayOrder { get; set; }

    public decimal? UsrExpsIdssgrossPrevDayOrder { get; set; }

    public bool? UsrExpsCheckIdss { get; set; }

    public decimal? UsrExpsIdssamt { get; set; }

    public decimal? UsrExpsIdssamtTopUp { get; set; }

    public decimal? UsrExpsIdssdayOrder { get; set; }

    public decimal? UsrExpsIdssprevDayOrder { get; set; }
}
