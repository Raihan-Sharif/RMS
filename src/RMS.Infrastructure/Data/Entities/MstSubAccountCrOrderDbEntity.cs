using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstSubAccountCrOrderDbEntity
{
    public string SubAccount { get; set; } = null!;

    public string ClntCode { get; set; } = null!;
}
