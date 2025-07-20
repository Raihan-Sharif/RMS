using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstStkProd
{
    public string XchgCode { get; set; } = null!;

    public string? StkBrdCode { get; set; }

    public string? StkSectCode { get; set; }

    public string StkCode { get; set; } = null!;

    public string? StkLname { get; set; }

    public string? StkSname { get; set; }

    public string? StkStat { get; set; }

    public decimal? StkLastDonePrice { get; set; }

    public decimal? StkClosePrice { get; set; }

    public decimal? StkRefPrc { get; set; }

    public decimal? StkUpperLmtPrice { get; set; }

    public decimal? StkLowerLmtPrice { get; set; }

    public string? StkIsSyariah { get; set; }

    public int? StkLot { get; set; }

    public string? StkDeliveryBasis { get; set; }

    public string? InstrumentTypeCode { get; set; }

    public DateTime? LastUpdateDate { get; set; }

    public long? StkShareIssue { get; set; }

    public string? Isin { get; set; }

    public string? Currency { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public long? StkVolumeTraded { get; set; }

    public int? StkIndexCode { get; set; }

    public string? ShortSellInd { get; set; }

    public string? Entitlement { get; set; }

    public string? SecurityType { get; set; }

    public DateTime? ListingDate { get; set; }

    public string? Pn17gn3ind { get; set; }
}
