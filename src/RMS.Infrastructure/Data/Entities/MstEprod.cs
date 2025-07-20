using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstEprod
{
    public string ClientType { get; set; } = null!;

    public string? ProdCode { get; set; }

    public string? ProdDesc { get; set; }
}
