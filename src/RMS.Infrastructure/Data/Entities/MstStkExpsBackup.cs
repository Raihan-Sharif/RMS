using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstStkExpsBackup
{
    public string XchgCode { get; set; } = null!;

    public string StkCode { get; set; } = null!;

    public bool? StkExpsCheckBuy { get; set; }

    public decimal? StkExpsBuyAmt { get; set; }

    public decimal? StkExpsBuyAmtTopUp { get; set; }

    public decimal? StkExpsBuyDayOrder { get; set; }

    public decimal? StkExpsBuyPrevDayOrder { get; set; }

    public bool? StkExpsCheckSell { get; set; }

    public decimal? StkExpsSellAmt { get; set; }

    public decimal? StkExpsSellAmtTopUp { get; set; }

    public decimal? StkExpsSellDayOrder { get; set; }

    public decimal? StkExpsSellPrevDayOrder { get; set; }

    public bool? StkExpsCheckTotal { get; set; }

    public decimal? StkExpsTotalAmt { get; set; }

    public decimal? StkExpsTotalAmtTopUp { get; set; }

    public decimal? StkExpsTotalDayOrder { get; set; }

    public decimal? StkExpsTotalPrevDayOrder { get; set; }

    public bool? StkExpsCheckNet { get; set; }

    public decimal? StkExpsNetAmt { get; set; }

    public decimal? StkExpsNetAmtTopUp { get; set; }

    public decimal? StkExpsNetDayOrder { get; set; }

    public decimal? StkExpsNetPrevDayOrder { get; set; }

    public bool? StkExpsWithLimit { get; set; }

    public decimal? StkExpsAddLimitPctg { get; set; }

    public int? StkTradeStatus { get; set; }

    public int? StkTradeAllow { get; set; }

    public int? StkList { get; set; }

    public string? StkRemarks { get; set; }

    public string? StkExpsRemarks { get; set; }

    public string CoBrchCode { get; set; } = null!;

    public string CoInd { get; set; } = null!;

    public int? StkExpsBuyMaxQty { get; set; }

    public int? StkExpsBuyBidLimit { get; set; }

    public int? StkExpsSellMaxQty { get; set; }

    public int? StkExpsSellBidLimit { get; set; }

    public decimal? StkExpsDefaultLimit { get; set; }

    public int? CtrlType { get; set; }

    public int? CtrlTypeEcos { get; set; }

    public int? StkExpsEcosbuyBidLimit { get; set; }

    public int? StkExpsEcossellBidLimit { get; set; }
}
