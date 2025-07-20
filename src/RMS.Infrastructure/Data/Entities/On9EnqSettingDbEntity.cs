using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class On9EnqSettingDbEntity
{
    public int On9EnqId { get; set; }

    public string? On9EnqDesc { get; set; }

    public string? On9EnqDescVn { get; set; }

    public string? On9EnqRedirectUrl { get; set; }
}
