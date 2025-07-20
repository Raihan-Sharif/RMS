using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class XlogStkCtrlDbEntity
{
    public int SeqNo { get; set; }

    public DateTime LogDate { get; set; }

    public string LogAction { get; set; } = null!;

    public string? LogUsr { get; set; }

    /// <summary>
    /// C - Company; B - Branch; P - Product; N - Client; 
    /// </summary>
    public string DataType { get; set; } = null!;

    public string DataCode { get; set; } = null!;

    /// <summary>
    /// X - Exchange; B - Board; S - Sector; T - Stock; 
    /// </summary>
    public string CtrlType { get; set; } = null!;

    public string CtrlCode { get; set; } = null!;

    public string Item { get; set; } = null!;

    public string? ExstVal { get; set; }

    public string? NewVal { get; set; }
}
