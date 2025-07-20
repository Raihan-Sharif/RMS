using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ClntStkCtrlDbEntity
{
    public string ClntCode { get; set; } = null!;

    public string CoBrchCode { get; set; } = null!;

    public string XchgCode { get; set; } = null!;

    public string StkCode { get; set; } = null!;

    public string CtrlStatus { get; set; } = null!;
}
