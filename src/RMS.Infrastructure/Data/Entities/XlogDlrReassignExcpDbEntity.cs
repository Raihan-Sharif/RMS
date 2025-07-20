using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class XlogDlrReassignExcpDbEntity
{
    public int SeqNo { get; set; }

    public DateTime LogDate { get; set; }

    public string? BranchId { get; set; }

    public string? ClntCode { get; set; }

    public string? DealerId { get; set; }

    public string? ReassignDealerId { get; set; }

    public string? PermanentTemporary { get; set; }

    public string? Remarks { get; set; }
}
