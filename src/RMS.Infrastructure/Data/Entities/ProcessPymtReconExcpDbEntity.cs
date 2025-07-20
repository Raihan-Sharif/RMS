using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ProcessPymtReconExcpDbEntity
{
    public DateTime? ExcpDate { get; set; }

    public string? ClntCode { get; set; }

    public string? PymtRefNo { get; set; }

    public string? ContNo { get; set; }

    public DateTime? ContDate { get; set; }

    public int? DbexcpCode { get; set; }

    public string? ExcpDetail { get; set; }
}
