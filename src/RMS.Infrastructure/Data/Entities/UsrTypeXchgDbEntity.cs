using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class UsrTypeXchgDbEntity
{
    public int UsrType { get; set; }

    public string XchgCode { get; set; } = null!;
}
