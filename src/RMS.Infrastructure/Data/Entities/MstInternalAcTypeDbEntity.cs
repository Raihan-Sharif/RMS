using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstInternalAcTypeDbEntity
{
    public string InternalAcTypeCode { get; set; } = null!;

    public string? InternalAcTypeDesc { get; set; }
}
