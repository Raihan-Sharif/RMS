using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstLegalStatDbEntity
{
    public string LegalStatCode { get; set; } = null!;

    public string? LegalStatDesc { get; set; }
}
