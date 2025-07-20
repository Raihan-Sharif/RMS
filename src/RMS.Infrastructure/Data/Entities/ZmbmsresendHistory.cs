using System;
using System.Collections.Generic;

namespace RMS.Infrastructure.Data.Entities;

public partial class ZmbmsresendHistory
{
    public int SeqNo { get; set; }

    public int TransCode { get; set; }

    public string? Status { get; set; }

    public DateTime? SubmissionTime { get; set; }

    public DateTime? CompletionTime { get; set; }

    public string? LogUsr { get; set; }

    public int Session { get; set; }

    public int? MbmstradeId { get; set; }
}
