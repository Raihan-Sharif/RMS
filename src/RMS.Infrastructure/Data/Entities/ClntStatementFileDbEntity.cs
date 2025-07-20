using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ClntStatementFileDbEntity
{
    public string FileId { get; set; } = null!;

    public DateTime FileDate { get; set; }

    /// <summary>
    /// M - Monthly Statement; C - Contract Statement; R - Contra Statement;
    /// </summary>
    public string FileType { get; set; } = null!;

    public string CoBrchCode { get; set; } = null!;

    public string ClntCode { get; set; } = null!;

    public string? FileName { get; set; }

    public string? FileDesc { get; set; }
}
