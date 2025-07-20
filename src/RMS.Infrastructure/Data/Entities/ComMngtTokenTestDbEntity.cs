using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ComMngtTokenTestDbEntity
{
    public int? SeqNo { get; set; }

    public string? AcctCd { get; set; }

    public string? Nric { get; set; }

    public string? ProjCd { get; set; }

    public string? Token { get; set; }

    public DateTime? ExpiryDt { get; set; }
}
