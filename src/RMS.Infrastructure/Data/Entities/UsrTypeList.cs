using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class UsrTypeList
{
    public int UsrType { get; set; }

    public string? UsrTypeDesc { get; set; }

    /// <summary>
    /// 1 - system default, user can&apos;t maintain; 0 - not system default, user can maintain
    /// </summary>
    public bool? UsrTypeDefault { get; set; }

    public int? SeqNo { get; set; }

    /// <summary>
    /// Y - Yes; N - No;
    /// </summary>
    public string? Allow2Trade { get; set; }

    /// <summary>
    /// Y - Yes; N - No;
    /// </summary>
    public string? IsSuperior { get; set; }

    /// <summary>
    /// Y - Yes; N - No;
    /// </summary>
    public string? IsNotifier { get; set; }

    /// <summary>
    /// Y - Yes; N - No;
    /// </summary>
    public string? HaveSuperior { get; set; }

    /// <summary>
    /// Y - Yes; N - No;
    /// </summary>
    public string? HaveNotifier { get; set; }

    /// <summary>
    /// Y - Yes; N - No;
    /// </summary>
    public string? CanDbtoverride { get; set; }

    public string? DealerInd { get; set; }

    public string? Rmsind { get; set; }

    public string? ShareCheckInd { get; set; }

    public int? PwdExpDays { get; set; }

    public int? MaxMrktDataHist { get; set; }

    public int? MaxPrcHist { get; set; }

    public int? Timeout { get; set; }

    public string? ShareCheckIndWith { get; set; }
}
