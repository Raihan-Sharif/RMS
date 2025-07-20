using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class StkCtrlDbEntity
{
    /// <summary>
    /// C - Company; B - Branch; P - Product; N - Client; 
    /// </summary>
    public string DataType { get; set; } = null!;

    public string DataCode { get; set; } = null!;

    public string? CoCode { get; set; }

    public string? CoBrchCode { get; set; }

    public string? UsrId { get; set; }

    public string? ClntCode { get; set; }

    /// <summary>
    /// X - Exchange; B - Board; S - Sector; T - Stock; 
    /// </summary>
    public string CtrlType { get; set; } = null!;

    public string CtrlCode { get; set; } = null!;

    public string? XchgCode { get; set; }

    public string? BrdCode { get; set; }

    public string? SectCode { get; set; }

    public string? StkCode { get; set; }

    /// <summary>
    /// Y - Allow Both Buy &amp; Sell; B - Suspend Buy; S - Suspend Sell; N - Suspend Both Buy &amp; Sell; 
    /// </summary>
    public string? CtrlStatus { get; set; }

    public string? ClntType { get; set; }
}
