using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class XlogOn9Tagging
{
    public int SeqNo { get; set; }

    public DateTime LogDate { get; set; }

    public string? BranchId { get; set; }

    public string? ClntCode { get; set; }

    public string? On9Ind { get; set; }

    public string? Status { get; set; }

    public string? Remarks { get; set; }
}
