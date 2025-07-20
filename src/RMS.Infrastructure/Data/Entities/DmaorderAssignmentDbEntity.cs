using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class DmaorderAssignmentDbEntity
{
    public string UsrId { get; set; } = null!;

    public string ClntCode { get; set; } = null!;

    public string CoBrchCode { get; set; } = null!;

    public int? DisabledDma { get; set; }
}
