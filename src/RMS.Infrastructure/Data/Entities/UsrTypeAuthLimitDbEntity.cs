using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class UsrTypeAuthLimitDbEntity
{
    public int UsrType { get; set; }

    /// <summary>
    /// Indicates if Authorization Limit for Buy should be checked.
    /// </summary>
    public bool? UsrTypeAuthBuyFlag { get; set; }

    public decimal? UsrTypeAuthBuyAmt { get; set; }

    /// <summary>
    /// Indicates if Authorization Limit for Sell should be checked.
    /// </summary>
    public bool? UsrTypeAuthSellFlag { get; set; }

    public decimal? UsrTypeAuthSellAmt { get; set; }

    /// <summary>
    /// Indicates if Authorization Limit for Total should be checked.
    /// </summary>
    public bool? UsrTypeAuthTotalFlag { get; set; }

    public decimal? UsrTypeAuthTotalAmt { get; set; }

    /// <summary>
    /// Indicates if Authorization Limit for Net should be checked.
    /// </summary>
    public bool? UsrTypeAuthNetFlag { get; set; }

    public decimal? UsrTypeAuthNetAmt { get; set; }

    public bool? UsrTypeAuthBuyTopUpFlag { get; set; }

    public decimal? UsrTypeAuthBuyTopUpAmt { get; set; }

    public bool? UsrTypeAuthSellTopUpFlag { get; set; }

    public decimal? UsrTypeAuthSellTopUpAmt { get; set; }

    public bool? UsrTypeAuthTotalTopUpFlag { get; set; }

    public decimal? UsrTypeAuthTotalTopUpAmt { get; set; }

    public bool? UsrTypeAuthNetTopUpFlag { get; set; }

    public decimal? UsrTypeAuthNetTopUpAmt { get; set; }

    public bool? UsrTypeAuthFstflag { get; set; }

    public bool? UsrTypeAuthFsttopUpFlag { get; set; }

    public decimal? UsrTypeAuthFsttopUpAmt { get; set; }

    public decimal? UsrTypeAuthFstamt { get; set; }

    public bool? UsrTypeAuthIdssgrossFlag { get; set; }

    public decimal? UsrTypeAuthIdssgrossAmt { get; set; }

    public bool? UsrTypeAuthIdssflag { get; set; }

    public decimal? UsrTypeAuthIdssamt { get; set; }

    public bool? UsrTypeAuthIdssgrossTopUpFlag { get; set; }

    public decimal? UsrTypeAuthIdssgrossTopUpAmt { get; set; }

    public bool? UsrTypeAuthIdsstopUpFlag { get; set; }

    public decimal? UsrTypeAuthIdsstopUpAmt { get; set; }
}
