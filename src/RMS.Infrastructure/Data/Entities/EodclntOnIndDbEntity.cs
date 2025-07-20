using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class EodclntOnIndDbEntity
{
    public string BranchCode { get; set; } = null!;

    public string ClntCode { get; set; } = null!;

    public string OnlineInd { get; set; } = null!;

    public DateTime? ProcessDate { get; set; }
}
