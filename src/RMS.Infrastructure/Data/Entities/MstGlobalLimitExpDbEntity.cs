using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstGlobalLimitExpDbEntity
{
    public string ExpsType { get; set; } = null!;

    public decimal? LimitChangePctg { get; set; }

    public string Type { get; set; } = null!;
}
