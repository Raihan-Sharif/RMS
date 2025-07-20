using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class FormOptLstDbEntity
{
    public string FormGrpId { get; set; } = null!;

    public string FormId { get; set; } = null!;

    public string? FormDesc { get; set; }

    public string? FormRedirectUrl { get; set; }

    public string? FormTitle { get; set; }

    public string? DeptDesc { get; set; }

    public decimal? CutOffTimeFr { get; set; }

    public decimal? CutOffTimeTo { get; set; }

    public string? FormTableName { get; set; }

    public string? DeptId { get; set; }
}
