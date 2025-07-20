using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class EDocument
{
    public int DocId { get; set; }

    public string? DocType { get; set; }

    public string? ClntCode { get; set; }

    public string? BrchCode { get; set; }

    public DateTime? DocDate { get; set; }

    public string? DocTitle { get; set; }

    public string? DocFileName { get; set; }

    public DateTime ProcDate { get; set; }

    public int? PublishStatus { get; set; }
}
