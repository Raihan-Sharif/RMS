using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class NewTableDbEntity
{
    public int TaskCategory { get; set; }

    public string TaskId { get; set; } = null!;

    public string TaskName { get; set; } = null!;

    public string ProcessProgram { get; set; } = null!;

    public string? StartTime { get; set; }

    public string InputParameter { get; set; } = null!;

    public string? InputFilePath { get; set; }

    public string OutputFile { get; set; } = null!;

    public string BrkCode { get; set; } = null!;

    public string NextDay { get; set; } = null!;

    public string? ProcessOnNonTradingDay { get; set; }

    public string? ForceSuccess { get; set; }

    public string? ForceSuccessTime { get; set; }

    public string Frequency { get; set; } = null!;

    public string TaskDependencyById { get; set; } = null!;

    public string? Phrase { get; set; }
}
