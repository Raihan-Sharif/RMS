using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class FormTrnxDtl
{
    public string FormGrpId { get; set; } = null!;

    public string FormId { get; set; } = null!;

    public string FormRefNo { get; set; } = null!;

    public DateTime TrnxDate { get; set; }

    public string? ApplyBy { get; set; }

    public DateTime? PrintDate { get; set; }
}
