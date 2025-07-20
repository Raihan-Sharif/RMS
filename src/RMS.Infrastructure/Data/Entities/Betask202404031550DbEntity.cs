using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class Betask202404031550DbEntity
{
    public int TaskCat { get; set; }

    public string TaskId { get; set; } = null!;

    public int Priority { get; set; }

    public string? BrkCode { get; set; }

    public string TaskName { get; set; } = null!;

    public string ProgramName { get; set; } = null!;

    public decimal StartTime { get; set; }

    public string? InputParameter { get; set; }

    public string? InputFileName { get; set; }

    public string? InputFolderName { get; set; }

    public string? OutputFolderName { get; set; }

    public string? NonTrading { get; set; }

    public string? Status { get; set; }

    public string? NextDay { get; set; }

    public string? Availability { get; set; }

    public string? CheckTime { get; set; }

    public decimal? RtnTrueTime { get; set; }

    public string? OldDependency { get; set; }

    public string? Notes { get; set; }
}
