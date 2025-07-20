using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class EcmmoneyProcessedRefNoDbEntity
{
    public DateTime DtCreateDate { get; set; }

    public string SPymtRefNo { get; set; } = null!;
}
