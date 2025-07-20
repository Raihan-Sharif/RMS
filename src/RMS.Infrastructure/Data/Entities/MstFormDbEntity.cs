using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstFormDbEntity
{
    public string DeptId { get; set; } = null!;

    public string FormId { get; set; } = null!;

    public string? FormDesc { get; set; }

    public decimal? CutOffTimeFr { get; set; }

    public decimal? CutOffTimeTo { get; set; }

    public string? Msg { get; set; }

    public string? PageEdit { get; set; }

    public string? PageView { get; set; }

    public string? SubmitAfterCutOffTime { get; set; }
}
