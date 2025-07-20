using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class CareOrderAssignmentDbEntity
{
    public string UsrId { get; set; } = null!;

    public string ClntCode { get; set; } = null!;

    public string CoBrchCode { get; set; } = null!;

    public int? IsAssigned { get; set; }

    public int? OrderNotify { get; set; }

    public int? PriceNotify { get; set; }

    public int? TimeInterval { get; set; }
}
