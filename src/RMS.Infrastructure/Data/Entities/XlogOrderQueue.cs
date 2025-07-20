using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class XlogOrderQueue
{
    public int SeqNo { get; set; }

    public DateTime LogDate { get; set; }

    public string LogAction { get; set; } = null!;

    public string? LogUsr { get; set; }

    public string? DataCode { get; set; }

    public string Item { get; set; } = null!;

    public string? ExstVal { get; set; }

    public string? NewVal { get; set; }

    public int? Sequence { get; set; }

    public string? ClientCode { get; set; }

    public string? AmendClientCode { get; set; }

    public int? SequenceNo { get; set; }

    public string? Remarks { get; set; }

    public string? ApprvRemarks { get; set; }
}
