using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class OsclntInfoBatch2
{
    public string SClntCode { get; set; } = null!;

    public string SCoBrchCode { get; set; } = null!;

    public string SCurrency { get; set; } = null!;
}
