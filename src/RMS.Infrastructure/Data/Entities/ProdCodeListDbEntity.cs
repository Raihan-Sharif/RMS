using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ProdCodeListDbEntity
{
    public int ProdCode { get; set; }

    public string? ProdDesc { get; set; }
}
