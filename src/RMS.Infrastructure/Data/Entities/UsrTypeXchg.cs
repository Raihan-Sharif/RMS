using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class UsrTypeXchg
{
    public int UsrType { get; set; }

    public string XchgCode { get; set; } = null!;
}
