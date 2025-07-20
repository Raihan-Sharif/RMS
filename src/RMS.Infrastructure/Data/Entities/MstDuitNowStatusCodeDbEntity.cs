using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstDuitNowStatusCodeDbEntity
{
    public string StatusCode { get; set; } = null!;

    public string StatusName { get; set; } = null!;

    public string StatusDescription { get; set; } = null!;
}
