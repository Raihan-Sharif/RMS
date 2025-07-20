using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class StkCtrl20220303DbEntity
{
    public string DataType { get; set; } = null!;

    public string DataCode { get; set; } = null!;

    public string? CoCode { get; set; }

    public string? CoBrchCode { get; set; }

    public string? UsrId { get; set; }

    public string? ClntCode { get; set; }

    public string CtrlType { get; set; } = null!;

    public string CtrlCode { get; set; } = null!;

    public string? XchgCode { get; set; }

    public string? BrdCode { get; set; }

    public string? SectCode { get; set; }

    public string? StkCode { get; set; }

    public string? CtrlStatus { get; set; }

    public string? ClntType { get; set; }
}
