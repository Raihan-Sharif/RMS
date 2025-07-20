using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class UsrXchgLog
{
    public int Id { get; set; }

    public string UsrId { get; set; } = null!;

    public string XchgCode { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string? ExstValMode { get; set; }

    public string? NewValMode { get; set; }

    public DateTime? ExstValStartDate { get; set; }

    public DateTime? NewValStartDate { get; set; }

    public DateTime? ExstValEndDate { get; set; }

    public DateTime? NewValEndDate { get; set; }

    public string? ExstValEnabled { get; set; }

    public string? NewValEnabled { get; set; }

    public string? ExstValMktDepth { get; set; }

    public string? NewValMktDepth { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? AppName { get; set; }

    public string? HostName { get; set; }

    public string? ActionType { get; set; }
}
