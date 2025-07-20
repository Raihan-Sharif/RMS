using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class MstDocument
{
    public int DocId { get; set; }

    public string? DocType { get; set; }

    public string? ClntCode { get; set; }

    public string? ClntCdsno { get; set; }

    public string? CoBrchCode { get; set; }

    public DateTime? DocDate { get; set; }

    public string? DocTitle { get; set; }

    public string? DocFileName { get; set; }
}
