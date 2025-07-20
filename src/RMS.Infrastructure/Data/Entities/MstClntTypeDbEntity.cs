using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstClntTypeDbEntity
{
    public string ClntType { get; set; } = null!;

    public string? ClntTypeDesc { get; set; }
}
