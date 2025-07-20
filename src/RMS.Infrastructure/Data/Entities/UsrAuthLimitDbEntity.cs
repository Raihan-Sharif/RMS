using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class UsrAuthLimitDbEntity
{
    public string UsrId { get; set; } = null!;

    /// <summary>
    /// Indicates if Authorization Limit for Buy should be checked.
    /// </summary>
    public bool? UsrAuthBuyFlag { get; set; }

    public decimal? UsrAuthBuyAmt { get; set; }

    /// <summary>
    /// Indicates if Authorization Limit for Sell should be checked.
    /// </summary>
    public bool? UsrAuthSellFlag { get; set; }

    public decimal? UsrAuthSellAmt { get; set; }

    /// <summary>
    /// Indicates if Authorization Limit for Total should be checked.
    /// </summary>
    public bool? UsrAuthTotalFlag { get; set; }

    public decimal? UsrAuthTotalAmt { get; set; }

    /// <summary>
    /// Indicates if Authorization Limit for Net should be checked.
    /// </summary>
    public bool? UsrAuthNetFlag { get; set; }

    public decimal? UsrAuthNetAmt { get; set; }

    public bool? UsrAuthBuyTopUpFlag { get; set; }

    public decimal? UsrAuthBuyTopUpAmt { get; set; }

    public bool? UsrAuthSellTopUpFlag { get; set; }

    public decimal? UsrAuthSellTopUpAmt { get; set; }

    public bool? UsrAuthTotalTopUpFlag { get; set; }

    public decimal? UsrAuthTotalTopUpAmt { get; set; }

    public bool? UsrAuthNetTopUpFlag { get; set; }

    public decimal? UsrAuthNetTopUpAmt { get; set; }

    public bool? UsrAuthFstflag { get; set; }

    public bool? UsrAuthFsttopUpFlag { get; set; }

    public decimal? UsrAuthFsttopUpAmt { get; set; }

    public decimal? UsrAuthFstamt { get; set; }

    public bool? UsrAuthIdssgrossFlag { get; set; }

    public decimal? UsrAuthIdssgrossAmt { get; set; }

    public bool? UsrAuthIdssflag { get; set; }

    public decimal? UsrAuthIdssamt { get; set; }

    public bool? UsrAuthIdssgrossTopUpFlag { get; set; }

    public decimal? UsrAuthIdssgrossTopUpAmt { get; set; }

    public bool? UsrAuthIdsstopUpFlag { get; set; }

    public decimal? UsrAuthIdsstopUpAmt { get; set; }
}
