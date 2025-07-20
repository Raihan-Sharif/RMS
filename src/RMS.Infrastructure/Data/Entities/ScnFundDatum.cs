using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ScnFundDatum
{
    public string StkCode { get; set; } = null!;

    public string XchgCode { get; set; } = null!;

    public decimal? DMktCap { get; set; }

    public decimal? DRevenue { get; set; }

    public decimal? DEbitda { get; set; }

    public decimal? DProfit { get; set; }

    public decimal? DProfitAttTo { get; set; }

    public decimal? DAvgEqtHld { get; set; }

    public decimal? DCurRatio { get; set; }

    public decimal? DQuickRatio { get; set; }

    public decimal? DTrdRcvblsCycle { get; set; }

    public decimal? DTrdPayblsCycle { get; set; }

    public decimal? DInventoryCycle { get; set; }

    public decimal? DFreeCashFlow { get; set; }

    public decimal? DNetOprCashFlow { get; set; }

    public decimal? DOprCashFlow { get; set; }

    public decimal? DDebtCvg { get; set; }

    public decimal? DFreeCashFlowDebt { get; set; }

    public decimal? DNetDebtCvg { get; set; }

    public decimal? DIntCvg { get; set; }

    public decimal? DGearingRatio { get; set; }

    public decimal? DYoyrevenue { get; set; }

    public decimal? DYoygrossProfit { get; set; }

    public decimal? DYoyebitda { get; set; }

    public decimal? DYoyfreeCashFlow { get; set; }

    public decimal? DGrossProfitMrgn { get; set; }

    public decimal? DNetProfitMrgn { get; set; }

    public decimal? DEbitdamrgn { get; set; }

    public decimal? DRetOnCapEmply { get; set; }

    public decimal? DRetOnAssets { get; set; }

    public decimal? DRetOnShareholderEqt { get; set; }

    public decimal? DBeta { get; set; }

    public decimal? DMktCapEv { get; set; }

    public decimal? DPeratio { get; set; }

    public decimal? DEvtoFcf { get; set; }

    public decimal? DEvtoEbitda { get; set; }

    public decimal? DPriceToBook { get; set; }

    public decimal? DPriceToRevenue { get; set; }

    public decimal? DPriceToEbitda { get; set; }

    public decimal? DPriceToOprCashFlow { get; set; }

    public decimal? DPriceToFcf { get; set; }

    public decimal? DGrossDvdnPerShare { get; set; }

    public decimal? DNetDvdnPerShare { get; set; }

    public decimal? DGrossDvdnYield { get; set; }

    public decimal? DNetDvdnYield { get; set; }

    public decimal? DDvdnPayoutRatio { get; set; }

    public decimal? DNetCashPerShare { get; set; }

    public decimal? DNetAssetsPerShare { get; set; }

    public decimal? DNetTgbAssetPerShare { get; set; }

    public decimal? DBasicEps { get; set; }
}
